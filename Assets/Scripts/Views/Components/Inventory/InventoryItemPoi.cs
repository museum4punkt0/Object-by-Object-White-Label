using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Wezit;
using TMPro;

public class InventoryItemPoi : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private Image _titleBackground;
    [SerializeField] private Image _titleBackgroundBottom;
    [SerializeField] private InventoryItemContent _inventoryItemContentPrefab;
    [SerializeField] private Transform _inventoryItemContentRoot;
    #endregion
    #region Private
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(Poi poi)
    {
        ResetComponent();

        _titleBackground.color = _titleBackgroundBottom.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = StringUtils.CleanFromWezit(poi.title);
        
        TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
        PoiProgressionData poiProgressionData = tourProgressionData.GetPoiProgression(poi.pid);
        if(tourProgressionData.IsChallengeMode)
        {
            _score.gameObject.SetActive(true);
            _score.text = poiProgressionData.GetPoiCurrentProgression() + "pts";
        }

        foreach (Poi contentPoi in poi.childs)
        {
            if (poiProgressionData.GetContentProgression(contentPoi.pid).HasBeenCompleted)
            {
                InventoryItemContent instance = Instantiate(_inventoryItemContentPrefab, _inventoryItemContentRoot);
                instance.Inflate(contentPoi);
            }
        }
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_inventoryItemContentRoot.gameObject));
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_inventoryItemContentRoot.gameObject));
    }
    #endregion
    #region Private
    private void ResetComponent()
    {
        _score.gameObject.SetActive(false);
        foreach (Transform child in _inventoryItemContentRoot)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
    #endregion
}
