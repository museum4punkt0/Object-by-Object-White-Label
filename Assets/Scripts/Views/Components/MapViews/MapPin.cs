using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;

public class MapPin : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private int _referenceHeight = 80;
    [SerializeField] private float _referenceScale = 0.6f;
    [SerializeField] private Material _pinMaterial = null;
    #endregion
    #region Private
    private Tour m_Tour;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Tour> GlobalMapPinClicked = new UnityEvent<Tour>();
    #endregion

    #region Methods
    #region Monobehaviour
    private void OnMouseDown()
    {
        if(m_Tour != null) GlobalMapPinClicked?.Invoke(m_Tour);
    }

    private void OnEnable()
    {
        if(_spriteRenderer.sprite == null)
        {
            Utils.ImageUtils.LoadImage(_spriteRenderer, this, m_Tour, _referenceHeight, _referenceScale, Wezit.RelationName.REF_PICTURE);
        }
        StartCoroutine(WaitAndResize());
    }
    #endregion
    #region Public
    public void Inflate(Tour tour)
    {
        m_Tour = tour;
        name = tour.title;
        Utils.ImageUtils.LoadImage(_spriteRenderer, this, tour, _referenceHeight, _referenceScale, Wezit.RelationName.REF_PICTURE);
        _pinMaterial.color = GlobalSettingsManager.Instance.AppColor;
        _pinMaterial.mainTexture = GlobalSettingsManager.Instance.GlobalPinTexture;
    }
    #endregion
    #region Private
    private IEnumerator WaitAndResize()
    {
        yield return null;
        Vector3 localScale = transform.localScale;
        transform.localScale = new Vector3(localScale.x, 1f, localScale.z);
    }
    #endregion
    #endregion
}
