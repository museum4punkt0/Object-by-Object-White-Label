using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PoiProgressionData
{
    [FormerlySerializedAs("Id")]
    [SerializeField]
    public string Id = "";

    [FormerlySerializedAs("ContentsProgression")]
    [SerializeField]
    public List<ContentProgressionData> ContentsProgression = new List<ContentProgressionData>();

    [FormerlySerializedAs("QuizCompleted")]
    public bool QuizCompleted = false;

    #region Public API
    private bool m_HasBeenVisited = false;
    private int m_MaxProgression = 0;

    public bool HasBeenVisited
    {
        get
        {
            if(m_HasBeenVisited) return true;
            if (ContentsProgression.Count == 0)
                return false;

            foreach(ContentProgressionData contentProgression in ContentsProgression)
            {
                if (contentProgression.HasBeenVisited)
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
            return GetPoiCurrentProgression() >= GetPoiMaxProgression();
        }
    }

    public ContentProgressionData GetContentProgression(string a_pid)
    {
        ContentProgressionData progression = ContentsProgression.Find(x => x.Id == a_pid);
        if (progression == null)
        {
            progression = new ContentProgressionData()
            {
                Id = a_pid,
            };
            ContentsProgression.Add(progression);
        }
        return progression;
    }

    public int GetPoiCurrentProgression()
    {
        int count = 0;

        foreach (ContentProgressionData contentProgression in ContentsProgression)
        {
            if(contentProgression.State == EContentProgressionState.Complete)
            {
                count++;
            }
        }

        return count;
    }

    public int GetPoiMaxProgression()
    {
        if (m_MaxProgression != 0) return m_MaxProgression;
        else
        {
            m_MaxProgression = ContentsProgression.Count;
            return m_MaxProgression;
        }
        return ContentsProgression.Count;
    }

    public void SetPoiMaxProgression(int contentCount)
    {
        m_MaxProgression = contentCount;
        PlayerManager.Instance.Player.Save();
    }

    public float GetPoiCurrentProgressionPercent()
    {
        return GetPoiCurrentProgression() / (float)GetPoiMaxProgression() * 100f;
    }


    #endregion
}