using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;
using TMPro;

public class MapListVertical : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private Transform _prefabRoot;
    [SerializeField] private MapListItem _itemPrefab;
    [SerializeField] private TextMeshProUGUI _title;
    #endregion
    #region Private
    List<MapListItem> m_Items = new List<MapListItem>();
    private string m_TitleSettingKey = "template.spk.tours.list.screen.list.title.text";
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Tour> ItemClickedTour = new UnityEvent<Tour>();
    public UnityEvent<Poi> ItemClickedPoi = new UnityEvent<Poi>();
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
            itemInstance.Inflate(tour, activeMonobehaviour);
            itemInstance.ItemClickedTour.AddListener(OnItemClicked);
            m_Items.Add(itemInstance);
        }
        _title.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = Settings.Instance.GetSettingAsCleanedText(m_TitleSettingKey, StoreAccessor.State.Language);
    }

    public void Inflate(List<Poi> pois, MonoBehaviour activeMonobehaviour)
    {
        ResetView();
        foreach (Poi poi in pois)
        {
            MapListItem itemInstance = Instantiate(_itemPrefab, _prefabRoot);
            itemInstance.Inflate(poi, activeMonobehaviour);
            itemInstance.ItemClickedTour.AddListener(OnItemClicked);
            m_Items.Add(itemInstance);
        }
        _title.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = Settings.Instance.GetSettingAsCleanedText(m_TitleSettingKey, StoreAccessor.State.Language);
    }
    #endregion
    #region Private
    private void ResetView()
    {
        foreach(MapListItem item in m_Items)
        {
            Destroy(item.gameObject);
        }
        m_Items.Clear();
    }

    private void OnItemClicked(Tour tour)
    {
        ItemClickedTour?.Invoke(tour);
    }
    #endregion
    #endregion
}
