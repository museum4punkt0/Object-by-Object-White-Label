using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventAppManagerLoading : UnityEvent { };

public class AppManager : Singleton<AppManager>
{
	/*************************************************************/
	/*********************** PROPERTIES **************************/
	/*************************************************************/

	private static string TAG = "<color=red>[AppManager]</color>";

	public bool loadingOver = false;
	public UnityEventAppManagerLoading onLoadingOver;

	private bool onStandByCompleteFlag = false;

	protected AppManager()
	{
	}

	/*************************************************************/
	/********************** PUBLIC METHODS ***********************/
	/*************************************************************/

	public void Init()
	{
		Debug.Log(TAG + " Init");
		loadingOver = false;

		PoiStore.Init();
		PoiLocationStore.Init();
		Poi3DPositionStore.Init();
		CategoryStore.Init();

		ApplyConfigParams();
		SetupStandByManager();
		SetupLocalizationManager();
		SetupEventManager();
		StartApp();
	}

	public IEnumerator SetLanguage(Language language, bool force = false)
	{
		if (language != StoreAccessor.State.Language || force)
		{
			Debug.Log(TAG + " SetLanguage - " + language);
			StoreAccessor.Dispatch(Store.Kiosk.ActionCreator.SetLanguage(language));
			yield return null; // force 'one frame waiting' to apply language changes
		}
	}

	/*************************************************************/
	/******************** INTERNAL METHODS ***********************/
	/*************************************************************/

	internal void GoToState(KioskState kioskState)
	{
		if (StoreAccessor.State.KioskState != kioskState) StoreAccessor.Dispatch(Store.Kiosk.ActionCreator.SetState(kioskState));
	}

	internal void SelectPoi(Wezit.Poi _wzPoi)
	{
		DispatchPoiSelection(_wzPoi);
	}

	internal void UnselectPoi()
	{
		DispatchPoiSelection(null);
	}

	internal void GoToHome()
	{
		UnselectPoi();
		GoToState(KioskState.HOME);
	}

	internal IEnumerator GoToHome(bool forceLanguage = false)
	{
		UnselectPoi();
		yield return SetLanguage((Language)Enum.Parse(typeof(Language), AppConfig.Instance.ConfigModel.defaultLanguage), forceLanguage);
		GoToState(KioskState.HOME);
	}

	internal IEnumerator GoToHome(Language language, bool forceLanguage = false)
	{
		UnselectPoi();
		yield return SetLanguage(language, forceLanguage);
		GoToState(KioskState.HOME);
	}

	/*************************************************************/
	/********************* PRIVATE METHODS ***********************/
	/*************************************************************/

	protected override void Awake()
	{
		base.Awake();
		onLoadingOver = new UnityEventAppManagerLoading();
	}

	private void Start()
	{
	}

	private void SetupStandByManager()
	{
		gameObject.AddComponent<StandByManager>();

		float standByDelay = 0;
		if(AppConfig.Instance.ConfigModel.loadWezit) standByDelay = Wezit.Settings.Instance.GetSettingAsFloat(AppConfig.Instance.ConfigModel.standByDelaySettingKey, Language.fr_FR);
		Debug.Log(TAG + " SetupStandByManager - standByDelay :: " + standByDelay);
		if (standByDelay > 0)
		{
			StandByManager.instance.Init(standByDelay, 0.25f);
			StandByManager.instance.onStandByComplete.AddListener(OnStandByComplete);
			StandByManager.instance.onStandByReset.AddListener(OnStandByReset);
			StandByManager.instance.Begin();
		}
	}

	private void SetupLocalizationManager()
	{
		gameObject.AddComponent<LocalizationManager>();
	}

	private void SetupEventManager()
	{
		gameObject.AddComponent<EventManager>();

		Debug.Log(TAG + " SetupEventManager ");
		EventManager.Instance.Init();
	}

	private void ApplyConfigParams()
	{
		Cursor.visible = AppConfig.Instance.ConfigModel.cursorVisible;

		if (AppConfig.Instance.ConfigModel.targetFrameRate > -1)
			Application.targetFrameRate = AppConfig.Instance.ConfigModel.targetFrameRate;

		ForceResolutionConfigParams();

		if (AppConfig.Instance.ConfigModel.resolutionSettings.force && AppConfig.Instance.ConfigModel.resolutionSettings.checkChanges) StartCoroutine(CheckResolutionCoroutine());
	}

	private void ForceResolutionConfigParams()
	{
		if (AppConfig.Instance.ConfigModel.resolutionSettings.force)
		{
			Screen.SetResolution(AppConfig.Instance.ConfigModel.resolutionSettings.targetWidth, AppConfig.Instance.ConfigModel.resolutionSettings.targetHeight, AppConfig.Instance.ConfigModel.resolutionSettings.fullscreen);
		}
	}

	private IEnumerator CheckResolutionCoroutine()
	{
		while (true)
		{
			yield return StartCoroutine(Utils.FrameUtils.WaitForFrames(30));

			if ((Screen.width != AppConfig.Instance.ConfigModel.resolutionSettings.targetWidth) || (Screen.height != AppConfig.Instance.ConfigModel.resolutionSettings.targetHeight) || (Screen.fullScreen != AppConfig.Instance.ConfigModel.resolutionSettings.fullscreen))
			{
				ForceResolutionConfigParams();
			}
		}
	}

	private void StartApp()
	{
		loadingOver = true;
		onLoadingOver.Invoke();

		GoToState(KioskState.LANGUAGE_SELECTION);
	}

	private void DispatchPoiSelection(Wezit.Poi _wzPoi)
	{
		StoreAccessor.Dispatch(Store.SelectedPoi.ActionCreator.Set(_wzPoi));
	}

	private void OnStandByComplete()
	{
		Debug.Log(TAG + "OnStandByComplete");

		if (!onStandByCompleteFlag)
		{
			onStandByCompleteFlag = true;

			StartCoroutine(GoToHome(false));
		}
	}

	private void OnStandByReset()
	{
		// Debug.Log(TAG + "OnStandByReset");
		onStandByCompleteFlag = false;
	}

	~AppManager()
	{

	}
}
