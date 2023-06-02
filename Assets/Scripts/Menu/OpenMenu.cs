using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Events;
using System.Collections;

public class OpenMenu : MonoBehaviour
{
	#region Fields
	#region SerializeFields
	[SerializeField] private Transform _menuRoot = null;
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Button _darkenCloseButton = null;
	[SerializeField] private MenuLink _menuLinkPrefab = null;
	[SerializeField] private Transform _menuLinksRoot = null;
	[SerializeField] private RawImage _logo = null;
	#endregion

	#region Private
	private string m_LogoSettingKey = "template.spk.header.logo.image";
	private string m_MenuLinksArraySettingKey = "template.spk.menu.links";
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
		StartCoroutine(SlideOpen());
    }

	public void InitViewContentByLang(Language lang)
    {
		ResetViewContent();
		m_MenuRecttransform = _menuRoot.GetComponent<RectTransform>();
		Wezit.Settings.Instance.SetImageFromSetting(_logo, m_LogoSettingKey, lang);
		SimpleJSON.JSONNode linksArray = Wezit.Settings.Instance.GetSettingArray(m_MenuLinksArraySettingKey, lang);
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
		AddListeners();
	}

    public void AddListeners()
	{
		RemoveListeners();
		_closeButton.onClick.AddListener(Close);
		_darkenCloseButton.onClick.AddListener(Close);
	}

	public void RemoveListeners()
	{
		_closeButton.onClick.RemoveAllListeners();
		_darkenCloseButton.onClick.RemoveAllListeners();
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
		AppManager.Instance.GoToState(state);
		Close();
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
		gameObject.SetActive(false);
	}
	#endregion Private
	#endregion Methods
}
