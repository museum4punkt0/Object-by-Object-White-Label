using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class TourIntroView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private RawImage _colorBackground = null;
	[SerializeField] private RawImage _background = null;
	[SerializeField] private string _backgroundSettingKey = "";
	[Header("Explanation")]
	[SerializeField] private ExplanationWindow _explanationWindow = null;
	[SerializeField] private TextMeshProUGUI _title = null;
	[SerializeField] private TextMeshProUGUI _description = null;

	[Space]
	[SerializeField] private Button _challengeButton = null;
	[SerializeField] private Image _challengeButtonBG = null;
	[SerializeField] private TextMeshProUGUI _challengeButtonText = null;
    [Space]
	[SerializeField] private Button _normalButton = null;
	[SerializeField] private Image _normalButtonBG = null;
	[SerializeField] private TextMeshProUGUI _normalButtonText = null;
    [Space]
	[SerializeField] private Button _startButton = null;
	[SerializeField] private Image _startButtonBG = null;
	[SerializeField] private TextMeshProUGUI _startButtonText = null;
    [Space]
	[SerializeField] private Transform _contrastPanelRoot = null;
	[SerializeField] private ModeExplanation _modeExplanation = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Tour m_TourData;
	private Language m_Language;
	
	private string m_ChallengeButtonTextSettingKey = "template.spk.tours.modeExplanation.challenge.button.text";
	private string m_NormalButtonTextSettingKey = "template.spk.tours.modeExplanation.normal.button.text";
	private string m_StartButtonTextSettingKey = "template.spk.tours.modeExplanation.start.button.text";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.TOUR_INTRO;
	}

	public override void InitView()
	{
		ResetViewContent();
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		base.ShowView();
		InitViewContentByLang(ViewManager.Instance.CurrentLanguage);
		AddListeners();
	}

	public override void HideView()
	{
		RemoveListeners();
		base.HideView();
	}

	public override void PrepareHideView()
	{
		RemoveListeners();
		base.PrepareHideView();
	}

	public override void OnLanguageUpdated(Language language)
	{
		if (m_IsActive && AppManager.Instance.loadingOver)
		{
			InitViewContentByLang(language);
		}
	}

	public void Dispose()
	{
		RemoveListeners();
	}
	#endregion Public

	#region Private
	private void InitViewContentByLang(Language language)
	{
		ResetViewContent();

		m_TourData = StoreAccessor.State.SelectedTour;
		m_Language = language;
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButton);
		MenuManager.Instance.SetBackButtonState(KioskState.GLOBAL_MAP);
		MenuManager.Instance.SetTitle(StringUtils.CleanFromWezit(m_TourData.title));

		_colorBackground.color = _startButtonBG.color = _challengeButtonBG.color = _normalButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

		ImageUtils.LoadImage(_background, this, m_TourData);

		TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(m_TourData.pid);
		if(tourProgressionData.IsModeSet)
        {
			_startButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_StartButtonTextSettingKey, language);
		}
		else
        {
			_challengeButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ChallengeButtonTextSettingKey, language);
			_normalButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_NormalButtonTextSettingKey, language);
        }
		_startButton.gameObject.SetActive(tourProgressionData.IsModeSet);
		_challengeButton.gameObject.SetActive(!tourProgressionData.IsModeSet);
		_normalButton.gameObject.SetActive(!tourProgressionData.IsModeSet);

		_title.text = m_TourData.title;
        _description.text = m_TourData.description;
        string[] paragraphs = { _description.text };
		_explanationWindow.Inflate(_title.text, paragraphs, _contrastPanelRoot);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
		_modeExplanation.gameObject.SetActive(false);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_challengeButton.onClick.AddListener(OnChallengeButton);
		_normalButton.onClick.AddListener(OnNormalButton);
		_modeExplanation.StartButtonClicked.AddListener(OnSetModeAndStart);
		_startButton.onClick.AddListener(OnStart);
	}

	private void RemoveListeners()
	{
		_challengeButton.onClick.RemoveAllListeners();
		_normalButton.onClick.RemoveAllListeners();
		_modeExplanation.StartButtonClicked.RemoveAllListeners();
		_startButton.onClick.RemoveAllListeners();
	}

	private void OnChallengeButton()
	{
		_modeExplanation.gameObject.SetActive(true);
		_modeExplanation.Inflate(true, m_Language);
	}

	private void OnNormalButton()
	{
		_modeExplanation.gameObject.SetActive(true);
		_modeExplanation.Inflate(false, m_Language);
	}

	private void OnSetModeAndStart(bool isChallenge)
    {
		TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(m_TourData.pid);
		tourProgressionData.IsModeSet = true;
		tourProgressionData.IsChallengeMode = isChallenge;
		PlayerManager.Instance.Player.Save();
		OnStart();
    }

	private void OnStart()
	{
		AppManager.Instance.GoToState(KioskState.TOUR_MAP);
	}
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}