using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using Wezit;

[Serializable]
public class PlayerData
{
    #region Saved data
    [FormerlySerializedAs("ToursProgression")]
    [SerializeField]
    public List<TourProgressionData> ToursProgression = new List<TourProgressionData>();

    [FormerlySerializedAs("Language")]
    [SerializeField]
    public string Language = "de";

    [FormerlySerializedAs("HasAcceptedRGPD")]
    [SerializeField]
    public bool HasAcceptedRGPD = false;
    #endregion

    #region Public API
    public TourProgressionData GetTourProgression(string a_tourId)
    {
        TourProgressionData progression = ToursProgression.Find(x => x.Id.CompareTo(a_tourId) == 0);
        if(progression == null)
        {
            progression = new TourProgressionData()
            {
                Id = a_tourId,
            };
            ToursProgression.Add(progression);
        }

        return progression;
    }
    public void SetContentProgression(string a_tourId, string a_poiId, string a_contentId, EContentProgressionState a_progressState)
    {
        if(GetTourProgression(a_tourId).GetPoiProgression(a_poiId).GetContentProgression(a_contentId).State == EContentProgressionState.Complete) return;
        GetTourProgression(a_tourId).GetPoiProgression(a_poiId).GetContentProgression(a_contentId).State = a_progressState;
    }

    public void SetPoiProgression(string a_tourId, string a_poiId)
    {
        if (GetTourProgression(a_tourId).GetPoiProgression(a_poiId).HasBeenVisited) return;
        GetTourProgression(a_tourId).GetPoiProgression(a_poiId).HasBeenVisited = true;
        Save();
    }
    #endregion

    #region Save/Load behavior
    const string CONST_FILE_NAME = "user.dat";

    #region Fields
    [NonSerialized]
    private string m_FileContent;
    [NonSerialized]
    private string m_FilePath;
    [NonSerialized]
    private Thread m_SavingThread;
    [NonSerialized]
    private bool m_Saving;
    #endregion

    private string FilePath
    {
        get
        {
            return Path.Combine(PlayerManager.PlayerDataPath, CONST_FILE_NAME);
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
            string data = (string) bf.Deserialize(file);
            file.Close();

            JsonUtility.FromJsonOverwrite(data, this);
        }
        else
        {
            Debug.LogError("No file");
        }
    }

    public void Save()
    {
        m_FileContent = JsonUtility.ToJson(this, false);
        m_SavingThread = new Thread(SaveData);
        m_FilePath = FilePath;
        if(!m_Saving) m_SavingThread.Start();
    }

    private void SaveData()
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

        m_Saving = false;
        m_SavingThread.Abort();
    }
    #endregion
}
