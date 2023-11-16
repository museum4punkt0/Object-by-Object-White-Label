using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class ScratchView : BaseView
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

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private Language m_Language;
	private TourProgressionData m_TourProgressionData;
	private Wezit.ScratchAndReveal m_ScratchAndReveal;

	private string m_SelfieButtonTextSettingKey = "template.spk.pois.secret.selfie.button.text";
	private string m_InventoryButtonTextSettingKey = "template.spk.pois.secret.inventory.button.text";
	private string m_scratchInstructionSettingKey = "template.spk.pois.scratch.instruction.text";
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

		m_TourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);

		bool isChallenge = m_TourProgressionData.IsChallengeMode;
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventoryScore : MenuManager.MenuStatus.BackButtonInventory;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(KioskState.TOUR_MAP);
		m_PoiData = StoreAccessor.State.SelectedPoi;
		m_Language = language;

		LoadActivity();

		_colorBackground.color = _inventoryButtonBG.color = _selfieButtonBG.color = GlobalSettingsManager.Instance.AppColor;
		_selfieButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_SelfieButtonTextSettingKey, language);
		_inventoryButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_InventoryButtonTextSettingKey, language);

		_title.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_scratchInstructionSettingKey);
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
		AppManager.Instance.GoToState(KioskState.SELFIE);
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
            m_ScratchAndReveal =  (Wezit.ScratchAndReveal) Wezit.ActivityLoader.Instance.InstantiateActivity(await Wezit.ActivityLoader.LoadActivity(relation), m_Language, _scratchRoot);
			m_ScratchAndReveal.ActivityOver.AddListener(OnActivityOver);
        }
    }

	private void OnActivityOver()
	{
		m_TourProgressionData.TourScratchImagePath = m_ScratchAndReveal.GetBackgroundImagePath(m_Language);
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