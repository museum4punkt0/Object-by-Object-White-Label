using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;
using UnityEngine.Android;

public class TourMapView : BaseView
{
    #region Fields
    #region Serialize Fields
    [Header("Map")]
	[SerializeField] private OnlineMaps _map;
	[SerializeField] private TourMapPin _tourMapPinPrefab;
	[SerializeField] private Button _centerButton = null;
	[SerializeField] private Image _centerButtonBG = null;
	[SerializeField] private Button _zoomInButton = null;
	[SerializeField] private Image _zoomInButtonBG = null;
	[SerializeField] private Button _zoomOutButton = null;
	[SerializeField] private Image _zoomOutButtonBG = null;
    [Header("List")]
	[SerializeField] private GameObject _listVerticalRoot;
	[SerializeField] private MapListVertical _mapListVertical;
	[SerializeField] private GameObject _listHorizontalRoot;
	[SerializeField] private MapListHorizontal _mapListHorizontal;
	[SerializeField] private MapListToggle _mapListToggle;
    [Header("Popins")]
	[SerializeField] private QRCodeNotification _QRCodeNotification;
	[SerializeField] private SecretPoiButton _secretPoiButton;
	[SerializeField] private SecretPoiNotification _secretPoiNotification;
	[SerializeField] private SecretUnlockPopin _secretPoiUnlockedPopin;
	[SerializeField] private LocationWarningNotification _locationWarningNotification;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Tour m_Tour;
	private OnlineMapsLocationService m_LocationService;
	private OnlineMapsCache m_cacheService;
	private Vector3 m_CenteredPostion;
	private List<GameObject> m_Pins = new List<GameObject>();
	private Dictionary<Wezit.PoiLocation, Wezit.Poi> m_PoisAndLocations = new Dictionary<Wezit.PoiLocation, Wezit.Poi>();

	// Dictionary to quickly get the pin corresponding to the selected Poi
	private Dictionary<Wezit.Poi, TourMapPin> m_PoisAndPins = new Dictionary<Wezit.Poi, TourMapPin>();
	private TourMapPin m_PreviousHighlightedPin;
	private TourMapPin m_CurrentHighlightedPin;

	private Wezit.Poi m_LastPoiInrange;
	private Wezit.PoiLocation m_tourPoiLocation;

	private string m_mapProviderSettingKey = "template.spk.maps.global.map.provider.url";
	private string m_mapProviderUrl;
	private bool m_hasSeenLocationWarning;
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
	private async void InitViewContentByLang(Language language)
	{
		ResetViewContent();

		m_Tour = StoreAccessor.State.SelectedTour;
		m_tourPoiLocation = PoiLocationStore.GetPoiLocationById(m_Tour.pid);

		PlayerManager.Instance.IsGPSOn = Input.location.isEnabledByUser;
		PlayerManager.Instance.ViewOnInventoryBackButton = KioskState.TOUR_MAP;

		_map.emptyColor = _centerButtonBG.color = _zoomInButtonBG.color = _zoomOutButtonBG.color = GlobalSettingsManager.Instance.AppColor;

		// Manage challenge mode
		TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
		bool isChallenge = tourProgressionData.IsChallengeMode;
		if(isChallenge)
        {
			ScoreDisplay.Instance.UpdateScore();
        }
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.TOUR_INTRO);

		OnlineMapsMarker3DManager onlineMapsMarker3Ds = _map.GetComponent<OnlineMapsMarker3DManager>();
		m_LocationService = _map.GetComponent<OnlineMapsLocationService>();
		m_LocationService.OnLocationChanged = OnLocationChanged;
		List<Vector2> poiLocations = new List<Vector2>();
		List<Wezit.Poi> locatedPois = new List<Wezit.Poi>();

		// Instantiate map markers and look for a map
		Wezit.PoiLocation poiLocation = null;
		string mapSource = "";

		bool hasHighlightedFirstPOI = false;
		foreach(Wezit.Poi poi in m_Tour.childs)
        {
			poiLocation = PoiLocationStore.GetPoiLocationById(poi.pid);
			if (poiLocation != null)
            {
				poiLocations.Add(new Vector2(poiLocation.lng, poiLocation.lat));
				locatedPois.Add(poi);
				m_PoisAndLocations.Add(poiLocation, poi);

				if(string.IsNullOrEmpty(mapSource))
                {
					mapSource = poiLocation.GetMapSourceByTransformation(WezitSourceTransformation.tilesZip).Replace("metadata.json", "");
                }

				OnlineMapsMarker3D marker3D = onlineMapsMarker3Ds.Create(poiLocation.lng, poiLocation.lat, _tourMapPinPrefab.gameObject);
				TourMapPin tourMapPinInstance = marker3D.instance.GetComponent<TourMapPin>();
				m_Pins.Add(marker3D.instance);
				tourMapPinInstance.Inflate(poi);
				tourMapPinInstance.TourMapPinClicked.AddListener(OnMapPinClicked);
				m_PoisAndPins.Add(poi, tourMapPinInstance);

				if(!hasHighlightedFirstPOI)
                {
					OnItemSelected(new Vector2(poiLocation.lng, poiLocation.lat), poi);
				}
			}
			else if(poi.type == "secret")
            {
				_secretPoiButton.Inflate(poi);
				_secretPoiUnlockedPopin.Inflate(poi);
			}
		}

		m_cacheService = _map.GetComponent<OnlineMapsCache>();
		m_cacheService.useFileCache = true;
		m_cacheService.fileCacheTilePath = "TileCache/" + m_Tour.pid.Replace(":", "-") + "/{lbs}/{lng}/{zoom}/{x}/{y}";

