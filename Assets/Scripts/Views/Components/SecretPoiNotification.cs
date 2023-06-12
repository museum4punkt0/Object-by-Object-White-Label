using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SecretPoiNotification : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Popin _popin;
    #endregion
    #region Private
    private Wezit.Poi m_poi;

    private string m_LockedTitleSettingKey = "template.spk.tours.map.popups.uncomplete.title.text";
    private string m_LockedDescriptionSettingKey = "template.spk.tours.map.popups.uncomplete.description.text";
    private string m_LockedStartSettingKey = "template.spk.tours.map.popups.uncomplete.button.text";
    private string m_UnlockedTitleSettingKey = "template.spk.tours.map.popups.complete.title.text";
    private string m_UnlockedDescriptionSettingKey = "template.spk.tours.map.popups.complete.description.text";
    private string m_UnlockedStartSettingKey = "template.spk.tours.map.popups.complete.button.text";
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Wezit.Poi> StartButtonClicked = new UnityEvent<Wezit.Poi>();
    #endregion

    #region Methods
    #region Public
    public void Inflate(Wezit.Poi poi)
    {
        m_poi = poi;

        bool isUnlocked = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).HasBeenCompleted;

        string title = Wezit.Settings.Instance.GetSettingAsCleanedText(isUnlocked ? m_UnlockedTitleSettingKey : m_LockedTitleSettingKey);
        string description = Wezit.Settings.Instance.GetSettingAsCleanedText(isUnlocked ? m_UnlockedDescriptionSettingKey : m_LockedDescriptionSettingKey);
        string buttonText = Wezit.Settings.Instance.GetSettingAsCleanedText(isUnlocked ? m_UnlockedStartSettingKey : m_LockedStartSettingKey);

        _popin.Inflate(title, description, buttonText, "", isUnlocked ? "good" : "bad");
        _popin.PopinButtonClicked.AddListener(OnStartButton);
    }
    #endregion
    #region Private
    private void OnStartButton()
    {
        StartButtonClicked?.Invoke(m_poi);
        OnClose();
    }

    private void OnClose()
    {
        MenuManager.Instance.SetPreviousStatus();
        gameObject.SetActive(false);
    }
    #endregion
    #endregion
}
