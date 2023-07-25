using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace Wezit
{
    public class DataGrabber : Singleton<DataGrabber>
    {
        #region Saved data
        [Serializable]
        public class ImageAndMd5
        {
            [FormerlySerializedAs("path")]
            [SerializeField]
            public string path;

            [FormerlySerializedAs("md5")]
            [SerializeField]
            public string md5;

            public ImageAndMd5(string a_path, string a_md5)
            {
                this.path = a_path;
                this.md5 = a_md5;
            }
        }

        [FormerlySerializedAs("DownloadedImagesMD5Dict")]
        [SerializeField]
        public List<ImageAndMd5> DownloadedImagesMd5Dict = new List<ImageAndMd5>();

        [FormerlySerializedAs("HasDownloaded")]
        [SerializeField]
        public bool HasDownloaded = false;
        #endregion

        #region Fields
        private string m_FileContent;
        private string m_FilePath;
        private Thread m_SavingThread;
        private bool m_Saving;

        private bool m_StopDownload = false;
        private List<WezitAssets.Asset> m_Assets = new List<WezitAssets.Asset>();
        private UnityWebRequest m_WebRequest;

        public static string ImagesFolderPath { get { return Path.Combine(UnityEngine.Application.persistentDataPath, "wezit"); } }
        public List<WezitAssets.Asset> Assets {  get { return m_Assets; } }
        public string AppDefaultTransformation;
        #endregion

        #region Properties
        public UnityEvent<int> DownloadProgress = new UnityEvent<int>();
        public UnityEvent DownloadOver = new UnityEvent();
        #endregion

        #region Methods
        public List<ImageAndMd5> GetImagesAndMd5Dict()
        {
            Load();
            return DownloadedImagesMd5Dict;
        }

        public async UniTask GetAllAssets(string transformation = "")
        {
            m_StopDownload = false;
            Load();
            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }

            if (m_Assets.Count == 0)
            {
                if(AssetsLoader.Assets.Count == 0)
                {
                    await AssetsLoader.Init(true);
                }
                else
                {
                    m_Assets = AssetsLoader.Assets;
                }
            }

            int currentDownloaded = 0;
            int currentCounter = 0;
            foreach (WezitAssets.Asset asset in m_Assets)
            {
                if (m_StopDownload) return;
                await DownloadAsset(asset, currentDownloaded, currentCounter, transformation);
            }
            HasDownloaded = true;
            Save();
            Wezit.FilesDownloader.SqliteUpdated = false;
            DownloadOver?.Invoke();
        }

        public async UniTask GetAssetsForTour(string tourId, string transformation = "")
        {
            m_StopDownload = false;
            Load();

            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }
            int currentDownloaded = 0;
            int currentCounter = 0;
            List<WezitAssets.Asset> tourAssets = AssetsLoader.GetAssetsForTour(tourId);
            foreach (WezitAssets.Asset asset in tourAssets)
            {
                if (m_StopDownload) return;
                (currentDownloaded, currentCounter) = await DownloadAsset(asset, currentDownloaded, currentCounter, transformation);
            }
            HasDownloaded = true;
            Save();
            Wezit.FilesDownloader.SqliteUpdated = false;
            DownloadOver?.Invoke();
        }

        public async UniTask GetSettingsAssets(string transformation = "")
        {
            m_StopDownload = false;
            Load();

            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }
            int currentDownloaded = 0;
            int currentCounter = 0;
            List<WezitAssets.Asset> tourAssets = AssetsLoader.GetAllSettingsAssets();
            foreach (WezitAssets.Asset asset in tourAssets)
            {
                if (m_StopDownload) return;
                (currentDownloaded, currentCounter) = await DownloadAsset(asset, currentDownloaded, currentCounter, transformation);
            }
            HasDownloaded = true;
            Save();
            Wezit.FilesDownloader.SqliteUpdated = false;
            DownloadOver?.Invoke();
        }

        // Download size
        public int GetDownloadSize(string transformation = "")
        {
            int downloadSize = 0;
            if(string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }

            foreach(WezitAssets.Asset asset in m_Assets)
            {
                foreach (WezitAssets.File file in asset.files)
                {
                    if (file.label == transformation || transformation == "all")
                    {
                        downloadSize += file.size;
                    }
                }
            }
            return downloadSize;
        }

        public int GetDownloadSizeForAssets(List<WezitAssets.Asset> assets, string transformation = "all")
        {
            int downloadSize = 0;
            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }

            foreach (WezitAssets.Asset asset in assets)
            {
                foreach (WezitAssets.File file in asset.files)
                {
                    if (file.label == transformation || transformation == "all")
                    {
                        downloadSize += file.size;
                    }
                }
            }
            return downloadSize;
        }

        public int GetDownloadSizeForTour(string tourId, string transformation = "all")
        {
            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }

            List<WezitAssets.Asset> tourAssets = AssetsLoader.GetAssetsForTour(tourId);
            return (GetDownloadSizeForAssets(tourAssets, transformation));
        }

        // Update size
        public int GetUpdateSizeForAssets(List<WezitAssets.Asset> assets, string transformation = "")
        {
            Load();
            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }

            List<WezitAssets.File> filesToUpdate = new List<WezitAssets.File>();
            foreach (WezitAssets.Asset asset in (assets == null ? m_Assets : assets))
            {
                string assetTransformation = asset.usages.Contains("maps") ? "tiles-zip" : transformation;

                foreach (WezitAssets.File file in asset.files)
                {
                    if (file.label != assetTransformation && assetTransformation != "all")
                    {
                        continue;
                    }

                    ImageAndMd5 imageMd5 = DownloadedImagesMd5Dict.Find(x => x.path == file.path);
                    if (imageMd5 != null)
                    {
                        if (imageMd5.md5 != file.md5)
                        {
                            Debug.Log("There is a file to update \nAsset name: " + asset.title + "\nFile uri: " + file.uri);
                            filesToUpdate.Add(file);
                        }
                    }
                    else
                    {
                        filesToUpdate.Add(file);
                        Debug.Log("There is a new file \n" + asset.title);
                    }
                }
            }
            int downloadSize = 0;
            int counter = filesToUpdate.Count;
            downloadSize = filesToUpdate.Sum(x => x.size);
            return downloadSize;
        }

        public int GetUpdateSizeForTour(string tourId, string transformation = "")
        {
            List<WezitAssets.Asset> tourAssets = AssetsLoader.GetAssetsForTour(tourId);
            if (string.IsNullOrEmpty(transformation))
            {
                transformation = AppDefaultTransformation;
            }

            return (GetUpdateSizeForAssets(tourAssets, transformation));
        }

        public int GetUpdateSize(string transformation = "")
        {
            return GetUpdateSizeForAssets(null, transformation);
        }

        public bool CheckDownloadNecessity(WezitAssets.File file)
        {
            ImageAndMd5 imageMd5 = DownloadedImagesMd5Dict.Find(x => x.path == file.path);
            if (imageMd5 != null)
            {
                if (imageMd5.md5 != file.md5)
                {
                    Debug.LogWarning(imageMd5.md5);
                    return true;
                }
                else return false;
            }
            else
            {
                return !File.Exists(Path.Combine(ImagesFolderPath, file.path));
            }
        }

        // Download
        public async UniTask<(int currentDownloaded, int currentCounter)> DownloadAsset(WezitAssets.Asset asset, int currentDownloaded, int currentCounter, string transformation = "all")
        {
            bool hasTransformation = false;
            if (asset.usages.Contains("maps"))
            {
                transformation = "tiles-zip";
            }
            foreach (WezitAssets.File file in asset.files)
            {
                if ((file.label == transformation) || (transformation == "all"))
                {
                    hasTransformation = true;
                    int downloaded = file.size;
                    if (CheckDownloadNecessity(file))
                    {
                        if(transformation == "tiles-zip")
                        {
                            downloaded = await DownloadMapTiles(file);
                        }
                        else
                        {
                            downloaded = await DownloadFile(file);
                        }
                    }

                    currentDownloaded += downloaded;
                    DownloadProgress?.Invoke(currentDownloaded);
                    currentCounter++;
                }
            }
            if (!hasTransformation)
            {
                WezitAssets.File file = asset.files.Find(x => x.label == "original");
                if (file == null) return (currentDownloaded, currentCounter);
                int downloaded = file.size;
                if (CheckDownloadNecessity(file)) downloaded = await DownloadFile(file);
                currentDownloaded += downloaded;
                DownloadProgress?.Invoke(currentDownloaded);
                currentCounter++;
            }
            return (currentDownloaded, currentCounter);
        }

        public async UniTask<int> DownloadFile(WezitAssets.File file)
        {
            m_WebRequest = UnityWebRequest.Get(file.uri);
            await m_WebRequest.SendWebRequest();
            if (!Directory.Exists(Path.Combine(ImagesFolderPath, Path.GetDirectoryName(file.path))))
            {
                Directory.CreateDirectory(Path.Combine(ImagesFolderPath, Path.GetDirectoryName(file.path)));
            }

            if (m_WebRequest.result == UnityWebRequest.Result.ConnectionError || m_WebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("WezitDownloader - DownloadImage - Error when downloading " + file.uri + " : " + m_WebRequest.error);
                m_WebRequest = null;
                return 0;
            }

            byte[] imageBytes = m_WebRequest.downloadHandler.data;
            using FileStream fileStream = File.Create(Path.Combine(ImagesFolderPath, file.path));
            fileStream.Write(imageBytes);
            DownloadedImagesMd5Dict.Add(new ImageAndMd5(file.path, file.md5));
            return (int)m_WebRequest.downloadedBytes;
        }

        private async UniTask<int> DownloadMapTiles(WezitAssets.File file)
        {
            if (!Directory.Exists(Path.Combine(ImagesFolderPath, Path.GetDirectoryName(file.path))))
            {
                Directory.CreateDirectory(Path.Combine(ImagesFolderPath, Path.GetDirectoryName(file.path)));
            }
            await UniRxZipDownloader.DownloadAndUnzip(file.uri, Path.Combine(ImagesFolderPath, file.path));

            DownloadedImagesMd5Dict.Add(new ImageAndMd5(file.path, file.md5));
            return file.size;
        }

        // Delete
        public void DeleteImages()
        {
            foreach (var directory in Directory.GetDirectories(ImagesFolderPath))
            {
                DirectoryInfo data_dir = new DirectoryInfo(directory);
                data_dir.Delete(true);
            }

            foreach (var file in Directory.GetFiles(ImagesFolderPath))
            {
                FileInfo file_info = new FileInfo(file);
                file_info.Delete();
            }
            HasDownloaded = false;
            m_Assets.Clear();
        }

        public void AbortDownload()
        {
            HasDownloaded = false;
            m_StopDownload = true;
            m_WebRequest.Abort();
            DownloadedImagesMd5Dict.Clear();
            DeleteImages();
        }

        #region Save/Load behavior
        const string CONST_FILE_NAME = "images.dat";

        private static string FilePath
        {
            get
            {
                return Path.Combine(ImagesFolderPath, CONST_FILE_NAME);
            }
        }

        public void Load(bool a_reset = false)
        {
            if (a_reset)
                return;

            FileStream file;

            if (File.Exists(FilePath))
            {
                file = File.OpenRead(FilePath);
                BinaryFormatter bf = new BinaryFormatter();
                string data = (string)bf.Deserialize(file);
                file.Close();
                JsonUtility.FromJsonOverwrite(data, this);

                Debug.Log("Images data loaded");
            }
            else
            {
                Debug.Log("There is no images data to load");
            }
        }

        public void Save()
        {
            m_FileContent = JsonUtility.ToJson(this, true);
            m_SavingThread = new Thread(SaveData);
            m_FilePath = FilePath;
            if (!m_Saving) m_SavingThread.Start();
        }

        public void SaveData()
        {
            m_Saving = true;
            FileStream file;

            if (File.Exists(m_FilePath))
            {
                File.WriteAllText(m_FilePath, string.Empty);
                file = File.OpenWrite(m_FilePath);
            }
            else file = File.Create(m_FilePath);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, m_FileContent);
            file.Close();

            Debug.Log("Images data saved");
            m_Saving = false;
            m_SavingThread.Abort();
        }
        #endregion
        #endregion
    }
}
