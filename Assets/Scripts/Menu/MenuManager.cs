using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UniRx;

[System.Serializable]
public class UnityEventMenu : UnityEvent { };

public class MenuManager : Singleton<MenuManager>
{
	public enum MenuStatus
	{
		Default,
		Hidden,
		BackButton,
		BackButtonInventory,
		BackButtonInventoryScore,
		BackButtonLogo,
		RightImage,
		Darken,
	}

	#region Fields
	public static string TAG = "<color=blue>[MenuManager]</color>";

	[SerializeField] private Button _menuButton = null;
	[SerializeField] private Image _openButtonBG = null;
	[SerializeField] private OpenMenu _openMenu = null;
	[SerializeField] private GameObject _openMenuUIRoot = null;
	[SerializeField] private Button _inventoryButton = null;
	[SerializeField] private Button _backButton = null;
	[SerializeField] private Image _backButtonBG = null;
	[SerializeField] private TextMeshProUGUI _titleText = null;
	[SerializeField] private Button _scoreCounter = null;
	[SerializeField] private GameObject _uiRoot = null;
	[SerializeField] private RawImage _logo = null;
	[SerializeField] private TextMeshProUGUI _appName = null;
	[SerializeField] private RawImage _rightImage = null;
	[SerializeField] private GameObject _darken = null;
	[SerializeField] private Image _colorBG;
	[SerializeField] private Image _topColorBG;

	private KioskState m_BackButtonkioskState;
	private MenuStatus m_PreviousStatus;
	private MenuStatus m_CurrentStatus;

	private string m_LogoSettingKey = "template.spk.header.logo.image";
	private string m_AppNameSettingKey = "template.spk.header.name.text";
	private string m_RightImageSettingKey = "template.spk.header.rightImage";

	private IDisposable m_storeSubscription;
	private Language m_currentLanguage = Language.none;

	private bool m_IsLogo = false;
	#endregion Fields

	#region Methods
	#region MonoBehaviour
	private new void Awake()
	{
		base.Awake();
		SetMenuStatus(MenuStatus.Hidden);
		if (AppManager.Instance.loadingOver)
		{
			OnLoadingOver();
		}
		else
		{
			AppManager.Instance.onLoadingOver.AddListener(OnLoadingOver);
		}
		SetMenuStatus(MenuStatus.Hidden);
	}
	#endregion MonoBehaviour

	#region Public
	public void AddListeners()
	{
		RemoveListeners();
		_menuButton.onClick.AddListener(OnOpenMenu);
		_backButton.onClick.AddListener(OnBackButton);
		_inventoryButton.onClick.AddListener(OnInventoryButton);
		_scoreCounter.onClick.AddListener(OnInventoryButton);

		m_currentLanguage = StoreAccessor.State.Language;

		if (m_storeSubscription != null)
		{
			m_storeSubscription.Dispose();
		}
		m_storeSubscription = StoreAccessor.Subject.Subscribe((state) =>
		{
			OnStoreStateChanged(state);
		});
	}

	public void RemoveListeners()
	{
		_menuButton.onClick.RemoveAllListeners();
		_backButton.onClick.RemoveAllListeners();
		_inventoryButton.onClick.RemoveAllListeners();
	}

	public void SetMenuStatus(MenuStatus a_status)
	{
		if(m_CurrentStatus != a_status)
        {
			m_PreviousStatus = m_CurrentStatus;
			m_CurrentStatus = a_status;
        }

		_uiRoot.SetActive(a_status != MenuStatus.Hidden);
		if(a_status == MenuStatus.Darken)
        {
			_darken.SetActive(true);
        }
		else
        {
			_darken.SetActive(false);
			_rightImage.gameObject.SetActive(a_status == MenuStatus.RightImage);
			_logo.gameObject.SetActive(m_IsLogo && (a_status == MenuStatus.Default || a_status == MenuStatus.RightImage || a_status == MenuStatus.BackButtonLogo));
			_appName.gameObject.SetActive(!m_IsLogo && (a_status == MenuStatus.Default || a_status == MenuStatus.RightImage || a_status == MenuStatus.BackButtonLogo));

			_inventoryButton.gameObject.SetActive(a_status == MenuStatus.BackButtonInventory || a_status == MenuStatus.BackButtonInventoryScore);
			_scoreCounter.gameObject.SetActive(a_status == MenuStatus.BackButtonInventoryScore);

			_menuButton.gameObject.SetActive(a_status == MenuStatus.Default || a_status == MenuStatus.RightImage);
			_backButton.gameObject.SetActive(a_status == MenuStatus.BackButton || a_status == MenuStatus.BackButtonInventory || a_status == MenuStatus.BackButtonLogo || a_status == MenuStatus.BackButtonInventoryScore);
			_titleText.gameObject.SetActive(a_status == MenuStatus.BackButton || a_status == MenuStatus.BackButtonInventory || a_status == MenuStatus.BackButtonInventoryScore);
        }

		StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_uiRoot));
	}

	public void SetPreviousStatus()
    {
		SetMenuStatus(m_PreviousStatus);
    }

	public void SetBackButtonState(KioskState kioskState)
    {
		m_BackButtonkioskState = kioskState;
    }

	public void SetTitle(string title)
    {
		_titleText.text = title;
    }

	public void Init(Language language)
    {
		InitViewContentByLang(language);
    }
	#endregion Public

	#region Private
	private void InitViewContentByLang(Language language)
	{
		ResetViewContent();
		_backButtonBG.color = _openButtonBG.color = _colorBG.color = _topColorBG.color = GlobalSettingsManager.Instance.AppColor;

		if(!string.IsNullOrEmpty(Wezit.Settings.Instance.GetSetting(m_LogoSettingKey, language)))
        {
			Wezit.Settings.Instance.SetImageFromSetting(_logo, m_LogoSettingKey, language, WezitSourceTransformation.default_base, false);
			m_IsLogo = true;
        }
		else
        {
			_appName.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_AppNameSettingKey, language);
			m_IsLogo = false;
        }
		Wezit.Settings.Instance.SetImageFromSetting(_rightImage, m_RightImageSettingKey, language);
		_openMenu.InitViewContentByLang(language);
	}

	private void ResetViewContent()
	{
		_openMenuUIRoot.SetActive(false);
		_openMenu.gameObject.SetActive(true);
	}

	private void OnLoadingOver()
	{
		InitViewContentByLang(StoreAccessor.State.Language);
		AddListeners();
	}

	private void OnStoreStateChanged(State state)
	{
		if (state.Language != m_currentLanguage)
		{
			m_currentLanguage = state.Language;
			InitViewContentByLang(state.Language);
		}
	}

	private void OnBackButton()
	{
		AppManager.Instance.GoToState(m_BackButtonkioskState);
	}

	private void OnOpenMenu()
    {
		_openMenu.Open();
    }

	private void OnInventoryButton()
    {
		AppManager.Instance.GoToState(KioskState.INVENTORY);
    }
	#endregion Private
	#endregion Methods
}
