using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ImageViewerView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Image _colorBackground = null;
	[SerializeField] private RawImage _background = null;
	[SerializeField] private PinchableScrollRect _pinchableScrollRect = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private TourProgressionData m_TourProgressionData;
	private PoiProgressionData m_PoiProgressionData;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.IMAGE_VIEWER;
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
	private void InitViewContentByLang(Language language)
	{
		ResetViewContent();

		m_TourProgressionData = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid);
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButton);
		MenuManager.Instance.SetBackButtonState(ViewManager.Instance.PreviousKioskState);

		StartCoroutine(ImageUtils.SetImage(_background, StoreAccessor.State.ImageViewerImageSource));

		_colorBackground.color = GlobalSettingsManager.Instance.AppColor;

		_pinchableScrollRect.Init(true);
		ImageUtils.LoadImage(_background, this, m_PoiData, Wezit.RelationName.SHOW_PICTURE, WezitSourceTransformation.default_base, false);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
		_background.gameObject.SetActive(true);
		_colorBackground.enabled = true;
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