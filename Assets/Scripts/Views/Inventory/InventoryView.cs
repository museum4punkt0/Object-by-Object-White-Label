using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class InventoryView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private InventoryItemTour _tourPrefab = null;
	[SerializeField] private Transform _toursRoot = null;
	[SerializeField] private TextMeshProUGUI _title = null;
	#endregion Serialize Fields
	#region Private m_Variables
	private List<Wezit.Tour> m_Tours = new List<Wezit.Tour>();

	private string m_InventoryTitleSettingKey = "";
	private KioskState m_previousKioskState;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.INVENTORY;
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
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButtonLogo);
		MenuManager.Instance.SetBackButtonState(PlayerManager.Instance.ViewOnInventoryBackButton);

		_title.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_InventoryTitleSettingKey, language);
		_title.color = GlobalSettingsManager.Instance.AppColor;

		m_Tours = WezitDataUtils.GetWezitToursByLang(language);
		foreach (Wezit.Tour tour in m_Tours)
		{
			Instantiate(_tourPrefab, _toursRoot).Inflate(tour);
		}
		StartCoroutine(LayoutGroupRebuilder.Rebuild(_toursRoot.gameObject));
	}

	private void ResetViewContent()
	{
		for (int i = 1; i < _toursRoot.childCount; i++)
		{
			Destroy(_toursRoot.GetChild(i).gameObject);
		}
	}

	private void AddListeners()
	{
		RemoveListeners();
	}

	private void RemoveListeners()
	{
	}
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}