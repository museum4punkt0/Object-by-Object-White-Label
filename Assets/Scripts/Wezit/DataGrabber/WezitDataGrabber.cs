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

        public async UniTask GetAssetsForTour(Tour tour, string transformation = "all")
        {
            m_StopDownload = false;
            Load();
            if (m_Assets.Count == 0)
            {
                if (AssetsLoader.Assets.Count == 0)
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
                if (!asset.use.Contains(tour.pid)) continue;
                if (m_StopDownload) return;
                bool hasTransformation = false;
                foreach (WezitAssets.File file in asset.files)
                {
                    if ((file.label == transformation) || (transformation == "all"))
                    {
                        hasTransformation = true;
                        int downloaded = file.size;
                        if (CheckDownloadNecessity(file)) downloaded = await DownloadImage(file);
                        currentDownloaded += downloaded;
                        DownloadProgress?.Invoke(currentDownloaded);
                        currentCounter++;
                    }
                }
                if (!hasTransformation)
                {
                    WezitAssets.File file = asset.files.Find(x => x.label == "original");
                    int downloaded = file.size;
                    if (CheckDownloadNecessity(file)) downloaded = await DownloadImage(file);
                    currentDownloaded += downloaded;
                    DownloadProgress?.Invoke(currentDownloaded);
                    currentCounter++;
                }
            }
            HasDownloaded = true;
            Save();
            Wezit.FilesDownloader.SqliteUpdated = false;
            DownloadOver?.Invoke();
        }

        public async UniTask GetAllAssets(string transformation)
        {
            m_StopDownload = false;
            Load();
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
                bool hasTransformation = false;
                foreach(WezitAssets.File file in asset.files)
                {
                    if ((file.label == transformation) || (transformation == "all"))
                    {
                        hasTransformation = true;
                        int downloaded = file.size;
                        if (CheckDownloadNecessity(file)) downloaded = await DownloadImage(file);
                        currentDownloaded += downloaded;
                        DownloadProgress?.Invoke(currentDownloaded);
                        currentCounter++;
                    }
                }
                if(!hasTransformation)
                {
                    if (asset.files.Count == 0) continue;
                    WezitAssets.File file = asset.files.Find(x => x.label == "original");
                    int downloaded = file.size;
                    if (CheckDownloadNecessity(file)) downloaded = await DownloadImage(file);
                    currentDownloaded += downloaded;
                    DownloadProgress?.Invoke(currentDownloaded);
                    currentCounter++;
                }
            }
            HasDownloaded = true;
            Save();
            Wezit.FilesDownloader.SqliteUpdated = false;
            DownloadOver?.Invoke();
        }

        public int GetDownloadSize(string transformation = "all")
        {
            int downloadSize = 0;
            int counter = m_Assets.Count;
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

        public int GetUpdateSize()
        {
            List<WezitAssets.File> filesToUpdate = new List<WezitAssets.File>();
            foreach (WezitAssets.Asset asset in m_Assets)
            {
                foreach(WezitAssets.File file in asset.files)
                {
                    ImageAndMd5 imageMd5 = DownloadedImagesMd5Dict.Find(x => x.path == file.path);
                    if (imageMd5 != null)
                    {
                        if (imageMd5.md5 != file.md5)
                        {
                            Debug.Log("There is a file to update \n" + asset.ToString());
                            filesToUpdate.Add(file);
                        }
                    }
                    else
                    {
                        filesToUpdate.Add(file);
                        Debug.Log("There is a new file \n" + asset.ToString());
                    }
                }
            }
            int downloadSize = 0;
            int counter = filesToUpdate.Count;
            downloadSize = filesToUpdate.Sum(x => x.size);
            return downloadSize;
        }

        public bool CheckDownloadNecessity(WezitAssets.File file)
        {
            ImageAndMd5 imageMd5 = DownloadedImagesMd5Dict.Find(x => x.path == file.path);
            if (imageMd5 != null)
            {
                if (imageMd5.md5 != file.md5)
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return !File.Exists(Path.Combine(ImagesFolderPath, file.path));
            }
        }

        public async UniTask<int> DownloadImage(WezitAssets.File file)
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
                File.WriteAllText(m_FilePath, String.Empty);
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