		// Display map
		if (!string.IsNullOrEmpty(mapSource))
        {
			string mapMetadataJsonString = await FileUtils.RequestTextContent(mapSource + "/metadata.json", 5);
			Wezit.MapMetadata mapMetadata = JsonUtility.FromJson<Wezit.MapMetadata>(mapMetadataJsonString);
			Vector4 bounds = mapMetadata.GetBounds();

			// Add limits to the map
			OnlineMapsLimits limits = _map.GetComponent<OnlineMapsLimits>();
			limits.minLongitude = bounds.x;
			limits.minLatitude = bounds.y;

			limits.maxLongitude = bounds.z;
			limits.maxLatitude = bounds.w;

			limits.minZoom = mapMetadata.minzoom;
			limits.maxZoom = mapMetadata.maxzoom;

			limits.positionRangeType = OnlineMapsPositionRangeType.center;
			limits.usePositionRange = true;
			limits.useZoomRange = true;
			limits.ApplySettings();

			_map.customProviderURL = mapSource + "/{zoom}/{x}/{y}.jpg";
        }
		else
		{
			m_mapProviderUrl = Wezit.Settings.Instance.GetSettingAsCleanedText(m_mapProviderSettingKey);
			if (!string.IsNullOrEmpty(m_mapProviderUrl))
			{
				_map.customProviderURL = m_mapProviderUrl;
			}
			else
			{
				_map.customProviderURL = "https://tiles.stadiamaps.com/styles/stamen_watercolor/{z}/{x}/{y}.jpg";
			}
		}
        _mapListVertical.Inflate(locatedPois, this);
        _mapListHorizontal.Inflate(locatedPois, this);

		m_CenteredPostion = MapUtils.CenterMapOnPoints(poiLocations);
		_map.SetPositionAndZoom(m_CenteredPostion.x, m_CenteredPostion.y, m_CenteredPostion.z + 1);

		// Display secret poi unlock popin if it just got unlocked
		if(tourProgressionData.HasBeenCompleted && !tourProgressionData.SecretUnlockPopinShown)
        {
			_secretPoiUnlockedPopin.Open();
			tourProgressionData.SecretUnlockPopinShown = true;
			PlayerManager.Instance.Player.Save();
        }
	}

	private void ResetViewContent()
	{
		_listHorizontalRoot.SetActive(true);
		_mapListHorizontal.ResetView();
		_listVerticalRoot.SetActive(false);
		_QRCodeNotification.gameObject.SetActive(false);
		_mapListToggle.Reset();

		if (m_cacheService != null)
		{
			m_cacheService.useFileCache = false;
		}

		foreach (GameObject mapPin in m_Pins)
		{
			if (mapPin != null) Destroy(mapPin);
		}

		OnlineMapsMarker3DManager onlineMapsMarker3Ds = _map.GetComponent<OnlineMapsMarker3DManager>();
		onlineMapsMarker3Ds.RemoveAll();
		m_Pins.Clear();
		m_PoisAndLocations.Clear();
		m_PoisAndPins.Clear();

		if(ViewManager.Instance.PreviousKioskState != KioskState.AR)
        {
			m_LastPoiInrange = null;
        }
		m_hasSeenLocationWarning = false;
	}

	private void AddListeners()
	{
		RemoveListeners();
		_mapListToggle.ToggleValueChanged.AddListener(OnMapListToggle);
		_centerButton.onClick.AddListener(CenterOnUser);
		_zoomInButton.onClick.AddListener(OnZoomIn);
		_zoomOutButton.onClick.AddListener(OnZoomOut);

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
		_secretPoiNotification.StartButtonClicked.RemoveAllListeners();

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
			_QRCodeNotification.StartButtonClicked.RemoveAllListeners();
			_QRCodeNotification.StartButtonClicked.AddListener(OnQRCodeStart);
			_QRCodeNotification.Inflate();
			m_LastPoiInrange = poiInRange;
			PlayerManager.Instance.LastPOIInRange = poiInRange;
        }
		PlayerManager.Instance.IsGPSOn = true;
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

		if(!m_hasSeenLocationWarning)
        {
			if(!IsInRange(location, m_tourPoiLocation).isInRange)
			{
				_locationWarningNotification.Inflate();
				m_hasSeenLocationWarning = true;
			}
        }

		return result;
    }

	private (bool isInRange, float distance) IsInRange(Vector2 location, Wezit.PoiLocation poiLocation)
    {
		float distance = MapUtils.CalculateDistance(location, new Vector2(poiLocation.lng, poiLocation.lat));
		return ((distance < (poiLocation.radius == 0 ? 15 : poiLocation.radius), distance));
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
#if UNITY_ANDROID
			if(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
				Permission.RequestUserPermission(Permission.FineLocation);
            }
#elif UNITY_IOS
			Input.location.Start();
			Input.location.Stop();
#endif
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
		_secretPoiNotification.StartButtonClicked.RemoveAllListeners();
		_secretPoiNotification.StartButtonClicked.AddListener(OnSecretPoiPopinClicked);
		_secretPoiNotification.Inflate(poi);
    }

	private void OnSecretPoiPopinClicked(Wezit.Poi poi)
	{
		AppManager.Instance.SelectPoi(poi);
		AppManager.Instance.GoToState(KioskState.SECRET_POI);
	}

	private void OnUserNotInTour()
    {

    }
#endregion Private

#region Internals
	protected override void OnFadeEndView()
	{

	}
#endregion Internals
#endregion Methods
}