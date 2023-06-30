using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class QuizView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Transform _quizRoot;
	[SerializeField] private Image _colorBackground;
	[Space]
	[SerializeField] private GameObject _pointsPanelRoot;
	[SerializeField] private ContrastButton _pointsPanelContrastButton;
	[SerializeField] private Transform _pointsPanelContrastPanelRoot;
	[SerializeField] private TextMeshProUGUI _pointsTitle;
	[SerializeField] private TextMeshProUGUI _pointsDescription;
	[SerializeField] private TextMeshProUGUI _pointsEarned;
	[Space]
	[SerializeField] private Button _continueButton;
	[SerializeField] private Image _continueButtonBG;
	[SerializeField] private TextMeshProUGUI _continueButtonText;
	#endregion Serialize Fields

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private Language m_Language;
	private TourProgressionData m_TourProgressionData;
	private PoiProgressionData m_PoiProgressionData;
	private Wezit.Quiz m_Quiz;

	private string m_ContinueButtonTextSettingKey = "template.spk.pois.quiz.continue.button.text";
	private string m_SuccessTitleSettingKey = "template.spk.pois.quiz.success.title";
	private string m_SuccessDescriptionSettingKey = "template.spk.pois.quiz.success.description";
	private string m_FailureTitleSettingKey = "template.spk.pois.quiz.failure.title";
	private string m_FailureDescriptionSettingKey = "template.spk.pois.quiz.failure.description";
	private string m_PointsEarnedSettingKey = "template.spk.pois.content.points.earned.description.text";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.QUIZ;
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

		m_PoiData = StoreAccessor.State.SelectedPoi;

		m_TourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
		m_PoiProgressionData = m_TourProgressionData.GetPoiProgression(m_PoiData.pid);

		bool isChallenge = m_TourProgressionData.IsChallengeMode;
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.AR);
		m_Language = language;

		LoadActivity();

		_pointsTitle.color = _colorBackground.color = _continueButtonBG.color = GlobalSettingsManager.Instance.AppColor;
		_continueButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ContinueButtonTextSettingKey, language);
	}

	private void ResetViewContent()
	{
        foreach (Transform child in _quizRoot)
        {
			Destroy(child.gameObject);
		}
		_pointsPanelRoot.SetActive(false);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_continueButton.onClick.AddListener(OnContinueButton);
	}

	private void RemoveListeners()
	{
		_continueButton.onClick.RemoveAllListeners();
	}

	private async void LoadActivity()
    {
        Wezit.Relation relation = await Wezit.ActivityLoader.LookForActivity(m_PoiData);
        if (relation != null)
        {
            m_Quiz = (Wezit.Quiz) Wezit.ActivityLoader.Instance.InstantiateActivity(await Wezit.ActivityLoader.LoadActivity(relation), m_Language, _quizRoot);
			m_Quiz.QuizOver.AddListener(OnActivityOver);
        }
    }

	private void OnActivityOver(bool hasWon)
	{
		m_PoiProgressionData.QuizCompleted = true;
		m_TourProgressionData.TourScore += GlobalSettingsManager.Instance.PointsEarnedSecret;

		_pointsTitle.text = hasWon ? Wezit.Settings.Instance.GetSettingAsCleanedText(m_SuccessTitleSettingKey) : Wezit.Settings.Instance.GetSettingAsCleanedText(m_FailureTitleSettingKey);
		_pointsDescription.text = hasWon ? Wezit.Settings.Instance.GetSettingAsCleanedText(m_SuccessDescriptionSettingKey) : Wezit.Settings.Instance.GetSettingAsCleanedText(m_FailureDescriptionSettingKey);
		_pointsEarned.gameObject.SetActive(hasWon);
		if(hasWon)
		{
			string pointsEarned = Wezit.Settings.Instance.GetSettingAsCleanedText(m_PointsEarnedSettingKey);
			_pointsEarned.text = string.Format(pointsEarned, GlobalSettingsManager.Instance.PointsEarnedSecret);
		}

		string[] paragraphs = { _pointsDescription.text, _pointsEarned.text };
		_pointsPanelContrastButton.Inflate(_pointsTitle.text, paragraphs, _pointsPanelContrastPanelRoot);

		_pointsPanelRoot.SetActive(true);
	}

	private void OnContinueButton()
    {
		AppManager.Instance.GoToState(KioskState.AR);
    }
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}