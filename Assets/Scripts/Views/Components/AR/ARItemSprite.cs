using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Wezit;

public class ARItemSprite : ARItemBase
{
    #region Fields
    #region SerializeField
    [SerializeField] private SpriteRenderer _spriteRenderer = null;
    [SerializeField] private LookAtCamera _lookAtCamera = null;
    [SerializeField] private int _referenceHeight = 80;
    [SerializeField] private float _referenceScale = 0.6f;
    #endregion
    #region Private
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviour
    private void OnMouseDown()
    {
        StartCoroutine(CheckIfClickingOnUI());
    }
    #endregion
    #region Public
    public void Inflate(Poi poi, Camera arCamera)
    {
        base.Inflate(poi);
        _lookAtCamera.Inflate(arCamera);

        switch(poi.type)
        {
            default:
            case "image":
                _spriteRenderer.sprite = GlobalSettingsManager.Instance.ContentImageSprite;
                break;

            case "audio":
                _spriteRenderer.sprite = GlobalSettingsManager.Instance.ContentAudioSprite;
                break;

            case "video":
                _spriteRenderer.sprite = GlobalSettingsManager.Instance.ContentVideoSprite;
                break;
        }
        _spriteRenderer.transform.localScale = _referenceScale * _referenceHeight / _spriteRenderer.sprite.texture.height * Vector3.one;
    }
    #endregion
    #region Private
    private IEnumerator CheckIfClickingOnUI()
    {
        yield return null;
        if (!ClickManager.ClickingOnUI)
        {
            ARItemClicked?.Invoke(m_Poi);
        }
    }
    #endregion
    #endregion
}
