using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

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
		BackButtonLogo,
		RightImage,
		Darken,
	}

	#region Fields
	public static string TAG = "<color=blue>[MenuManager]</color>";

	[SerializeField] private Button _menuButton = null;
	[SerializeField] private OpenMenu _openMenu = null;
	[SerializeField] private Button _inventoryButton = null;
	[SerializeField] private Button _backButton = null;
	[SerializeField] private TextMeshProUGUI _titleText = null;
	[SerializeField] private Button _scoreCounter = null;
	[SerializeField] private GameObject _uiRoot = null;
	[SerializeField] private RawImage _logo = null;
	[SerializeField] private TextMeshProUGUI _appName = null;
	[SerializeField] private RawImage _rightImage = null;
	[SerializeField] private GameObject _darken = null;

	private KioskState m_BackButtonkioskState;
	private MenuStatus m_PreviousStatus;
	private MenuStatus m_CurrentStatus;

	private string m_LogoSettingKey = "template.spk.header.logo.image";
	private string m_AppNameSettingKey = "template.spk.header.name.text";
	private string m_RightImageSettingKey = "template.spk.header.rightImage";

	private bool m_IsLogo = false;
	#endregion Fields

	#region Methods
	#region MonoBehaviour
	private void Awake()
	{
		SetMenuStatus(MenuStatus.Hidden);
		if (AppManager.Instance.loadingOver)
		{
			OnLoadingOver();
		}
		else
		{
			AppManager.Instance.onLoadingOver.AddListener(OnLoadingOver);
		}
	}
	#endregion MonoBehaviour

	#region Public
	public void AddListeners()
	{
		RemoveListeners();
		_menuButton.onClick.AddListener(OnOpenMenu);
		_backButton.onClick.AddListener(OnBackButton);
		_inventoryButton.onClick.AddListener(OnInventoryButton);
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

			_inventoryButton.gameObject.SetActive(a_status == MenuStatus.BackButtonInventory);
			_scoreCounter.gameObject.SetActive(a_status == MenuStatus.BackButtonInventory);

			_menuButton.gameObject.SetActive(a_status == MenuStatus.Default || a_status == MenuStatus.RightImage);
			_backButton.gameObject.SetActive(a_status == MenuStatus.BackButton || a_status == MenuStatus.BackButtonInventory || a_status == MenuStatus.BackButtonLogo);
			_titleText.gameObject.SetActive(a_status == MenuStatus.BackButton || a_status == MenuStatus.BackButtonInventory);
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
	#endregion Public

	#region Private
	private void InitViewContentByLang(Language language)
	{
		ResetViewContent();
		if(!string.IsNullOrEmpty(Wezit.Settings.Instance.GetSetting(m_LogoSettingKey, language)))
        {
			Wezit.Settings.Instance.SetImageFromSetting(_logo, m_LogoSettingKey, language);
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
		_openMenu.gameObject.SetActive(false);
	}

	private void OnLoadingOver()
	{
		InitViewContentByLang(StoreAccessor.State.Language);
		AddListeners();
	}

	private void OnBackButton()
	{
		AppManager.Instance.GoToState(m_BackButtonkioskState);
	}

	private void OnOpenMenu()
    {
		_openMenu.gameObject.SetActive(true);
		_openMenu.Open();
    }

	private void OnInventoryButton()
    {
		AppManager.Instance.GoToState(KioskState.INVENTORY);
    }
	#endregion Private
	#endregion Methods
}
