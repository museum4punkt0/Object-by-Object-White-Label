using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class TourMapView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private OnlineMaps _map;
	[SerializeField] private TourMapPin _tourMapPinPrefab;
	[SerializeField] private GameObject _listVerticalRoot;
	[SerializeField] private MapListVertical _mapListVertical;
	[SerializeField] private GameObject _listHorizontalRoot;
	[SerializeField] private MapListHorizontal _mapListHorizontal;
	[SerializeField] private MapListToggle _mapListToggle;
	[SerializeField] private Button _centerButton;
	[SerializeField] private Button _zoomInButton = null;
	[SerializeField] private Button _zoomOutButton = null;
	[SerializeField] private QRCodeNotification _QRCodeNotification;
	[SerializeField] private SecretPoiButton _secretPoiButton;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Tour m_Tour;
	private OnlineMapsLocationService m_LocationService;
	private Vector3 m_CenteredPostion;
	private List<GameObject> m_Pins = new List<GameObject>();
	private Dictionary<Wezit.PoiLocation, Wezit.Poi> m_PoisAndLocations = new Dictionary<Wezit.PoiLocation, Wezit.Poi>();

	// Dictionary to quickly get the pin corresponding to the selected Poi
	private Dictionary<Wezit.Poi, TourMapPin> m_PoisAndPins = new Dictionary<Wezit.Poi, TourMapPin>();
	private TourMapPin m_PreviousHighlightedPin;
	private TourMapPin m_CurrentHighlightedPin;

	private Wezit.Poi m_LastPoiInrange;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.TOUR_MAP;
	}

	public override void InitView()
	{
		ResetViewContent();
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		base.ShowView();
		InitViewContentByLang(ViewManager.Instance.CurrentLanguage);
		AddListeners();
	}

	public override void HideView()
	{
		RemoveListeners();
		ResetViewContent();
		base.HideView();
	}

	public override void PrepareHideView()
	{
		RemoveListeners();
		base.PrepareHideView();
	}

	public override void OnLanguageUpdated(Language language)
	{
		if (m_IsActive && AppManager.Instance.loadingOver)
		{
			InitViewContentByLang(language);
		}
	}

	public void Dispose()
	{
		RemoveListeners();
	}
	#endregion Public

	#region Private
	private void InitViewContentByLang(Language language)
	{
		ResetViewContent();

		m_Tour = StoreAccessor.State.SelectedTour;

		bool isChallenge = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).IsChallengeMode;
		if(isChallenge)
        {
			ScoreDisplay.Instance.UpdateScore();
        }
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventory : MenuManager.MenuStatus.BackButton;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.GLOBAL_MAP);

		OnlineMapsMarker3DManager onlineMapsMarker3Ds = _map.GetComponent<OnlineMapsMarker3DManager>();
		m_LocationService = _map.GetComponent<OnlineMapsLocationService>();
		m_LocationService.OnLocationChanged = OnLocationChanged;
		List<Vector2> poiLocations = new List<Vector2>();
		List<Wezit.Poi> locatedPois = new List<Wezit.Poi>();

		foreach(Wezit.Poi poi in m_Tour.childs)
        {
			Wezit.PoiLocation poiLocation = PoiLocationStore.GetPoiLocationById(poi.pid);
			if (poiLocation != null)
            {
				poiLocations.Add(new Vector2(poiLocation.lng, poiLocation.lat));
				locatedPois.Add(poi);
				m_PoisAndLocations.Add(poiLocation, poi);


				OnlineMapsMarker3D marker3D = onlineMapsMarker3Ds.Create(poiLocation.lng, poiLocation.lat, _tourMapPinPrefab.gameObject);
				TourMapPin tourMapPinInstance = marker3D.instance.GetComponent<TourMapPin>();
				m_Pins.Add(marker3D.instance);
				tourMapPinInstance.Inflate(poi);
				tourMapPinInstance.TourMapPinClicked.AddListener(OnMapPinClicked);
				m_PoisAndPins.Add(poi, tourMapPinInstance);
			}
			else if(poi.type == "secret")
            {
				_secretPoiButton.Inflate(poi);
            }
			else if(poi.type == "bank")
            {
				StoreAccessor.State.SelectedTourBank = poi;
            }
        }
        _mapListVertical.Inflate(locatedPois, this);
        _mapListHorizontal.Inflate(locatedPois, this);

		m_CenteredPostion = MapUtils.CenterMapOnPoints(poiLocations);
		_map.SetPositionAndZoom(m_CenteredPostion.x, m_CenteredPostion.y, m_CenteredPostion.z);
	}

	private void ResetViewContent()
	{
		_listHorizontalRoot.SetActive(true);
		_mapListHorizontal.ResetView();
		_listVerticalRoot.SetActive(false);
		_QRCodeNotification.gameObject.SetActive(false);

		foreach (GameObject mapPin in m_Pins)
		{
			if (mapPin != null) Destroy(mapPin);
		}
		OnlineMapsMarker3DManager onlineMapsMarker3Ds = _map.GetComponent<OnlineMapsMarker3DManager>();
		onlineMapsMarker3Ds.RemoveAll();
		m_Pins.Clear();
		m_PoisAndLocations.Clear();
		m_PoisAndPins.Clear();
	}

	private void AddListeners()
	{
		RemoveListeners();
		_mapListToggle.ToggleValueChanged.AddListener(OnMapListToggle);
		_centerButton.onClick.AddListener(CenterOnUser);
		_zoomInButton.onClick.AddListener(OnZoomIn);
		_zoomOutButton.onClick.AddListener(OnZoomOut);

		_QRCodeNotification.StartButtonClicked.AddListener(OnQRCodeStart);

		_mapListVertical.ItemClickedPoi.AddListener(OnPoiClicked);
		_mapListHorizontal.ItemSelectedPoi.AddListener(OnItemSelected);
		_mapListHorizontal.ItemClickedPoi.AddListener(OnPoiClicked);

		_secretPoiButton.SecretPoiClicked.AddListener(OnSecretPoiClicked);
	}

	private void RemoveListeners()
	{
		_mapListToggle.ToggleValueChanged.RemoveAllListeners();
		_centerButton.onClick.RemoveAllListeners();
		_zoomInButton.onClick.RemoveAllListeners();
		_zoomOutButton.onClick.RemoveAllListeners();

		_QRCodeNotification.StartButtonClicked.RemoveAllListeners();

		_mapListVertical.ItemClickedPoi.RemoveAllListeners();
		_mapListHorizontal.ItemSelectedPoi.RemoveAllListeners();
		_mapListHorizontal.ItemClickedPoi.RemoveAllListeners();

		_secretPoiButton.SecretPoiClicked.RemoveAllListeners();
	}

	private void OnLocationChanged(Vector2 location)
    {
		Wezit.Poi poiInRange = CheckPoisInRange(location);
		if(m_LastPoiInrange != poiInRange)
        {
			_QRCodeNotification.Inflate();
			m_LastPoiInrange = poiInRange;
        }
    }

	// Map management
	private Wezit.Poi CheckPoisInRange(Vector2 location)
    {
		Wezit.Poi result = null;
		foreach(Wezit.PoiLocation poiLocation in m_PoisAndLocations.Keys)
        {
			if(IsInRange(location, poiLocation).isInRange)
            {
				result = m_PoisAndLocations[poiLocation];
            }
        }
		return result;
    }

	private (bool isInRange, float distance) IsInRange(Vector2 location, Wezit.PoiLocation poiLocation)
    {
		float distance = MapUtils.CalculateDistance(location, new Vector2(poiLocation.lng, poiLocation.lat));
		return ((distance < poiLocation.radius, distance));
    }

	private void CenterOnUser()
    {
		if(m_LocationService.IsLocationServiceRunning())
        {
			m_LocationService.GetLocation(out float lng, out float lat);
			_map.SetPosition(lng, lat);
        }
		else
        {
			_map.SetPositionAndZoom(m_CenteredPostion.x, m_CenteredPostion.y, m_CenteredPostion.z);
        }
	}

	private void OnZoomIn()
	{
		ZoomInOut(true);
	}

	private void OnZoomOut()
	{
		ZoomInOut(false);
	}

	private void ZoomInOut(bool isIn)
	{
		_map.zoom = _map.zoom + (isIn ? 1 : -1);
	}

	private void OnMapPinClicked(Wezit.Poi poi)
	{
		_mapListHorizontal.SelectPoi(poi.pid);
	}

	// Lists management
	private void OnPoiClicked(Wezit.Poi poi)
	{
		StoreAccessor.State.SelectedPoi = poi;
		PlayerManager.Instance.CurrentPoi = poi;
        AppManager.Instance.GoToState(KioskState.POI_INTRO);
    }

	private void OnMapListToggle(bool isOn)
    {
		_listHorizontalRoot.SetActive(isOn);
		_listVerticalRoot.SetActive(!isOn);
    }

	private void OnQRCodeStart()
    {
		AppManager.Instance.SelectPoi(m_LastPoiInrange);
		AppManager.Instance.GoToState(KioskState.AR);
	}

	private void OnItemSelected(Vector2 geolocation, Wezit.Poi poi)
	{
		_map.SetPosition(geolocation.x, geolocation.y);
		TourMapPin selectedPin = m_PoisAndPins[poi];
		if (m_CurrentHighlightedPin != null && m_CurrentHighlightedPin != selectedPin)
		{
			m_PreviousHighlightedPin = m_CurrentHighlightedPin;
			m_PreviousHighlightedPin.Highlight(false);
		}
		m_CurrentHighlightedPin = selectedPin;
		m_CurrentHighlightedPin.Highlight(true);
	}

	private void OnSecretPoiClicked(Wezit.Poi poi)
    {
		AppManager.Instance.SelectPoi(poi);
		AppManager.Instance.GoToState(KioskState.SECRET_POI);
    }
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}