using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class SecretPoiView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private RawImage _background = null;
	[SerializeField] private Image _colorBG = null;
	[Header("Explanation")]
	[SerializeField] private ExplanationWindow _explanationWindow = null;
	[SerializeField] private TextMeshProUGUI _title = null;
	[SerializeField] private TextMeshProUGUI _description = null;
    [Space]
	[SerializeField] private Button _bonusButton = null;
	[SerializeField] private Image _bonusButtonBG = null;
	[SerializeField] private TextMeshProUGUI _bonusButtonText = null;
    [Space]
	[SerializeField] private Transform _contrastPanelRoot = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private Language m_Language;
	
	private string m_ContinueButtonTextSettingKey = "template.spk.pois.content.button.continue.text";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.SECRET_POI;
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

		TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
		bool isChallenge = tourProgressionData.IsChallengeMode;
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.TOUR_MAP);
		m_PoiData = StoreAccessor.State.SelectedPoi;
		m_Language = language;

		_colorBG.color = _bonusButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;
		_bonusButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ContinueButtonTextSettingKey, language);
		_bonusButton.gameObject.SetActive(tourProgressionData.HasBeenCompleted);

		ImageUtils.LoadImage(_background, this, m_PoiData);

		_title.text = StringUtils.CleanFromWezit(m_PoiData.title);
        _description.text = StringUtils.CleanFromWezit(m_PoiData.description);
		string[] paragraphs = { _description.text };
		_explanationWindow.Inflate(_title.text, paragraphs, _contrastPanelRoot);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_bonusButton.onClick.AddListener(OnBonus);
	}

	private void RemoveListeners()
	{
		_bonusButton.onClick.RemoveAllListeners();
	}

	private void OnBonus()
    {
		AppManager.Instance.GoToState(KioskState.SCRATCH);
    }
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}