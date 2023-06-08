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
    [SerializeField] private Camera _arCamera = null;
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
    }

    private void OnDisable()
    {
        _aRTrackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }
    #endregion
    #region Public
    public async void Inflate(Poi poi)
    {
        if(m_ItemRootInstance)
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
                    StartCoroutine(SpriteUtils.GetTextureFromSource(relation.GetAssetSourceByTransformation(WezitSourceTransformation.original), OnQRCodeDownloaded, ""));
                    break;
                }
            }
        }
    }
    #endregion
    #region Private
    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs trackedImageArgs)
    {
        foreach (ARTrackedImage trackedImage in trackedImageArgs.added)
        {
            ImageFound?.Invoke();
            m_ItemRootInstance = Instantiate(_arRootPrefab, trackedImage.transform);
            m_ItemRootInstance.transform.Rotate(m_QRCodeAngle, 0, 0);
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
        float.TryParse(m_PoiData.spatial,
                           System.Globalization.NumberStyles.AllowDecimalPoint,
                           new System.Globalization.CultureInfo("en-US"),
                           out m_QRCodeAngle);
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
