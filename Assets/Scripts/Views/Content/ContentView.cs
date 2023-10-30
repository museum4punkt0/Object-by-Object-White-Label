﻿using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class ContentView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private RawImage _background = null;
	[SerializeField] private PinchableScrollRect _pinchableScrollRect = null;
	[SerializeField] private Image _uIBackground = null;
	[Header("Explanation")]
	[SerializeField] private ExplanationWindow _explanationWindow = null;
	[SerializeField] private TextMeshProUGUI _title = null;
	[SerializeField] private TextMeshProUGUI _description = null;
	[SerializeField] private TextMeshProUGUI _remainingItems = null;
	[SerializeField] private GameObject _textContainer = null;

    [Space]
    [Header("Content")]
	[SerializeField] private Button _continueButton = null;
	[SerializeField] private Image _continueButtonBG = null;
	[SerializeField] private TextMeshProUGUI _continueButtonText = null;
	[SerializeField] private Transform _contrastPanelRoot = null;
	[SerializeField] private AudioManager _audioManager;
	[SerializeField] private VideoManager _videoManager;
	[SerializeField] private ThreeDManager _threeDManager;
	[SerializeField] private Image _3DManipulationInstruction;
	[Space]
	[Header("Popin")]
	[SerializeField] private PopinPoints _popinPoints;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private TourProgressionData m_TourProgressionData;
	private PoiProgressionData m_PoiProgressionData;

	private string m_ContinueButtonTextSettingKey = "template.spk.pois.content.button.continue.text";
	private string m_RemainingItemsPluralTextSettingKey = "template.spk.pois.content.remainingItems.plural.text";
	private string m_RemainingItemsSingularTextSettingKey = "template.spk.pois.content.remainingItems.plural.text";
    #endregion Private m_Variables
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    #region Public
    public override KioskState GetKioskState()
	{
		return KioskState.CONTENT;
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
		if (StoreAccessor.State.SelectedPoi == m_PoiData && m_PoiData != null)
		{
			AppManager.Instance.SelectPoi(m_PoiData.parentPoi);
		}
		base.PrepareHideView();
	}

	public override void OnLanguageUpdated(Language language)
	{
		if (m_IsActive && AppManager.Instance.loadingOver)
		{
			InitViewContentByLang(language);
		}
	}
	#endregion Public

	#region Private
	private async void InitViewContentByLang(Language language)
	{
		ResetViewContent();

		m_PoiData = StoreAccessor.State.SelectedPoi;

		m_TourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
		bool isChallenge = m_TourProgressionData.IsChallengeMode;
		m_PoiProgressionData = m_TourProgressionData.GetPoiProgression(m_PoiData.parentPid);
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(ViewManager.Instance.PreviousKioskState);

		_remainingItems.color = _uIBackground.color = _3DManipulationInstruction.color = _continueButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

		_pinchableScrollRect.Init(true);
		switch (m_PoiData.type)
        {
			case ARItemTypes.image:
				ImageUtils.LoadImage(_background, this, m_PoiData, Wezit.RelationName.SHOW_PICTURE, WezitSourceTransformation.default_base, false);
				OnItemCompleted();
				break;
			case ARItemTypes.video:
				ImageUtils.LoadCover(_background, this, m_PoiData, Wezit.RelationName.PLAY_VIDEO, WezitSourceTransformation.default_base, false);
				_videoManager.Toggle(true);
				_videoManager.Inflate(await VideoUtils.GetVideoSourceByTransformation(m_PoiData), m_PoiData.title);
				break;
			case ARItemTypes.audio:
				ImageUtils.LoadCover(_background, this, m_PoiData, Wezit.RelationName.PLAY_TRACK, WezitSourceTransformation.default_base, false);
				_audioManager.gameObject.SetActive(true);
				_audioManager.Inflate(await AudioUtils.GetAudioClip(m_PoiData));
				break;
			case ARItemTypes.threeD:
				_threeDManager.gameObject.SetActive(true);
				_threeDManager.Inflate(m_PoiData);
				_uIBackground.enabled = false;
				_3DManipulationInstruction.gameObject.SetActive(true);
				break;
		}

		_continueButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ContinueButtonTextSettingKey, language);
		_title.text = m_PoiData.CleanedTitle;
		_description.text = m_PoiData.CleanedDescription;
        string[] paragraphs = { _description.text };
		_explanationWindow.Inflate(_title.text, paragraphs, _contrastPanelRoot, await AudioUtils.GetAudioSource(m_PoiData));
		await StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));
		await StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));

		int progress = m_PoiProgressionData.GetPoiCurrentProgression();
		int numberofPois = m_PoiProgressionData.GetPoiMaxProgression();
		string remainingText =  (numberofPois - progress) > 1 ? Wezit.Settings.Instance.GetSettingAsCleanedText(m_RemainingItemsPluralTextSettingKey) :
																Wezit.Settings.Instance.GetSettingAsCleanedText(m_RemainingItemsSingularTextSettingKey);
		_remainingItems.text = string.Format(remainingText, numberofPois - progress);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
		_background.gameObject.SetActive(true);
		_uIBackground.enabled = true;
		_audioManager.gameObject.SetActive(false);
		_videoManager.Toggle(false);
		_threeDManager.gameObject.SetActive(false);
		_popinPoints.gameObject.SetActive(false);
		_3DManipulationInstruction.gameObject.SetActive(false);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_continueButton.onClick.AddListener(OnContinueButton);
		_videoManager.VideoPlayerToggled.AddListener(OnVideoPlayerToggle);
		_videoManager.VideoLoopPointReached.AddListener(OnItemCompleted);
		_audioManager.AudioLoopPointReached.AddListener(OnItemCompleted);
		_threeDManager.ItemManipulated.AddListener(OnItemCompleted);
	}

	private void RemoveListeners()
	{
		_continueButton.onClick.RemoveAllListeners();
		_videoManager.VideoPlayerToggled.RemoveAllListeners();
		_videoManager.VideoLoopPointReached.RemoveAllListeners();
		_audioManager.AudioLoopPointReached.RemoveAllListeners();
		_threeDManager.ItemManipulated.RemoveAllListeners();
	}

	private void OnContinueButton()
    {
		AppManager.Instance.GoToState(ViewManager.Instance.PreviousKioskState);
    }

	private void OnVideoPlayerToggle(bool isOpen)
    {
        //_explanationWindow.gameObject.SetActive(!isOpen);
        if (!isOpen)
		{
			StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));
			_explanationWindow.Rebuild();
		}
    }

	private void OnItemCompleted()
    {
		ContentProgressionData contentProgressionData = m_PoiProgressionData.GetContentProgression(m_PoiData.pid);
		if(contentProgressionData.State != EContentProgressionState.Complete)
		{
			if(m_TourProgressionData.IsChallengeMode)
			{
				m_TourProgressionData.TourScore += GlobalSettingsManager.Instance.PointsEarnedContent;
				_popinPoints.Inflate(m_PoiData);
			}
			contentProgressionData.SetCompleted();
			PlayerManager.Instance.Player.Save();

			int progress = m_PoiProgressionData.GetPoiCurrentProgression();
			int numberofPois = m_PoiProgressionData.GetPoiMaxProgression();
			string remainingText = (numberofPois - progress) > 1 ? Wezit.Settings.Instance.GetSettingAsCleanedText(m_RemainingItemsPluralTextSettingKey) :
				Wezit.Settings.Instance.GetSettingAsCleanedText(m_RemainingItemsSingularTextSettingKey);
			_remainingItems.text = string.Format(remainingText, numberofPois - progress);
		}
	}
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{
	}
	#endregion Internals
	#endregion Methods
}