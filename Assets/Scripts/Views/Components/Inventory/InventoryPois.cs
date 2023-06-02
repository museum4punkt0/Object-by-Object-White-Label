using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;
using TMPro;

public class InventoryPois : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private InventoryItemPoi _poiItemPrefab;
    [SerializeField] private Transform _poiItemsRoot;
    #endregion
    #region Private
    private Tour m_Tour;
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(Tour tour)
    {
        ResetComponent();

        m_Tour = tour;
        _title.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = StringUtils.CleanFromWezit(m_Tour.title);
        foreach (Poi poi in m_Tour.childs)
        {
            if(poi.type == "secret")
            {
                continue;
            }
            else
            {
                InventoryItemPoi instance = Instantiate(_poiItemPrefab, _poiItemsRoot);
                instance.Inflate(poi);
            }
        }
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_poiItemsRoot.gameObject));
    }
    #endregion
    #region Private
    private void ResetComponent()
    {
        for (int i = 1; i < _poiItemsRoot.childCount; i++)
        {
            Destroy(_poiItemsRoot.GetChild(i).gameObject);
        }
    }
    #endregion
    #endregion
}
