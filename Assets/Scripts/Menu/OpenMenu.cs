using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class OpenMenu : MonoBehaviour
{
	#region Fields
	#region SerializeFields
	[SerializeField] private Transform _menuRoot = null;
	[SerializeField] private GameObject _uiRoot;
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Button _darkenCloseButton = null;
	[SerializeField] private MenuLink _menuLinkPrefab = null;
	[SerializeField] private Transform _menuLinksRoot = null;
	[SerializeField] private RawImage _logo = null;
	[SerializeField] private TextMeshProUGUI _appName = null;
	[Space]
	[SerializeField] private Popin _resetPopin = null;
	[SerializeField] private Button _resetPopinConfirmButton = null;
	[SerializeField] private Image _resetPopinConfirmButtonBG = null;
	[SerializeField] private TextMeshProUGUI _resetPopinConfirmButtonText = null;
	#endregion

	#region Private
	private string m_logoSettingKey = "template.spk.header.logo.image";
	private string m_appNameSettingKey = "template.spk.header.name.text";
	private string m_menuLinksArraySettingKey = "template.spk.menu.links";

	private string m_resetPopinTitleSettingKey = "template.spk.reset.title";
	private string m_resetPopinDescriptionSettingKey = "template.spk.reset.description";
	private string m_resetPopinCancelSettingKey = "template.spk.reset.button.cancel.text";
	private string m_resetPopinConfirmSettingKey = "template.spk.reset.button.confirm.text";
	private RectTransform m_MenuRecttransform;
	#endregion
	#endregion Fields

	#region Methods
	#region MonoBehaviour
	private void OnDisable()
    {
        _menuRoot.localPosition = Vector3.left * _menuRoot.GetComponent<RectTransform>().sizeDelta.x;
	}
    #endregion MonoBehaviour

    #region Public
    public void Open()
    {
		_uiRoot.SetActive(true);
		StartCoroutine(SlideOpen());
    }

	public void InitViewContentByLang(Language lang)
    {
		ResetViewContent();
		m_MenuRecttransform = _menuRoot.GetComponent<RectTransform>();
		
		if (!string.IsNullOrEmpty(Wezit.Settings.Instance.GetSetting(m_logoSettingKey, lang)))
		{
			Wezit.Settings.Instance.SetImageFromSetting(_logo, m_logoSettingKey, lang, WezitSourceTransformation.original, false);
			_logo.gameObject.SetActive(true);
			_appName.gameObject.SetActive(false);
		}
		else
		{
			_appName.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_appNameSettingKey, lang);
			_logo.gameObject.SetActive(false);
			_appName.gameObject.SetActive(true);
		}
		
		Wezit.Settings.Instance.SetImageFromSetting(_logo, m_logoSettingKey, lang, WezitSourceTransformation.original, false);
		SimpleJSON.JSONNode linksArray = Wezit.Settings.Instance.GetSettingArray(m_menuLinksArraySettingKey, lang);
		
		foreach(SimpleJSON.JSONNode node in linksArray)
        {
            if (node["enable"])
			{
				KioskState state = (KioskState)Enum.Parse(typeof(KioskState), node["corresponding_view"]);
				MenuLink instance = Instantiate(_menuLinkPrefab, _menuLinksRoot);
				instance.Inflate(node["link.text"], state, node["addSeparator"]);
				instance.MenuLinkClicked.AddListener(OnMenuLinkClicked);
            }
        }

		_resetPopin.Inflate(Wezit.Settings.Instance.GetSettingAsCleanedText(m_resetPopinTitleSettingKey),
							Wezit.Settings.Instance.GetSettingAsCleanedText(m_resetPopinDescriptionSettingKey),
							Wezit.Settings.Instance.GetSettingAsCleanedText(m_resetPopinCancelSettingKey));
		_resetPopinConfirmButtonBG.color = GlobalSettingsManager.Instance.AppColor;
		_resetPopinConfirmButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_resetPopinConfirmSettingKey);
		_resetPopin.Close();

		AddListeners();
	}

    public void AddListeners()
	{
		RemoveListeners();
		_closeButton.onClick.AddListener(Close);
		_darkenCloseButton.onClick.AddListener(Close);
		_resetPopin.PopinButtonClicked.AddListener(_resetPopin.Close);
		_resetPopinConfirmButton.onClick.AddListener(OnResetConfirm);
	}

	public void RemoveListeners()
	{
		_closeButton.onClick.RemoveAllListeners();
		_darkenCloseButton.onClick.RemoveAllListeners();
		_resetPopin.PopinButtonClicked.RemoveAllListeners();
		_resetPopinConfirmButton.onClick.RemoveAllListeners();
	}
	#endregion Public

	#region Private
	private void ResetViewContent()
	{
		Close();
		foreach(Transform child in _menuLinksRoot)
        {
			Destroy(child.gameObject);
        }
	}

	private void OnMenuLinkClicked(KioskState state)
    {
		if (state == KioskState.RESET)
        {
			_resetPopin.Open();
		}
		else
        {
			AppManager.Instance.GoToState(state);
			Close();
        }
    }

	private IEnumerator SlideOpen()
    {
		float width = m_MenuRecttransform.sizeDelta.x;
		m_MenuRecttransform.anchoredPosition = width * Vector2.left;
		while(m_MenuRecttransform.anchoredPosition.x < 0)
        {
			m_MenuRecttransform.Translate(Mathf.Min(100, -m_MenuRecttransform.anchoredPosition.x), 0, 0);
			yield return null;
        }
		m_MenuRecttransform.anchoredPosition = Vector2.zero;
    }

	private void Close()
    {
		StartCoroutine(SlideClosed());
    }

	private IEnumerator SlideClosed(bool instantly = false)
	{
		float width = m_MenuRecttransform.sizeDelta.x;
		while (m_MenuRecttransform.anchoredPosition.x > -width || instantly)
		{
			m_MenuRecttransform.Translate(-100, 0, 0);
			yield return null;
		}
		_uiRoot.SetActive(false);
	}

	private void OnResetConfirm()
    {
		PlayerManager.Instance.DeleteSave();
		_resetPopin.Close();
    }
	#endregion Private
	#endregion Methods
}
