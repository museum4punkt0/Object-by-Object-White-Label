/**
 * Created by Willy
 */

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;

public class MapManager : MonoBehaviour
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Transform _activityRoot = null;
	[SerializeField] private OnlineMaps _onlineMap = null;
	[SerializeField] private GameObject _pinPrefab = null;
	[SerializeField] private GameObject _pinPrefab2 = null;
	[SerializeField] private Button _centerButton = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private List<HomeEntry> m_HomeEntriesList = null;
	private Wezit.Tour m_WzPoiData = null;
	private List<Wezit.PoiLocation> m_PoiLocations = new List<Wezit.PoiLocation>();
	private int m_CurrentPoi;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	// Special case : if null _homeEntriesGO && null _homeEntriesList => touch screen is start

	#region Methods
	#region MonoBehavior
	private void Start()
	{
	}
	#endregion MonoBehavior

	#region Public

	public void Dispose()
	{
	}
	#endregion Public

	#region Private
	public async void Inflate(Language language)
	{

		m_WzPoiData = WezitDataUtils.GetWezitTourByLang(language);
		if (m_WzPoiData == null) return;
		foreach (Wezit.Poi childPoi in m_WzPoiData.childs)
		{
			//GLTFSpawner.SpawnGLTF(transform, childPoi);

			//Wezit.Relation relation = await Wezit.ActivityLoader.LookForActivity(childPoi);
			//if (relation != null)
			//{
			//    Wezit.ActivityLoader.Instance.InstantiateActivity(await Wezit.ActivityLoader.LoadActivity(relation), language, _activityRoot);
			//}

			_centerButton.onClick.RemoveAllListeners();
			m_CurrentPoi = 0;
			m_PoiLocations.Clear();
			_centerButton.onClick.AddListener(Toto);
			if (PoiLocationStore.GetPoiLocationById(childPoi.pid) != null)
			{
				int max = Mathf.Max(Screen.width / 2, Screen.height / 2);
				_onlineMap.GetComponent<OnlineMapsTileSetControl>().sizeInScene = max * Vector2.one;
				_onlineMap.GetComponent<OnlineMapsCameraOrbit>().adjustToGameObject.transform.localPosition = new Vector3(-max / 2, 0, 5 * max / 8);
				Wezit.PoiLocation poiLocation = PoiLocationStore.GetPoiLocationById(childPoi.pid);
				m_PoiLocations.Add(poiLocation);
				_onlineMap.position = new Vector2(poiLocation.lng, poiLocation.lat);
				OnlineMapsMarker3DManager markersManager = _onlineMap.GetComponent<OnlineMapsMarker3DManager>();
				markersManager.Create(poiLocation.lng, poiLocation.lat, _pinPrefab);

				foreach (Wezit.Poi subchildPoi in childPoi.childs)
				{
					Wezit.PoiLocation childPoiLocation = PoiLocationStore.GetPoiLocationById(subchildPoi.pid);
					m_PoiLocations.Add(childPoiLocation);
					markersManager.Create(childPoiLocation.lng, childPoiLocation.lat, _pinPrefab2);
				}
			}
		}
	}

	private void Toto()
	{
		m_CurrentPoi = (m_CurrentPoi + 1) % m_PoiLocations.Count;
		//_onlineMap.position = new Vector2(m_PoiLocations[m_CurrentPoi].lng, m_PoiLocations[m_CurrentPoi].lat);
		_onlineMap.SetPositionAndZoom(m_PoiLocations[m_CurrentPoi].lng, m_PoiLocations[m_CurrentPoi].lat, 18);
	}

	#endregion Private

	#region Internals
	#endregion Internals
	#endregion Methods
}