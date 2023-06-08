using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using Wezit;

public class InventoryPoiView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private TextMeshProUGUI _title;
	[SerializeField] private InventoryItemPoi _poiItemPrefab;
	[SerializeField] private Transform _poiItemsRoot;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Tour m_Tour;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.INVENTORY_POI;
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
		MenuManager.Instance.SetBackButtonState(KioskState.INVENTORY);

		m_Tour = StoreAccessor.State.SelectedTour;

		_title.color = GlobalSettingsManager.Instance.AppColor;
		_title.text = StringUtils.CleanFromWezit(m_Tour.title);
		foreach (Poi poi in m_Tour.childs)
		{
			if (poi.type == "secret" || poi.type == "bank")
			{
				continue;
			}
			else
			{
				InventoryItemPoi instance = Instantiate(_poiItemPrefab, _poiItemsRoot);
				instance.Inflate(poi);
			}
		}
		StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_poiItemsRoot.gameObject));
		StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_poiItemsRoot.gameObject));
		StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_poiItemsRoot.gameObject));
	}

	private void ResetViewContent()
	{
		for (int i = 1; i < _poiItemsRoot.childCount; i++)
		{
			Destroy(_poiItemsRoot.GetChild(i).gameObject);
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