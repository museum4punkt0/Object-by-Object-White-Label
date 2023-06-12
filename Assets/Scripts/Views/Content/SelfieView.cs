using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class SelfieView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private RawImage _cameraImage;
	[SerializeField] private RawImage _bonusImage;
	[SerializeField] private Popin _popin;
    [Space]
	[SerializeField] private GameObject _UIRoot;
	[SerializeField] private Button _cameraButton;
	[SerializeField] private Image _cameraButtonBG;
    [Space]
	[SerializeField] private GameObject _buttonsRoot;
	[Space]
	[SerializeField] private Button _restartButton = null;
	[SerializeField] private Image _restartButtonBG = null;
	[SerializeField] private TextMeshProUGUI _restartButtonText = null;
	[Space]
	[SerializeField] private Button _shareButton = null;
	[SerializeField] private Image _shareButtonBG = null;
	[SerializeField] private TextMeshProUGUI _shareButtonText = null;
	[Space]
	[SerializeField] private Button _inventoryButton = null;
	[SerializeField] private Image _inventoryButtonBG = null;
	[SerializeField] private TextMeshProUGUI _inventoryButtonText = null;
	#endregion Serialize Fields

	#region Private m_Variables
	private Wezit.Poi m_PoiData;
	private TourProgressionData m_TourProgressionData;
	private WebCamTexture m_WebCamTexture;

	private string m_PicturePopinTitleSettingKey = "template.spk.pois.secret.picture.popin.title";
	private string m_PicturePopinDescriptionSettingKey = "template.spk.pois.secret.picture.popin.description";
	private string m_PicturePopinButtonTextSettingKey = "template.spk.pois.secret.picture.popin.confirm.button.text";
	private string m_RestartButtonTextSettingKey = "template.spk.pois.secret.picture.restart.button.text";

	private string m_ShareButtonTextSettingKey = "template.spk.pois.secret.picture.share.button.text";
	private string m_SharePopinTitleSettingKey = "template.spk.pois.secret.share.popin.title";
	private string m_SharePopinDescriptionSettingKey = "template.spk.pois.secret.share.popin.description";
	private string m_SharePopinButtonTextSettingKey = "template.spk.pois.secret.share.popin.confirm.button.text";

	private string m_InventoryButtonTextSettingKey = "template.spk.pois.secret.inventory.button.text";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.SELFIE;
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
		MenuManager.MenuStatus status = isChallenge ? MenuManager.MenuStatus.BackButtonInventory : MenuManager.MenuStatus.BackButton;
		MenuManager.Instance.SetMenuStatus(status);
		MenuManager.Instance.SetBackButtonState(ViewManager.Instance.PreviousKioskState == KioskState.INVENTORY ? KioskState.INVENTORY : KioskState.TOUR_MAP);
		m_PoiData = StoreAccessor.State.SelectedPoi;

		_cameraButtonBG.color = _restartButtonBG.color = _shareButtonBG.color = _inventoryButtonBG.color = GlobalSettingsManager.Instance.AppColor;
		_restartButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_RestartButtonTextSettingKey, language);
		_shareButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ShareButtonTextSettingKey, language);
		_inventoryButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_InventoryButtonTextSettingKey, language);

		StartCoroutine(LoadCamera());
		ImageUtils.LoadImage(_bonusImage, this, m_PoiData, Wezit.RelationName.REF_PICTURE, WezitSourceTransformation.original, false);
	}

	private void ResetViewContent()
	{
		_buttonsRoot.SetActive(false);
		_cameraButton.gameObject.SetActive(true);
		_UIRoot.SetActive(true);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_cameraButton.onClick.AddListener(OnCameraButton);
		_restartButton.onClick.AddListener(OnRestart);
		_shareButton.onClick.AddListener(OnShare);
		_inventoryButton.onClick.AddListener(OnInventory);
	}

	private void RemoveListeners()
	{
		_restartButton.onClick.RemoveAllListeners();
		_shareButton.onClick.RemoveAllListeners();
		_inventoryButton.onClick.RemoveAllListeners();
		_popin.PopinButtonClicked.RemoveAllListeners();
	}

	private void OnCameraButton()
    {
		m_WebCamTexture.Pause();
		_popin.Inflate(Wezit.Settings.Instance.GetSettingAsCleanedText(m_PicturePopinTitleSettingKey),
					   Wezit.Settings.Instance.GetSettingAsCleanedText(m_PicturePopinDescriptionSettingKey),
					   Wezit.Settings.Instance.GetSettingAsCleanedText(m_PicturePopinButtonTextSettingKey),
					   "", "main");
		_popin.PopinButtonClicked.RemoveAllListeners();
		_popin.PopinButtonClicked.AddListener(OnCameraPopinOkayButton);
    }

	private void OnCameraPopinOkayButton()
	{
		_popin.Close();
		StartCoroutine(TakeSelfie());
	}

	private IEnumerator TakeSelfie()
	{
		_UIRoot.SetActive(false);
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Hidden);
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(PlayerManager.SelfiesScreenshotPath, StoreAccessor.State.SelectedTour.title + ".png"));
		Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		screenshot.Apply();
		yield return null;
		m_TourProgressionData.TourSelfiePath = System.IO.Path.Combine(PlayerManager.SelfiesPath, StoreAccessor.State.SelectedTour.title + ".png");
        NativeGallery.SaveImageToGallery(screenshot, "spk", StoreAccessor.State.SelectedTour.title + ".png");
        MenuManager.Instance.SetPreviousStatus();
		_UIRoot.SetActive(true);

		_cameraButton.gameObject.SetActive(false);
		_buttonsRoot.SetActive(true);
	}

	private void OnRestart()
	{
		m_WebCamTexture.Play();
		_cameraButton.gameObject.SetActive(true);
		_buttonsRoot.SetActive(false);
	}

	private void OnShare()
	{
		_popin.Inflate(Wezit.Settings.Instance.GetSettingAsCleanedText(m_SharePopinTitleSettingKey),
					   Wezit.Settings.Instance.GetSettingAsCleanedText(m_SharePopinDescriptionSettingKey),
					   Wezit.Settings.Instance.GetSettingAsCleanedText(m_SharePopinButtonTextSettingKey),
					   "", "main");
		_popin.PopinButtonClicked.RemoveAllListeners();
		_popin.PopinButtonClicked.AddListener(OnSharePopinConfirm);
	}

	private void OnSharePopinConfirm()
    {
		_popin.Close();
	}

	private void OnInventory()
	{
		AppManager.Instance.GoToState(KioskState.INVENTORY);
	}

	private IEnumerator LoadCamera()
	{
		WebCamDevice[] devices = WebCamTexture.devices;

		foreach (WebCamDevice device in devices)
		{
			if (device.isFrontFacing)
			{
				m_WebCamTexture = new WebCamTexture(device.name);
				_cameraImage.texture = m_WebCamTexture;
			}
#if UNITY_EDITOR
			m_WebCamTexture = new WebCamTexture(device.name);
			_cameraImage.texture = m_WebCamTexture;
#endif
		}

		m_WebCamTexture.Play();
		while (!m_WebCamTexture.didUpdateThisFrame) yield return null;
		_cameraImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * m_WebCamTexture.width / (float)m_WebCamTexture.height, Screen.width);
	}
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}