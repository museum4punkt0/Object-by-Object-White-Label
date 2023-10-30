using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Wezit;

public class ARManager : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private ARTrackedImageManager _aRTrackedImageManager = null;
    [SerializeField] private ARItemRoot _arRootPrefab = null;
    [SerializeField] private Transform _arRootRoot;
    [SerializeField] private Camera _arCamera = null;
    [SerializeField] private Light _light = null;
    #endregion
    #region Private
    private Poi m_PoiData;
    private ARItemRoot m_ItemRootInstance;

    private float m_QRCodeAngle;
    #endregion
    #endregion

    #region Properties
    public UnityEvent ImageFound = new UnityEvent();
    public UnityEvent<Poi> ARItemClicked = new UnityEvent<Poi>();
    #endregion

    #region Methods
    #region Monobehaviour
    private void OnEnable()
    {
        _aRTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
        _arCamera.enabled = true;
        _light.enabled = true;
    }

    private void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
        _arCamera.enabled = false;
        _light.enabled = false;
    }
    #endregion
    #region Public
    public async void Inflate(Poi poi)
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
        _aRTrackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
        _arCamera.enabled = true;
        _light.enabled = true;

        if (m_ItemRootInstance)
        {
            Destroy(m_ItemRootInstance.gameObject);
        }
        m_PoiData = poi;

        if (m_PoiData != null)
        {
            await m_PoiData.GetRelations(m_PoiData);
            foreach (Relation relation in m_PoiData.Relations)
            {
                if (relation.relation == RelationName.REF_PICTURE)
                {
                    RuntimeReferenceImageLibrary library = _aRTrackedImageManager.CreateRuntimeLibrary();
                    _aRTrackedImageManager.referenceLibrary = library;
                    _aRTrackedImageManager.enabled = true;
                    StartCoroutine(SpriteUtils.GetTextureFromSource(relation.GetAssetSourceByTransformation(WezitSourceTransformation.default_base), OnQRCodeDownloaded, ""));
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("ARManager - POI data is null");
        }
    }

    public void Reset()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
        if (m_ItemRootInstance)
        {
            Destroy(m_ItemRootInstance.gameObject);
        }
        _arCamera.enabled = false;
        _light.enabled = false;

    }

    public void ToggleItemsRoot(bool isOn)
    {
        if(m_ItemRootInstance != null)
        {
            m_ItemRootInstance.Toggle(isOn);
        }
    }
    #endregion
    #region Private
    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs trackedImageArgs)
    {
        foreach (ARTrackedImage trackedImage in trackedImageArgs.added)
        {
            ImageFound?.Invoke();
            m_ItemRootInstance = Instantiate(_arRootPrefab, _arRootRoot);
            m_ItemRootInstance.transform.position = trackedImage.transform.position;
            m_ItemRootInstance.Inflate(m_PoiData, _arCamera);
            m_ItemRootInstance.ARItemClicked.AddListener(OnARItemClicked);
        }
    }

    private void OnQRCodeDownloaded(Texture2D texture2D)
    {
        float.TryParse(m_PoiData.extent,
                       System.Globalization.NumberStyles.AllowDecimalPoint,
                       new System.Globalization.CultureInfo("en-US"),
                       out float size);
        float.TryParse(m_PoiData.location,
                           System.Globalization.NumberStyles.AllowDecimalPoint,
                           new System.Globalization.CultureInfo("en-US"),
                           out m_QRCodeAngle);
#if UNITY_IOS
        size = size == 0 ? 0.015f : size;
#endif
        StartCoroutine(AddImage(texture2D, m_PoiData.title, size));
    }

    private IEnumerator AddImage(Texture2D imageToAdd, string title, float size)
    {
        yield return null;
        var library = _aRTrackedImageManager.referenceLibrary;

        if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
                mutableLibrary.ScheduleAddImageWithValidationJob(
                imageToAdd,
                title,
                size == 0 ? null : size /* in meters */);
        }
    }

    private void OnARItemClicked(Poi poi)
    {
        ARItemClicked?.Invoke(poi);
    }
#endregion
#endregion
}
