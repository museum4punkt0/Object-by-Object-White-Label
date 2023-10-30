using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class ARView : BaseView
{
	#region Fields
	#region Serialize Fields
    [Header("UI")]
	[SerializeField] private RawImage _background;
	[SerializeField] private GameObject _qRCodeScannerRoot;
	[SerializeField] private Image _panIcon;
	[SerializeField] private GraphicFader _instructionGraphicFader;
	[SerializeField] private GameObject _flashOoooh;
	[SerializeField] private AudioSource _audio;
	[Header("AR")]
	[SerializeField] private ARItemRoot _arRootPrefab;
	[SerializeField] private Transform _itemsRoot;
	[SerializeField] private Camera _arCamera;
	[SerializeField] private ARManager _arManager;
	[SerializeField] private ARSession _aRSession;
    [Header("Quiz")]
	[SerializeField] private QuizManager _quizManager;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_poiData;
	private ARItemRoot m_debugItemRoot;

	private string m_instructionLifetimeSettingKey = "template.spk.pois.AR.content.ar.started.text.lifetime";
	private string m_audioSettingKey = "template.spk.pois.AR.content.ar.started.audio";
	private float m_instructionLifetime = 8;
    #endregion Private m_Variables
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    #region Public
    public override KioskState GetKioskState()
	{
		return KioskState.AR;
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
		base.HideView();
		RemoveListeners();
		ResetViewContent();
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
		
		m_poiData = StoreAccessor.State.SelectedPoi;

		TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
		bool isChallenge = tourProgressionData.IsChallengeMode;
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.TOUR_MAP);

		_panIcon.color = GlobalSettingsManager.Instance.AppColor;
		m_instructionLifetime = Wezit.Settings.Instance.GetSettingAsFloat(m_instructionLifetimeSettingKey);
		_instructionGraphicFader.FadeDelay = m_instructionLifetime;
		Wezit.Settings.Instance.SetAudioClipFromSetting(_audio, m_audioSettingKey);

		_quizManager.Inflate(isChallenge && !tourProgressionData.GetPoiProgression(m_poiData.pid).QuizCompleted);

		// Keep AR session going while the user did not go back to the map
		KioskState previousKioskState = ViewManager.Instance.PreviousKioskState;
		if (previousKioskState == KioskState.CONTENT || previousKioskState == KioskState.QUIZ)
		{
			_arManager.ToggleItemsRoot(true);
#if UNITY_EDITOR
			if(m_debugItemRoot != null)
            {
				m_debugItemRoot.Toggle(true);
            }
#endif
			return;
		}

		_aRSession.enabled = true;
		_arManager.Inflate(m_poiData);

		// Spawn AR items when on PC for testing purpose
#if UNITY_EDITOR
		PlayerManager.Instance.Player.SetPoiProgression(StoreAccessor.State.SelectedTour.pid, m_poiData.pid);
		m_debugItemRoot = Instantiate(_arRootPrefab, _itemsRoot);
        m_debugItemRoot.Inflate(m_poiData, _arCamera);
        m_debugItemRoot.ARItemClicked.AddListener(OnARItemClicked);
#endif
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);

		KioskState previousKioskState = ViewManager.Instance.PreviousKioskState;
		KioskState currentKioskState = ViewManager.Instance.CurrentKioskState;
		if (previousKioskState == KioskState.CONTENT || previousKioskState == KioskState.QUIZ || currentKioskState == KioskState.CONTENT || currentKioskState == KioskState.QUIZ)
		{
			_arManager.ToggleItemsRoot(false);
			_instructionGraphicFader.gameObject.SetActive(false);
#if UNITY_EDITOR
			if (m_debugItemRoot != null)
            {
				m_debugItemRoot.Toggle(false);
            }
#endif
			return;
        }

		_arManager.Reset();
        foreach (Transform child in _itemsRoot)
        {
            Destroy(child.gameObject);
        }
        _qRCodeScannerRoot.SetActive(true);
		_panIcon.gameObject.SetActive(false);
		_flashOoooh.SetActive(false);
		_instructionGraphicFader.gameObject.SetActive(false);
		_aRSession.enabled = false;
	}

	private void AddListeners()
	{
		RemoveListeners();
		_arManager.ImageFound.AddListener(OnImageFound);
		_arManager.ARItemClicked.AddListener(OnARItemClicked);
	}

	private void RemoveListeners()
	{
		_arManager.ImageFound.RemoveAllListeners();
		_arManager.ARItemClicked.RemoveAllListeners();
	}

	private void OnImageFound()
    {
		_qRCodeScannerRoot.SetActive(false);
		_panIcon.gameObject.SetActive(true);
		_flashOoooh.SetActive(true);
		_instructionGraphicFader.gameObject.SetActive(true);
		_audio.Play();
		PlayerManager.Instance.Player.SetPoiProgression(StoreAccessor.State.SelectedTour.pid, m_poiData.pid);
    }

	private void OnARItemClicked(Wezit.Poi poi)
    {
		AppManager.Instance.SelectPoi(poi);
		AppManager.Instance.GoToState(KioskState.CONTENT);
    }
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}