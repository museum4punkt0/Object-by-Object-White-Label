using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Wezit;
using TMPro;

public class MapListItem : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private Button _button;
    [SerializeField] private RawImage _image = null;
    [SerializeField] private LayoutElement _imageMask = null;
    [SerializeField] private CompletionBars _completion = null;
    [SerializeField] private GameObject _textContainer = null;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    #endregion
    #region Private
    private List<GameObject> m_Items = new List<GameObject>();
    private Tour m_Tour;
    private Poi m_Poi;
    private Vector2 m_Geolocation;
    private bool m_isVertical;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Tour> ItemClickedTour = new UnityEvent<Tour>();
    public UnityEvent<Poi> ItemClickedPoi = new UnityEvent<Poi>();
    public Vector2 Geolocation
    {
        get
        {
            return m_Geolocation;
        }
    }

    public string pid
    {
        get
        {
            return m_Tour == null ? (m_Poi == null ? "" : m_Poi.pid) : m_Tour.pid;
        }
    }

    public Poi Poi
    {
        get
        {
            return m_Poi;
        }
    }
    #endregion

    #region Methods
    #region Monobehaviour
    private void OnEnable()
    {
        if (m_isVertical)
        {
            StartCoroutine(SquarifyImage());
        }
    }
    #endregion

    #region Public
    public void Inflate(Tour tour, MonoBehaviour activeMonobehavour, bool isVertical = false)
    {
        ResetView();
        SetProgression(PlayerManager.Instance.Player.GetTourProgression(tour.pid), tour);

        _title.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = tour.title;
        _description.text = tour.subject;
        if (_textContainer)
        {
            activeMonobehavour.StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));
        }

        Utils.ImageUtils.LoadImage(_image, activeMonobehavour, tour);
        m_isVertical = isVertical;

        m_Tour = tour;
        Wezit.PoiLocation poiLocation = PoiLocationStore.GetPoiLocationById(tour.pid);
        if (poiLocation != null)
        {
            m_Geolocation = new Vector2(poiLocation.lng, poiLocation.lat);
        }
        _button.onClick.AddListener(OnButtonClickTour);
    }

    public void Inflate(Poi poi, MonoBehaviour activeMonobehavour, bool isVertical = false)
    {
        ResetView();

        SetProgression(PlayerManager.Instance.Player.GetTourProgression(PlayerManager.Instance.CurrentTour.pid).GetPoiProgression(poi.pid), poi);

        _title.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = poi.title;
        _description.text = poi.subject;

        if (_textContainer)
        {
            activeMonobehavour.StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));
        }

        Utils.ImageUtils.LoadImage(_image, activeMonobehavour, poi, RelationName.SHOW_PICTURE, WezitSourceTransformation.default_base, false);
        m_isVertical = isVertical;

        m_Poi = poi;
        Wezit.PoiLocation poiLocation = PoiLocationStore.GetPoiLocationById(poi.pid);
        if (poiLocation != null)
        {
            m_Geolocation = new Vector2(poiLocation.lng, poiLocation.lat);
        }
        _button.onClick.AddListener(OnButtonClickPoi);
    }
    #endregion
    #region Private
    private void ResetView()
    {
        foreach(GameObject item in m_Items)
        {
            Destroy(item);
        }
        m_Items.Clear();
    }

    private void SetProgression(TourProgressionData tourProgressionData, Tour tour)
    {
        if(tourProgressionData.MaxProgression != tour.childs.Count - 2)
        {
            tourProgressionData.SetTourMaxProgression(tour.childs.Count - 2);
        }
        _completion.Inflate(tourProgressionData.MaxProgression, tourProgressionData.GetTourCurrentProgression());
    }

    private void SetProgression(PoiProgressionData poiProgressionData, Poi poi)
    {
        if (poiProgressionData.GetPoiMaxProgression() != poi.childs.Count)
        {
            poiProgressionData.SetPoiMaxProgression(poi.childs.Count);
        }
        _completion.Inflate(poi.childs.Count, poiProgressionData.GetPoiCurrentProgression());
    }

    private void OnButtonClickTour()
    {
        ItemClickedTour?.Invoke(m_Tour);
    }

    private void OnButtonClickPoi()
    {
        ItemClickedPoi?.Invoke(m_Poi);
    }

    private IEnumerator SquarifyImage()
    {
        yield return null;
        _imageMask.minHeight = GetComponent<RectTransform>().sizeDelta.x;
    }
    #endregion
    #endregion
}
