﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using SimpleJSON;

public class CreditsView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private Image _colorBG;
	[SerializeField] private TextMeshProUGUI _titleText;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private string m_titleSettingKey = "template.spk.credits.title.text";
	private string m_creditsSettingKey = "template.spk.credits.credits.text";
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

	#region Private
	private void AddListeners()
	{
		RemoveListeners();
	}

	private void RemoveListeners()
	{
	}

	private void ResetViewContent()
	{
		if (_titleText) _titleText.text = "";
		if (_descriptionText) _descriptionText.text = "";
	}

	private void InitViewContentByLang(Language language)
	{
        ResetViewContent();

		MenuManager.Instance.SetBackButtonState(ViewManager.Instance.PreviousKioskState);
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.BackButtonLogo);

		if (_titleText) _titleText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_titleSettingKey, language);
        _descriptionText.text = Wezit.Settings.Instance.GetSettingAsTaggedText(m_creditsSettingKey, language);
	}
	#endregion Private
	#region Internals
	#endregion Internals
	#endregion Methods
}