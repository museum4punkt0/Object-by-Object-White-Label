using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class PoiIntroView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private RawImage _background = null;
	[Header("Explanation")]
	[SerializeField] private ExplanationWindow _explanationWindow = null;
	[SerializeField] private GameObject _textContainer = null;
	[SerializeField] private TextMeshProUGUI _title = null;
	[SerializeField] private TextMeshProUGUI _description = null;
    [Space]
	[SerializeField] private Button _scanButton = null;
	[SerializeField] private Image _scanButtonBG = null;
	[SerializeField] private TextMeshProUGUI _scanButtonText = null;
	[Space]
	[SerializeField] private Button _goButton = null;
	[SerializeField] private Image _goButtonBG = null;
	[SerializeField] private TextMeshProUGUI _goButtonText = null;
	[Space]
	[SerializeField] private Transform _contrastPanelRoot = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private Language m_Language;
	
	private string m_scanButtonTextSettingKey = "template.spk.tours.map.popups.QRCode.button.text";
	private string m_goButtonTextSettingKey = "template.spk.pois.intro.go.button.text";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.POI_INTRO;
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

		bool isChallenge = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).IsChallengeMode;
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.TOUR_MAP);
		m_PoiData = StoreAccessor.State.SelectedPoi;
		m_Language = language;

		_goButtonBG.color = _scanButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;
		_scanButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_scanButtonTextSettingKey, language);
		_goButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_goButtonTextSettingKey, language);

		ImageUtils.LoadImage(_background, this, m_PoiData);

		_title.text = m_PoiData.CleanedTitle;
        _description.text = m_PoiData.CleanedDescription;
		StartCoroutine(LayoutGroupRebuilder.Rebuild(_textContainer));
		string[] paragraphs = { _description.text };
		_explanationWindow.Inflate(_title.text, paragraphs, _contrastPanelRoot, await AudioUtils.GetAudioSource(m_PoiData));

		_goButton.gameObject.SetActive(m_PoiData != PlayerManager.Instance.LastPOIInRange && PlayerManager.Instance.IsGPSOn);
		_scanButton.gameObject.SetActive(true);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_scanButton.onClick.AddListener(OnScan);
		_goButton.onClick.AddListener(OnStart);
	}

	private void RemoveListeners()
	{
		_scanButton.onClick.RemoveAllListeners();
		_goButton.onClick.RemoveAllListeners();
	}

	private void OnStart()
	{
		AppManager.Instance.GoToState(KioskState.TOUR_MAP);
	}

	private void OnScan()
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