using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Wezit;

public class SecretPoiButton : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private Image _background = null;
    [SerializeField] private RawImage _icon = null;
    [SerializeField] private Button _button = null;
    #endregion
    #region Private
    private Poi m_Poi;

    private string m_SecretLockedImageSettingKey = "template.spk.maps.tour.secret.locked.image";
    private string m_SecretLockedColorSettingKey = "template.spk.maps.tour.secret.background.color";
    private string m_SecretUnlockedImageSettingKey = "template.spk.maps.tour.secret.unlocked.image";
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Poi> SecretPoiClicked = new UnityEvent<Poi>();
    #endregion

    #region Methods
    #region Monobehaviour
    #endregion
    #region Public
    public void Inflate(Poi poi)
    {
        m_Poi = poi;
        name = m_Poi.title;
        TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
        _background.color = tourProgressionData.HasBeenCompleted ? GlobalSettingsManager.Instance.AppColor : Settings.Instance.GetSettingAsColor(m_SecretLockedColorSettingKey);
        Settings.Instance.SetImageFromSetting(_icon, tourProgressionData.HasBeenCompleted ? m_SecretUnlockedImageSettingKey : m_SecretLockedImageSettingKey);
        _button.interactable = tourProgressionData.HasBeenVisited;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnButtonClick);
    }
    #endregion
    #region Private
    private void OnButtonClick()
    {
        SecretPoiClicked?.Invoke(m_Poi);
    }
    #endregion
    #endregion
}
