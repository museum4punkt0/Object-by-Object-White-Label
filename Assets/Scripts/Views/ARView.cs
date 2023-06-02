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
	[SerializeField] private RawImage _colorBackground = null;
	[SerializeField] private RawImage _background = null;
	[SerializeField] private ARManager _arManager = null;
	[SerializeField] private ARSession _aRSession = null;
	[SerializeField] private GameObject _qRCodeScannerRoot = null;
	#endregion Serialize Fields
	[SerializeField] private ARItemRoot _arRootPrefab = null;
	[SerializeField] private Transform _itemsRoot;
	[SerializeField] private Camera _arCamera;

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
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

		if (ViewManager.Instance.PreviousKioskState == KioskState.CONTENT)
		{
			return;
		}
		_aRSession.enabled = true;
		m_PoiData = StoreAccessor.State.SelectedPoi;
		_arManager.Inflate(m_PoiData);

		// Spawn AR items when on PC for testing purpose
#if UNITY_EDITOR
		PlayerManager.Instance.Player.SetPoiProgression(StoreAccessor.State.SelectedTour.pid, m_PoiData.pid);
		ARItemRoot instance = Instantiate(_arRootPrefab, _itemsRoot);
        instance.Inflate(m_PoiData, _arCamera);
        instance.ARItemClicked.AddListener(OnARItemClicked);
#endif
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
		if(ViewManager.Instance.PreviousKioskState == KioskState.CONTENT)
        {
			return;
        }

        foreach (Transform child in _itemsRoot)
        {
            Destroy(child.gameObject);
        }
        _qRCodeScannerRoot.SetActive(true);
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
		PlayerManager.Instance.Player.SetPoiProgression(StoreAccessor.State.SelectedTour.pid, m_PoiData.pid);
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