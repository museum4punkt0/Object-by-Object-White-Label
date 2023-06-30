using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class LocationWarningNotification : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Popin _popin;
    #endregion
    #region Private
    private string m_TitleSettingKey = "template.spk.tours.map.popups.location.warning.title.text";
    private string m_DescriptionSettingKey = "template.spk.tours.map.popups.location.warning.description.text";
    private string m_StartSettingKey = "template.spk.tours.map.popups.location.warning.button.text";
    #endregion
    #endregion

    #region Properties
    public UnityEvent StartButtonClicked = new UnityEvent();
    #endregion

    #region Methods
    #region Public
    public void Inflate()
    {
        gameObject.SetActive(true);
        string title = Wezit.Settings.Instance.GetSettingAsCleanedText(m_TitleSettingKey);
        string description = Wezit.Settings.Instance.GetSettingAsCleanedText(m_DescriptionSettingKey);
        string buttonText = Wezit.Settings.Instance.GetSettingAsCleanedText(m_StartSettingKey);

        _popin.Inflate(title, description, buttonText, "", "main");
        _popin.PopinButtonClicked.AddListener(OnStartButton);
    }
    #endregion
    #region Private
    private void OnStartButton()
    {
        StartButtonClicked?.Invoke();
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
