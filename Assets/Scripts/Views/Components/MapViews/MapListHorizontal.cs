using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;
using TMPro;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.UI;

public class MapListHorizontal : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private Transform _prefabRoot;
    [SerializeField] private MapListItem _itemPrefab;
    [SerializeField] private SimpleScrollSnap _simpleScrollSnap;
    #endregion
    #region Private
    List<MapListItem> m_Items = new List<MapListItem>();
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Tour> ItemClickedTour = new UnityEvent<Tour>();
    public UnityEvent<Poi> ItemClickedPoi = new UnityEvent<Poi>();
    public UnityEvent<Vector2> ItemSelectedTour = new UnityEvent<Vector2>();
    public UnityEvent<Vector2, Poi> ItemSelectedPoi = new UnityEvent<Vector2, Poi>();
    #endregion

    #region Methods
    #region Monobehaviour
    #endregion
    #region Public
    public void Inflate(List<Tour> tours, MonoBehaviour activeMonobehaviour)
    {
        ResetView();
        foreach (Tour tour in tours)
        {
            MapListItem itemInstance = Instantiate(_itemPrefab, _prefabRoot);
            itemInstance.name = tour.title;
            itemInstance.Inflate(tour, activeMonobehaviour);
            itemInstance.ItemClickedTour.AddListener(OnItemClickedTour);
            m_Items.Add(itemInstance);
        }
        _simpleScrollSnap.Setup();
        _simpleScrollSnap.OnPanelCentered.AddListener(OnItemSelectedTour);
        _simpleScrollSnap.GetComponent<ScrollRect>().enabled = tours.Count > 1;
    }

    public void Inflate(List<Poi> pois, MonoBehaviour activeMonobehaviour)
    {
        ResetView();
        foreach (Poi poi in pois)
        {
            MapListItem itemInstance = Instantiate(_itemPrefab, _prefabRoot);
            itemInstance.name = poi.title;
            itemInstance.Inflate(poi, activeMonobehaviour);
            itemInstance.ItemClickedPoi.AddListener(OnItemClickedPoi);
            m_Items.Add(itemInstance);
        }
        _simpleScrollSnap.Setup();
        _simpleScrollSnap.OnPanelCentered.AddListener(OnItemSelectedPoi);
        _simpleScrollSnap.GetComponent<ScrollRect>().enabled = pois.Count > 1;
    }

    public void SelectPoi(string pid)
    {
        int index = (m_Items.FindIndex(0, m_Items.Count - 1, x => x.pid == pid) + m_Items.Count) % m_Items.Count;
        _simpleScrollSnap.GoToPanel(index);
    }

    public void ResetView()
    {
        foreach(MapListItem item in m_Items)
        {
            Destroy(item.gameObject);
        }
        m_Items.Clear();
    }
    #endregion
    #region Private
    private void OnItemClickedTour(Tour tour)
    {
        ItemClickedTour?.Invoke(tour);
    }

    private void OnItemClickedPoi(Poi poi)
    {
        ItemClickedPoi?.Invoke(poi);
    }

    private void OnItemSelectedTour(int centered, int selected)
    {
        ItemSelectedTour?.Invoke(m_Items[centered].Geolocation);
    }

    private void OnItemSelectedPoi(int centered, int selected)
    {
        ItemSelectedPoi?.Invoke(m_Items[centered].Geolocation, m_Items[centered].Poi);
    }
    #endregion
    #endregion
}
