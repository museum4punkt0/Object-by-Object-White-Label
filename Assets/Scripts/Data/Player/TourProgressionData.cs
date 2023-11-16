using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TourProgressionData
{
    [FormerlySerializedAs("Id")]
    [SerializeField]
    public string Id = "";

    [FormerlySerializedAs("MaxProgression")]
    [SerializeField]
    public int MaxProgression = 0;

    [FormerlySerializedAs("NatureProgression")]
    [SerializeField]
    public List<PoiProgressionData> PoisProgression = new List<PoiProgressionData>();
    [FormerlySerializedAs("HasBeenVisited")]
    [SerializeField]
    private bool m_HasBeenVisited = false;
    [FormerlySerializedAs("HasBeenCompleted")]
    [SerializeField]
    private bool m_HasBeenCompleted = false;
    [FormerlySerializedAs("IsModeSet")]
    [SerializeField]
    public bool IsModeSet = false;
    [FormerlySerializedAs("IsChallengeMode")]
    [SerializeField]
    public bool IsChallengeMode = false;
    [FormerlySerializedAs("TourScore")]
    [SerializeField]
    private int m_TourScore = 0;
    [FormerlySerializedAs("TourScratchImagePath")]
    [SerializeField]
    private string m_TourScratchImagePath = "";
    [FormerlySerializedAs("TourSelfiePath")]
    [SerializeField]
    private string m_TourSelfiePath = "";
    [FormerlySerializedAs("HasBeenDownloaded")]
    [SerializeField] public bool HasBeenDownloaded = false;
    [FormerlySerializedAs("SecretUnlockPopinShown")]
    [SerializeField] public bool SecretUnlockPopinShown = false;

    #region Public API
    public bool HasBeenVisited
    {
        get
        {
            if(m_HasBeenVisited) return true;
            if (PoisProgression.Count == 0)
                return false;

            foreach (PoiProgressionData natureProgression in PoisProgression)
            {
                if (natureProgression.HasBeenVisited)
                {
                    m_HasBeenVisited = true;
                    return true;
                }
            }

            return false;
        }
        set
        {
            m_HasBeenVisited = value;
        }
    }

    public bool HasBeenCompleted
    {
        get
        {
            if(m_HasBeenCompleted) return true;
            if(HasBeenVisited && GetTourCurrentProgression() >= GetTourMaxProgression())
            {
                m_HasBeenCompleted = true;
                return true;
            }
            else return false;
        }
    }

    public int TourScore
    {
        get
        {
            return m_TourScore;
        }
        set
        {
            m_TourScore = value;
            ScoreDisplay.Instance.SetScore(m_TourScore);
            PlayerManager.Instance.Player.Save();
        }
    }

    public string TourScratchImagePath
    {
        get
        {
            return m_TourScratchImagePath;
        }
        set
        {
            m_TourScratchImagePath = value;
            PlayerManager.Instance.Player.Save();
        }
    }

    public string TourSelfiePath
    {
        get
        {
            return m_TourSelfiePath;
        }
        set
        {
            m_TourSelfiePath = value;
            PlayerManager.Instance.Player.Save();
        }
    }

    public PoiProgressionData GetPoiProgression(string a_poiId)
    {
        PoiProgressionData progression = PoisProgression.Find(x => x.Id.CompareTo(a_poiId) == 0);
        if (progression == null)
        {
            progression = new PoiProgressionData()
            {
                Id = a_poiId,
            };
            PoisProgression.Add(progression);
        }

        return progression;
    }

    public int GetTourCurrentProgression()
    {
        int value = 0;

        for(int i = 0; i < PoisProgression.Count; i++)
        {
            value += PoisProgression[i].HasBeenVisited ? 1 : 0;
        }

        return value;
    }

    public int GetTourMaxProgression()
    {
        return MaxProgression;
    }

    public void SetTourMaxProgression(int maxProg)
    {
        MaxProgression = maxProg;
        PlayerManager.Instance.Player.Save();
    }

    public int GetTourScore()
    {
        return TourScore;
    }

    public void ResetProgression()
    {
        MaxProgression = 0;
        foreach (PoiProgressionData poiProgression in PoisProgression)
        {
            poiProgression.ResetProgression();
        }
        PoisProgression = new List<PoiProgressionData>();
        m_HasBeenVisited = false;
        m_HasBeenCompleted = false;
        IsModeSet = false;
        IsChallengeMode = false;
        m_TourScore = 0;
        m_TourScratchImagePath = "";
        m_TourSelfiePath = "";
        HasBeenDownloaded = false;
        SecretUnlockPopinShown = false;
    }
    #endregion

}
