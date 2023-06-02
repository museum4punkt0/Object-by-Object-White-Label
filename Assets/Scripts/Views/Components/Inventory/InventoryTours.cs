using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wezit;
using TMPro;

public class InventoryTours : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private InventoryItemTour _tourPrefab = null;
    [SerializeField] private Transform _toursRoot = null;
    [SerializeField] private TextMeshProUGUI _title = null;
    #endregion
    #region Private
    private List<Wezit.Tour> m_Tours = new List<Wezit.Tour>();

    private string m_InventoryTitleSettingKey = "";
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Tour> TourClicked = new();
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Init(Language language)
    {
        ResetComponent();

        _title.text = Settings.Instance.GetSettingAsCleanedText(m_InventoryTitleSettingKey, language);
        _title.color = GlobalSettingsManager.Instance.AppColor;

        m_Tours = WezitDataUtils.GetWezitToursByLang(language);
        foreach(Tour tour in m_Tours)
        {
            InventoryItemTour instance = Instantiate(_tourPrefab, _toursRoot);
            instance.Inflate(tour);
        }
    }
    #endregion
    #region Private
    private void ResetComponent()
    {
        for (int i = 1; i < _toursRoot.childCount; i++)
        {
            Destroy(_toursRoot.GetChild(i).gameObject);
        }
    }
    #endregion
    #endregion
}
