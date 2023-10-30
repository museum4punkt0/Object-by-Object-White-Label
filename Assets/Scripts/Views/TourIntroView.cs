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
    [Space]
	[SerializeField] private Button _downloadButton = null;
	[SerializeField] private Image _downloadButtonBG = null;
	[SerializeField] private DownloadPopin _downloadPopin = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Tour m_TourData;
	private Language m_Language;
	
	private string m_challengeButtonTextSettingKey = "template.spk.tours.modeExplanation.challenge.button.text";
	private string m_normalButtonTextSettingKey = "template.spk.tours.modeExplanation.normal.button.text";
	private string m_startButtonTextSettingKey = "template.spk.tours.modeExplanation.start.button.text";

	private int m_downloadSize;
	private string m_downloadPopinTitleSettingKey = "template.spk.tours.download.title.text";
	private string m_downloadPopinDescriptionSettingKey = "template.spk.tours.download.description.text";
	private string m_downloadPopinButtonSettingKey = "template.spk.tours.download.button.text";
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
	private async void InitViewContentByLang(Language language)
	{
		ResetViewContent();

		m_TourData = StoreAccessor.State.SelectedTour;
		m_Language = language;
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButton);
		MenuManager.Instance.SetBackButtonState(KioskState.GLOBAL_MAP);
		MenuManager.Instance.SetTitle(m_TourData.CleanedTitle);

        foreach (Wezit.Poi child in m_TourData.childs)
        {
			if(child.type == "bank")
            {
				StoreAccessor.State.SelectedTourBank = child;
            }
        }

		_colorBackground.color = _startButtonBG.color = _challengeButtonBG.color = _normalButtonBG.color = _title.color = _downloadButtonBG .color = GlobalSettingsManager.Instance.AppColor;

		ImageUtils.LoadImage(_background, this, m_TourData);

		TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(m_TourData.pid);
		if(tourProgressionData.IsModeSet)
        {
			if(tourProgressionData.IsChallengeMode)
            {
				ScoreDisplay.Instance.Init();
			}

			_startButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_startButtonTextSettingKey, language);
		}
		else
        {
			_challengeButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_challengeButtonTextSettingKey, language);
			_normalButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_normalButtonTextSettingKey, language);
        }
		_startButton.gameObject.SetActive(tourProgressionData.IsModeSet);
		_challengeButton.gameObject.SetActive(!tourProgressionData.IsModeSet);
		_normalButton.gameObject.SetActive(!tourProgressionData.IsModeSet);

		_title.text = m_TourData.CleanedTitle;
        _description.text = m_TourData.CleanedDescription;
        string[] paragraphs = { _description.text };
		_explanationWindow.Inflate(_title.text, paragraphs, _contrastPanelRoot, await AudioUtils.GetAudioSource(m_TourData));

		// Check download necessity
		if (PlayerManager.Instance.Player.GetTourProgression(m_TourData.pid).HasBeenDownloaded)
		{
			m_downloadSize = Wezit.DataGrabber.Instance.GetUpdateSizeForTour(m_TourData.pid);
		}
		else
		{
			m_downloadSize = Wezit.DataGrabber.Instance.GetDownloadSizeForAssets(Wezit.AssetsLoader.GetAssetsForTour(m_TourData.pid));
		}
		_downloadButton.gameObject.SetActive(m_downloadSize != 0);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
		_modeExplanation.gameObject.SetActive(false);
		_downloadPopin.Close(false);
		_downloadButton.gameObject.SetActive(false);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_challengeButton.onClick.AddListener(OnChallengeButton);
		_normalButton.onClick.AddListener(OnNormalButton);
		_modeExplanation.StartButtonClicked.AddListener(OnSetModeAndStart);
		_startButton.onClick.AddListener(OnStart);
		_downloadButton.onClick.AddListener(OnDownloadOpen);
		_downloadPopin.DownloadOver.AddListener(OnDownloadOver);
	}

	private void RemoveListeners()
	{
		_challengeButton.onClick.RemoveAllListeners();
		_normalButton.onClick.RemoveAllListeners();
		_modeExplanation.StartButtonClicked.RemoveAllListeners();
		_startButton.onClick.RemoveAllListeners();
		_downloadButton.onClick.RemoveAllListeners();
		_downloadPopin.DownloadOver.RemoveAllListeners();
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

	private void OnDownloadOpen()
    {
		_downloadPopin.Inflate(Wezit.Settings.Instance.GetSettingAsCleanedText(m_downloadPopinTitleSettingKey),
			string.Format(Wezit.Settings.Instance.GetSettingAsCleanedText(m_downloadPopinDescriptionSettingKey), string.Format(" {0:0.00}Mo", m_downloadSize / 1024f / 1024f)),
			Wezit.Settings.Instance.GetSettingAsCleanedText(m_downloadPopinButtonSettingKey), m_TourData.pid, m_downloadSize);
    }

	private void OnDownloadOver(bool success)
    {
		_downloadButton.gameObject.SetActive(!success);
    }
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}