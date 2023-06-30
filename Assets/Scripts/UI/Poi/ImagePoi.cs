using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Wezit;
using Utils;
using UniRx;
using UniRx.Async;
using System;

public class ImagePoi : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private TextMeshProUGUI _legend = null;
    [SerializeField] private Image _image = null;
    [SerializeField] private RectTransform _pinchableScrollRectContent = null;
    [Header("Preview")]
    [SerializeField] private Image _preview = null;
    [SerializeField] private Image _frame = null;
    [SerializeField] private Image _grayImage = null;
    [SerializeField] private Image _colorImage = null;
    [SerializeField] private Image _previewImage = null;
    #endregion
    #region Private
    private Poi m_Poi = null;

    private float m_ImageHeight = 0;
    private RectTransform m_PreviewRect = null;
    private RectTransform m_PreviewImageRect = null;
    private RectTransform m_ImageRect = null;

    private float m_PreviousScale = 1;
    private Vector2 m_PreviousPosition = Vector2.one;

    private bool m_Loaded = false;
    #endregion
    #endregion

    #region Properties
    public UnityEvent PoiCloseButtonClicked = new UnityEvent();
    public UnityEvent PoiLoaded = new UnityEvent();
    #endregion

    #region Methods
    #region Public
    public async void Inflate(Poi a_poi)
    {
        m_Loaded = false;

        m_Poi = a_poi;
        _closeButton.onClick.AddListener(OnCloseButton);
        _legend.text = StringUtils.CleanFromWezit(m_Poi.subject);

        await m_Poi.GetRelations(m_Poi);
        foreach (Relation relation in m_Poi.Relations)
        {
            if (relation.relation == RelationName.SHOW_PICTURE)
            {
                Wezit.AssetInfo assetInfo = relation.GetAssetByTransformation(WezitSourceTransformation.default_base);
                await StartCoroutine(ImageUtils.SetImage(_image, assetInfo.GetSource(), assetInfo.GetMimeType(), false));
                assetInfo = relation.GetAssetByTransformation(WezitSourceTransformation.default2kpx);
                await StartCoroutine(ImageUtils.SetImage(_colorImage, assetInfo.GetSource(), assetInfo.GetMimeType(), false));
                _legend.text = StringUtils.CleanFromWezit(relation.subject);
            }
        }
        _preview.sprite = _grayImage.sprite = _previewImage.sprite = _frame.sprite = _colorImage.sprite;
        _grayImage.gameObject.SetActive(false);
        _grayImage.gameObject.SetActive(true);
        _preview.gameObject.SetActive(false);
        _preview.gameObject.SetActive(true);

        m_ImageRect = _image.GetComponent<RectTransform>();

        m_PreviewImageRect = _colorImage.GetComponent<RectTransform>();
        m_ImageHeight = m_PreviewImageRect.sizeDelta.y;

        m_PreviewRect = _previewImage.GetComponent<RectTransform>();
        m_PreviousScale = _pinchableScrollRectContent.localScale.x;
        m_PreviousPosition = _pinchableScrollRectContent.anchoredPosition;
        m_PreviewRect.sizeDelta = new Vector2(m_PreviewRect.sizeDelta.x, m_ImageHeight) / m_PreviousScale;
        m_PreviewRect.anchoredPosition = Vector2.zero;

        m_Loaded = true;
        PoiLoaded?.Invoke();
    }

    private void Update()
    {
        if (!m_Loaded) return;
        if(_pinchableScrollRectContent.localScale.x != m_PreviousScale)
        {
            float zoom = _pinchableScrollRectContent.localScale.x;
            m_PreviousScale = zoom;

            m_PreviewRect.sizeDelta = new Vector2(m_PreviewRect.sizeDelta.x, m_ImageHeight) / zoom;
            //m_PreviewRect.pivot = Vector2.one - _pinchableScrollRect.content.pivot;
        }
        if(m_PreviousPosition != _pinchableScrollRectContent.anchoredPosition)
        {
            m_PreviousPosition = _pinchableScrollRectContent.anchoredPosition;
            ComputePreviewPosition(m_PreviousScale);
        }
        //m_PreviewRect.localPosition = new Vector3(_pinchableScrollRect.content.localPosition.x / m_PreviewRect.sizeDelta.x, _pinchableScrollRect.content.localPosition.y / m_PreviewRect.sizeDelta.y, 0);
    }
    #endregion
    #region Private
    private void ComputePreviewPosition(float zoom)
    {
        Debug.LogError("Yoyoyo");
        float alphax = (_pinchableScrollRectContent.anchoredPosition.x + 0.5f * 1920 - _pinchableScrollRectContent.pivot.x * m_ImageRect.sizeDelta.x * zoom) / (1920 - m_ImageRect.sizeDelta.x * zoom);
        float alphay = (_pinchableScrollRectContent.anchoredPosition.y + 0.5f * 1080 - _pinchableScrollRectContent.pivot.y * m_ImageRect.sizeDelta.y * zoom) / (1080 - m_ImageRect.sizeDelta.y * zoom);

        float previewx = (m_PreviewImageRect.sizeDelta.x - m_PreviewRect.sizeDelta.x) * (alphax - 0.5f);
        float previewy = (m_PreviewImageRect.sizeDelta.y - m_PreviewRect.sizeDelta.y) * (alphay - 0.5f);
        m_PreviewRect.anchoredPosition = new Vector2(previewx, previewy);
    }

    private void OnCloseButton()
    {
        PoiCloseButtonClicked?.Invoke();
        Destroy(gameObject);
    }
    #endregion
    #endregion
}
