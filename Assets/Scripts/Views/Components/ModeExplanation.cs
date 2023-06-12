using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class ModeExplanation : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Popin _popin;
    #endregion
    #region Private
    private string m_ChallengeTitleSettingKey = "template.spk.tours.modeExplanation.challenge.title.text";
    private string m_ChallengeDescriptionSettingKey = "template.spk.tours.modeExplanation.challenge.description";
    private string m_NormalTitleSettingKey = "template.spk.tours.modeExplanation.normal.title.text";
    private string m_NormalDescriptionSettingKey = "template.spk.tours.modeExplanation.normal.description.text";
    private string m_StartSettingKey = "template.spk.tours.modeExplanation.start.button.text";

    private bool m_IsChallenge;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<bool> StartButtonClicked = new UnityEvent<bool>();
    #endregion

    #region Methods
    #region Public
    public void Inflate(bool isChallenge, Language language)
    {
        m_IsChallenge = isChallenge;
        string title = isChallenge ? Wezit.Settings.Instance.GetSettingAsCleanedText(m_ChallengeTitleSettingKey, language) :
                                        Wezit.Settings.Instance.GetSettingAsCleanedText(m_NormalTitleSettingKey, language);
        string description = isChallenge ? Wezit.Settings.Instance.GetSettingAsCleanedText(m_ChallengeDescriptionSettingKey, language) :
                                        Wezit.Settings.Instance.GetSettingAsCleanedText(m_NormalDescriptionSettingKey, language);
        string buttonText = Wezit.Settings.Instance.GetSettingAsCleanedText(m_StartSettingKey, language);

        _popin.Inflate(title, description, buttonText, "", "main");
        _popin.PopinButtonClicked.RemoveAllListeners();
        _popin.PopinButtonClicked.AddListener(OnStartButton);
    }
    #endregion
    #region Private
    private void OnStartButton()
    {
        StartButtonClicked?.Invoke(m_IsChallenge);
    }

    private void OnClose()
    {
        MenuManager.Instance.SetPreviousStatus();
        gameObject.SetActive(false);
    }
    #endregion
    #endregion
}
