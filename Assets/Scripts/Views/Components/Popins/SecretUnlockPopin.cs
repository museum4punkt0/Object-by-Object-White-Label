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

    private Wezit.Poi m_poiData;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<Wezit.Poi> StartButtonClicked = new UnityEvent<Wezit.Poi>();
    #endregion

    #region Methods
    #region Public
    public void Inflate(Wezit.Poi secretPoi)
    {
        m_poiData = secretPoi;

        string title = Wezit.Settings.Instance.GetSettingAsCleanedText(m_titleSettingKey);
        string description = Wezit.Settings.Instance.GetSettingAsCleanedText(m_descriptionSettingKey);
        string buttonText = Wezit.Settings.Instance.GetSettingAsCleanedText(m_startSettingKey);

        _popin.Inflate(title, description, buttonText, m_iconSettingKey, "", false);
        _popin.PopinButtonClicked.AddListener(OnStartButton);
    }

    public void Open()
    {
        _popin.Open();
    }
    #endregion
    #region Private
    private void OnStartButton()
    {
        PlayerManager.Instance.CurrentPoi = m_poiData;
        StoreAccessor.State.SelectedPoi = m_poiData;
        AppManager.Instance.GoToState(KioskState.SECRET_POI);
        _popin.Close(true);
    }

    private void OnPopinClosed()
    {
        AppManager.Instance.SelectPoi(m_poiData);
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButtonInventory);
    }
    #endregion
    #endregion
}
