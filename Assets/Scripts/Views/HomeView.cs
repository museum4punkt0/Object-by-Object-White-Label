using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;
using UniRx;
using System.Collections;

public class HomeView : BaseView
{
	#region Fields
	#region Serialize Fields
	[SerializeField] private RawImage _colorBackground = null;
	[SerializeField] private RawImage _background = null;
	[Header("Explanation")]
	[SerializeField] private ExplanationWindow _explanationWindow = null;
	[SerializeField] private TextMeshProUGUI _introTitle = null;
	[SerializeField] private TextMeshProUGUI _introPart1 = null;
	[SerializeField] private RawImage _introImage = null;
	[SerializeField] private TextMeshProUGUI _introPart2 = null;

    [Space]
	[SerializeField] private Button _continueButton = null;
	[SerializeField] private Image _continueButtonBG = null;
	[SerializeField] private TextMeshProUGUI _continueButtonText = null;
	[SerializeField] KioskState _stateOnContinue = KioskState.NONE;
	[SerializeField] private Transform _contrastPanelRoot = null;
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private string _backgroundSettingKey = "template.spk.home.background.image";
	private string _introTitleSettingKey = "template.spk.home.intro.title.text";
	private string _introPart1SettingKey = "template.spk.home.intro.part1.text";
	private string _introImageSettingKey = "template.spk.home.intro.image";
	private string _introPart2SettingKey = "template.spk.home.intro.part2.text";
	private string _continueButtonTextSettingKey = "template.spk.home.intro.continue.button.text";
    #endregion Private m_Variables
    #endregion Fields

    #region Properties
    #endregion Properties

    #region Methods
    #region Public
    public override KioskState GetKioskState()
	{
		return KioskState.HOME;
	}

	public override void InitView()
	{
		ResetViewContent();
		SetInterfaceVisible(false);
	}

	public override void ShowView()
	{
		base.ShowView();
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Default);
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

		if(!PlayerManager.Instance.Player.HasSeenHomeScreen)
        {
			PlayerManager.Instance.Player.HasSeenHomeScreen = true;
			PlayerManager.Instance.Player.Save();
		}

		_colorBackground.color = _continueButtonBG.color = _introTitle.color = GlobalSettingsManager.Instance.AppColor;

        Wezit.Settings.Instance.SetImageFromSetting(_background, _backgroundSettingKey, language, "default", false);

        if (string.IsNullOrEmpty(Wezit.Settings.Instance.GetSetting(_introImageSettingKey, language)))
        {
			_introImage.transform.parent.gameObject.SetActive(false);
        }
		else
        {
			_introImage.transform.parent.gameObject.SetActive(true);
			Wezit.Settings.Instance.SetImageFromSetting(_introImage, _introImageSettingKey, language);
        }

		_continueButtonText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(_continueButtonTextSettingKey, language);
		if (_introTitle) _introTitle.text = Wezit.Settings.Instance.GetSettingAsCleanedText(_introTitleSettingKey, language);
        _introPart1.text = Wezit.Settings.Instance.GetSettingAsCleanedText(_introPart1SettingKey, language);
        _introPart2.text = Wezit.Settings.Instance.GetSettingAsCleanedText(_introPart2SettingKey, language);
        string[] paragraphs = { _introPart1.text, _introPart2.text };
		_explanationWindow.Inflate(_introTitle.text, paragraphs, _contrastPanelRoot);
	}

	private void ResetViewContent()
	{
		if (_background != null) ImageUtils.ResetImage(_background);
	}

	private void AddListeners()
	{
		RemoveListeners();
		_continueButton.onClick.AddListener(OnContinueButton);
	}

	private void RemoveListeners()
	{
		_continueButton.onClick.RemoveAllListeners();
	}

	private void OnContinueButton()
    {
		AppManager.Instance.GoToState(_stateOnContinue);
    }
	#endregion Private

	#region Internals
	protected override void OnFadeEndView()
	{

	}
	#endregion Internals
	#endregion Methods
}