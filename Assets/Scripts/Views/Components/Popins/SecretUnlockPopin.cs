using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SecretUnlockPopin : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Popin _popin;
    #endregion
    #region Private
    private string m_titleSettingKey = "template.spk.tours.map.popups.secret.unlocked.title.text";
    private string m_descriptionSettingKey = "template.spk.tours.map.popups.secret.unlocked.description.text";
    private string m_startSettingKey = "template.spk.tours.map.popups.secret.unlocked.button.text";
    private string m_iconSettingKey = "template.spk.tours.map.popups.secret.unlocked.icon.image";
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Wezit.Poi> StartButtonClicked = new UnityEvent<Wezit.Poi>();
    #endregion

    #region Methods
    #region Public
    public void Inflate()
    {
        string title = Wezit.Settings.Instance.GetSettingAsCleanedText(m_titleSettingKey);
        string description = Wezit.Settings.Instance.GetSettingAsCleanedText(m_descriptionSettingKey);
        string buttonText = Wezit.Settings.Instance.GetSettingAsCleanedText(m_startSettingKey);

        _popin.Inflate(title, description, buttonText, m_iconSettingKey, "");
        _popin.PopinButtonClicked.AddListener(OnStartButton);
    }
    #endregion
    #region Private
    private void OnStartButton()
    {
        _popin.Close(true);
    }

    private void OnPopinClosed()
    {
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButtonInventory);
    }
    #endregion
    #endregion
}
