using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ARItemBase : MonoBehaviour
{
    #region Fields
    #region SerializeField
    #endregion
    #region Protected
    protected Wezit.Poi m_Poi;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Wezit.Poi> ARItemClicked = new();
    #endregion

    #region Methods
    #region Monobehaviour
    #endregion
    #region Public
    public void Inflate(Wezit.Poi poi)
    {
        m_Poi = poi;
        name = m_Poi.title;
        if (!string.IsNullOrEmpty(poi.location))
        {
            transform.localPosition = StringUtils.StringToVector3(StringUtils.CleanFromWezit(poi.location));
        }
        if (!string.IsNullOrEmpty(poi.spatial))
        {
            transform.localEulerAngles = StringUtils.StringToVector3(StringUtils.CleanFromWezit(poi.spatial));
        }
    }
    #endregion
    #region Private
    #endregion
    #endregion
}
