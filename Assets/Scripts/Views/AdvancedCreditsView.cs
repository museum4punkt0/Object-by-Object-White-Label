using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class AdvancedCreditsView : BaseView
{
	#region Subclasses
	[Serializable]
	private class CreditsPanel
	{
		public TextMeshProUGUI titleText = null;
		public TextMeshProUGUI descriptionText = null;

		public void ResetContent()
		{
			if (titleText != null)
			{
				titleText.text = "";
			}

			if (descriptionText != null)
			{
				descriptionText.text = "";
			}
		}
	}
	#endregion Subclasses

	#region Fields
	public static string TAG = "<color=orange>[CreditsManager]</color>";
	public static string WEZIT_TAG = "credits";

	#region Serialize Fields
	[SerializeField] private Image _uIBackground = null;
	[SerializeField] private CreditsPanel[] _panels = null;
	[SerializeField] private Button _closeButton = null;
	// [SerializeField] private CanvasGroupFader _faderOut;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private Wezit.Poi m_WzPoiData = null;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.CREDITS;
	}

	public override void InitView()
	{
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		if (_uIBackground != null) _uIBackground.color = ColorsUtils.GetColorByHtmlString(AppConfig.Instance.ConfigModel.backgroundColor);

		InitViewContentByLang(ViewManager.Instance.CurrentLanguage);

		base.ShowView();

		AddListeners();
	}

	public override void HideView()
	{
		RemoveListeners();

		base.HideView();
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

	#region MonoBehavior
	#endregion MonoBehavior

	#region Internals
	protected override void OnFadeEndView() { }
	#endregion Internals

	#region Private
	private void AddListeners()
	{
		RemoveListeners();

		if (_closeButton) _closeButton.onClick.AddListener(OnCloseButton);
		// if (_faderOut) _faderOut.OnFadeEnd.AddListener(OnFadeOutEnd);
	}

	private void RemoveListeners()
	{
		if (_closeButton) _closeButton.onClick.RemoveAllListeners();
		// if (_faderOut) _faderOut.OnFadeEnd.RemoveAllListeners();
	}

	private void OnFadeOutEnd()
	{
		HideView();
	}

	private void OnCloseButton()
	{
		// if (_faderOut) _faderOut.StartFading();
		ViewManager.Instance.GoBack();
	}

	private void ResetViewContent()
	{
		m_WzPoiData = null;

		foreach (CreditsPanel panel in _panels)
		{
			panel.ResetContent();
		}
	}

	private void InitViewContentByLang(Language language)
	{
		ResetViewContent();
		m_WzPoiData = WezitDataUtils.GetWezitPoiByTag(language, WEZIT_TAG);

		if (m_WzPoiData == null) return;

		if (m_WzPoiData.childs.Count > 0)
		{
			for (int i = 0; i < m_WzPoiData.childs.Count; i++)
			{
				if (i < _panels.Length)
				{
					_panels[i].titleText.text = StringUtils.CleanFromWezit(m_WzPoiData.childs[i].title);
					_panels[i].descriptionText.text = StringUtils.RemoveUnderlineTag(StringUtils.CleanFromWezit(m_WzPoiData.childs[i].description));
				}
			}
		}
		else
		{
			if (_panels.Length > 0)
			{
				_panels[0].titleText.text = StringUtils.CleanFromWezit(m_WzPoiData.title);
				_panels[0].descriptionText.text = StringUtils.RemoveUnderlineTag(StringUtils.CleanFromWezit(m_WzPoiData.description));
			}
		}
	}
	#endregion Private
	#endregion Methods
}