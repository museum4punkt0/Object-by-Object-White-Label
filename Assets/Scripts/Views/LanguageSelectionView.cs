using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;

public class LanguageSelectionView : BaseView
{
	#region Fields

	#region Serialize Fields
	[SerializeField] private RawImage _rawImageBackground = null;
	[SerializeField] private RawImage _colorBackground = null;
	[SerializeField] private LanguageButton _languageButtonPrefab = null;
	[SerializeField] private Transform _languageButtonRoot = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private string m_BackgroundSettingKey = "template.spk.language.screen.background.image";
	private string m_LanguageSettingsKey = "template.spk.language.languages";
	private KioskState m_StateOnLanguageButton = KioskState.HOME;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public override KioskState GetKioskState()
	{
		return KioskState.LANGUAGE_SELECTION;
	}

	public override void InitView()
	{
		ResetViewContent();
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		base.ShowView();
		InitViewContent();
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
			InitViewContent();
		}
	}

	public void Dispose()
	{
		RemoveListeners();
	}
	#endregion Public

	#region Private
	private void InitViewContent()
	{
		ResetViewContent();
        _colorBackground.color = GlobalSettingsManager.Instance.AppColor;
        m_StateOnLanguageButton = PlayerManager.Instance.Player.HasSeenHomeScreen ? KioskState.GLOBAL_MAP : KioskState.HOME;

		// If no language has previously been selected or if the user accesses the language selection screen through the menu, display the language selection screen
		if ((ViewManager.Instance.PreviousKioskState != KioskState.NONE && ViewManager.Instance.PreviousKioskState != KioskState.SPLASH) || string.IsNullOrEmpty(PlayerManager.Instance.Player.Language))
        {
			SimpleJSON.JSONNode languageArray = Wezit.Settings.Instance.GetSettingArray(m_LanguageSettingsKey);
			if(languageArray.Count == 0)
			{
				OnLanguageButtonClicked(Language.de);
			}
			else if(languageArray.Count == 1)
			{
				OnLanguageButtonClicked(LanguageExtensions.From(languageArray[0]["language"]));
			}
			else
			{
				int numberOfActiveLanguage = 0;
				SimpleJSON.JSONNode lastActiveLanguage = null;
				foreach (SimpleJSON.JSONNode node in languageArray)
				{
					if (node["enableLanguage"])
					{
						numberOfActiveLanguage++;
						lastActiveLanguage = node;
						LanguageButton languageButtonInstance = Instantiate(_languageButtonPrefab, _languageButtonRoot);
						languageButtonInstance.Inflate(LanguageExtensions.From(node["language"]), node["button.text"], GlobalSettingsManager.Instance.AppColor);
						languageButtonInstance.LanguageButtonClicked.AddListener(OnLanguageButtonClicked);
					}
				}

				if (numberOfActiveLanguage > 1)
				{
					Wezit.Settings.Instance.SetImageFromSetting(_rawImageBackground, m_BackgroundSettingKey, Language.de);
				}
				else
				{
					OnLanguageButtonClicked(LanguageExtensions.From(lastActiveLanguage["language"]));
				}
			}
        }
		// Else, show the home page
		else
        {
			Language language = LanguageExtensions.From(PlayerManager.Instance.Player.Language);
			OnLanguageButtonClicked(language, false);
        }
	}

	private void ResetViewContent()
	{
		foreach(Transform child in _languageButtonRoot)
        {
			Destroy(child.gameObject);
        }
	}

	private void AddListeners()
	{
		RemoveListeners();
	}

	private void RemoveListeners()
	{

	}

	private void OnLanguageButtonClicked(Language language, bool save = true)
    {
		StartCoroutine(AppManager.Instance.SetLanguage(language));
		if(save && language.ToString() != PlayerManager.Instance.Player.Language)
        {
			PlayerManager.Instance.Player.SetLanguage(language);
        }
		AppManager.Instance.GoToState(m_StateOnLanguageButton);
    }

	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}