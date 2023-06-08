using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private ExplanationWindow _quizPanel;
    [Space]
    [SerializeField] private GameObject _textContainer;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Transform _contrastRoot;
    [Space]
    [SerializeField] private Button _startButton;
    [SerializeField] private Image _startButtonBG;
    [SerializeField] private TextMeshProUGUI _startButtonText;
    #endregion

    #region Private

    private string m_QuizPanelTitleSettingKey = "template.spk.pois.quiz.panel.title";
    private string m_QuizPanelDescriptionSettingKey = "template.spk.pois.quiz.panel.description";
    private string m_QuizPanelButtonTextSettingKey = "template.spk.pois.quiz.panel.button.text";
    #endregion
    #endregion

    #region Methods
    #region Public
    public void Inflate(bool showScreen)
    {
        if(!showScreen)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        _startButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

        string title = Wezit.Settings.Instance.GetSettingAsCleanedText(m_QuizPanelTitleSettingKey);
        string description = Wezit.Settings.Instance.GetSettingAsCleanedText(m_QuizPanelDescriptionSettingKey);

        _title.text = title;
        _description.text = description;
        _startButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_QuizPanelButtonTextSettingKey);
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));

        string[] paragraphs = { description };
        _quizPanel.Inflate(title, paragraphs, _contrastRoot);

        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(OnStartButton);
    }
    #endregion

    #region Private
    private void OnStartButton()
    {
        AppManager.Instance.GoToState(KioskState.QUIZ);
    }
    #endregion
    #endregion
}
