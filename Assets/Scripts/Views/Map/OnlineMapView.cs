﻿using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class OnlineMapView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private OnlineMaps _map = null;
	[SerializeField] private MapPin _mapPinPrefab = null;
	[SerializeField] private GameObject _listRoot = null;
	[SerializeField] private MapListVertical _list = null;
	[SerializeField] private GameObject _mapRoot = null;
	[SerializeField] private MapListHorizontal _mapList = null;
	[SerializeField] private MapListToggle _mapListToggle = null;
	[SerializeField] private Button _centerButton = null;
	[SerializeField] private Image _centerButtonBG = null;
	[SerializeField] private Button _zoomInButton = null;
	[SerializeField] private Image _zoomInButtonBG = null;
	[SerializeField] private Button _zoomOutButton = null;
	[SerializeField] private Image _zoomOutButtonBG = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private List<Wezit.Tour> m_Tours = new List<Wezit.Tour>();
	private OnlineMapsLocationService m_LocationService;
	private OnlineMapsCache m_cacheService;
	private Vector3 m_CenteredPostion;
	private List<GameObject> m_Pins = new List<GameObject>();

	private string m_mapProviderSettingKey = "template.spk.maps.global.map.provider.url";
	private string m_mapProviderUrl;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.GLOBAL_MAP;
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
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.RightImage);
		PlayerManager.Instance.ViewOnInventoryBackButton = KioskState.GLOBAL_MAP;

		StoreAccessor.State.SelectedTourBank = null;

		_centerButtonBG.color = _zoomInButtonBG.color = _zoomOutButtonBG.color = GlobalSettingsManager.Instance.AppColor;

		m_cacheService = _map.GetComponent<OnlineMapsCache>();
		m_cacheService.useFileCache = true;
		m_cacheService.fileCacheTilePath = "TileCache/Global_Map/OnlineMap/{lbs}/{lng}/{zoom}/{x}/{y}";

		m_mapProviderUrl = Wezit.Settings.Instance.GetSettingAsCleanedText(m_mapProviderSettingKey);
		if (!string.IsNullOrEmpty(m_mapProviderUrl))
        {
			_map.customProviderURL = m_mapProviderUrl;
        }
		else
        {
			_map.customProviderURL = "https://tiles.stadiamaps.com/styles/stamen_watercolor/{z}/{x}/{y}.jpg";

		}

		m_Tours = WezitDataUtils.GetWezitToursByLang(language);
		OnlineMapsMarker3DManager onlineMapsMarker3Ds = _map.GetComponent<OnlineMapsMarker3DManager>();
		m_LocationService = _map.GetComponent<OnlineMapsLocationService>();
		List<Vector2> poiLocations = new List<Vector2>();

		foreach(Wezit.Tour tour in m_Tours)
        {
			Wezit.PoiLocation poiLocation = PoiLocationStore.GetPoiLocationById(tour.pid);
			if (poiLocation != null)
            {
				poiLocations.Add(new Vector2(poiLocation.lng, poiLocation.lat));

				OnlineMapsMarker3D marker3D = onlineMapsMarker3Ds.Create(poiLocation.lng, poiLocation.lat, _mapPinPrefab.gameObject);
				MapPin mapPinInstance = marker3D.instance.GetComponent<MapPin>();
				m_Pins.Add(marker3D.instance);
				mapPinInstance.Inflate(tour);
				mapPinInstance.GlobalMapPinClicked.AddListener(OnMapPinClicked);
			}
        }
		_list.Inflate(m_Tours, this);
		_list.ItemClickedTour.AddListener(OnTourClicked);
		_mapList.Inflate(m_Tours, this);
		_mapList.ItemSelectedTour.AddListener(OnItemSelected);
		_mapList.ItemClickedTour.AddListener(OnTourClicked);

		m_CenteredPostion = MapUtils.CenterMapOnPoints(poiLocations);
		_map.SetPositionAndZoom(m_CenteredPostion.x, m_CenteredPostion.y, m_CenteredPostion.z);
	}

	private void OnItemSelected(Vector2 geolocation)
    {
		_map.SetPositionAndZoom(geolocation.x, geolocation.y, 16);
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

	private void ResetViewContent()
	{
		_mapRoot.SetActive(true);
		_mapList.ResetView();
		_listRoot.SetActive(false);
		_mapListToggle.Reset();
		
		if(m_cacheService != null)
        {
			m_cacheService.useFileCache = false;
        }

		foreach (GameObject mapPin in m_Pins)
        {
            if (mapPin!= null) Destroy(mapPin);
        }
		OnlineMapsMarker3DManager onlineMapsMarker3Ds = _map.GetComponent<OnlineMapsMarker3DManager>();
		onlineMapsMarker3Ds.RemoveAll();
        m_Pins.Clear();
	}

	private void OnMapPinClicked(Wezit.Tour tour)
    {
		_mapList.SelectPoi(tour.pid);
	}

	private void OnTourClicked(Wezit.Tour tour)
    {
		StoreAccessor.State.SelectedTour = tour;
		PlayerManager.Instance.CurrentTour = tour;
		AppManager.Instance.GoToState(KioskState.TOUR_INTRO);
    }

	private void AddListeners()
	{
		RemoveListeners();
		_mapListToggle.ToggleValueChanged.AddListener(OnMapListToggle);
		_centerButton.onClick.AddListener(CenterOnUser);
		_zoomInButton.onClick.AddListener(OnZoomIn);
		_zoomOutButton.onClick.AddListener(OnZoomOut);
	}

	private void RemoveListeners()
	{
		_mapListToggle.ToggleValueChanged.RemoveAllListeners();
		_centerButton.onClick.RemoveAllListeners();
		_zoomInButton.onClick.RemoveAllListeners();
		_zoomOutButton.onClick.RemoveAllListeners();
	}

	private void OnMapListToggle(bool isOn)
    {
		_mapRoot.SetActive(isOn);
		_listRoot.SetActive(!isOn);
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
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}