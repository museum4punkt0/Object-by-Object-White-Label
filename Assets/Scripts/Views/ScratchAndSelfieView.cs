using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class ScratchAndSelfieView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private TextMeshProUGUI _title = null;
	[SerializeField] private Transform _scratchRoot;
	[SerializeField] private Image _colorBackground;
	[Space]
	[SerializeField] private Button _selfieButton = null;
	[SerializeField] private Image _selfieButtonBG = null;
	[SerializeField] private TextMeshProUGUI _selfieButtonText = null;
	[Space]
	[SerializeField] private Button _inventoryButton = null;
	[SerializeField] private Image _inventoryButtonBG = null;
	[SerializeField] private TextMeshProUGUI _inventoryButtonText = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private Language m_Language;
	
	private string m_ScanButtonTextSettingKey = "template.spk.tours.map.popups.QRCode.button.text";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.SCRATCH;
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

		bool isChallenge = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).IsChallengeMode;
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventory : MenuManager.MenuStatus.BackButton;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.TOUR_MAP);
		m_PoiData = StoreAccessor.State.SelectedPoi;
		m_Language = language;

		LoadActivity();

		_colorBackground.color = _inventoryButtonBG.color = _selfieButtonBG.color = GlobalSettingsManager.Instance.AppColor;
		_selfieButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ScanButtonTextSettingKey, language);

		_title.text = StringUtils.CleanFromWezit(m_PoiData.title);
	}

	private void ResetViewContent()
	{
        foreach (Transform child in _scratchRoot)
        {
			Destroy(child.gameObject);
        }
		_selfieButton.gameObject.SetActive(false);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_selfieButton.onClick.AddListener(OnSelfie);
		_inventoryButton.onClick.AddListener(OnInventory);
	}

	private void RemoveListeners()
	{
		_selfieButton.onClick.RemoveAllListeners();
		_inventoryButton.onClick.RemoveAllListeners();
	}

	private void OnSelfie()
    {
		AppManager.Instance.GoToState(KioskState.TOUR_MAP);
	}

	private void OnInventory()
	{
		AppManager.Instance.GoToState(KioskState.INVENTORY);
	}

	private async void LoadActivity()
    {
        Wezit.Relation relation = await Wezit.ActivityLoader.LookForActivity(m_PoiData);
        if (relation != null)
        {
            Wezit.Activity scratchActivity =  Wezit.ActivityLoader.Instance.InstantiateActivity(await Wezit.ActivityLoader.LoadActivity(relation), m_Language, _scratchRoot);
			scratchActivity.ActivityOver.AddListener(OnActivityOver);
        }
    }

	private void OnActivityOver()
    {
		_selfieButton.gameObject.SetActive(true);
	}
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}