/**
 * Created by Willy
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;

public class LanguageSelecter : MonoBehaviour
{
	#region Fields
	public static string TAG = "<color=white>[LanguageSelecter]</color>";

	[SerializeField] private bool _initOnEnable = false;

	private HorizontalLayoutGroup horizontalLayoutGroup;
	private ToggleGroup toggleGroup;
	private IDisposable langSelectSubscription = null;
	private RectOffset buttonPadding;
	private Language selectedLanguage = Language.none;
	#endregion Fields

	#region Methods
	#region MonoBehaviour
	private void Awake()
	{
		horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
		buttonPadding = new RectOffset();
	}

	private void OnEnable()
	{
		if (_initOnEnable)
		{
			Init();
		}
	}

	private void OnDisable()
	{
		RemoveListeners();
	}
	#endregion MonoBehaviour

	#region Public
	public void Init()
	{
		RemoveListeners();

		InitButtons();
		SetupLabels();
		ForceTogglesByLang(ViewManager.Instance.CurrentLanguage);

		AddListeners();
	}

	public void AddListeners()
	{
		RemoveListeners();

		foreach (Transform child in transform)
		{
			if (child.GetComponent<Toggle>()) child.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleChanged);
		}

		// langSelectSubscription = StoreAccessor.Subject.Subscribe((state) =>
		// {
		// 	if (AppManager.Instance.loadingOver)
		// 	{
		// 		if (state.Language != selectedLanguage)
		// 		{
		// 			selectedLanguage = state.Language;
		// 			ForceTogglesByLang(selectedLanguage);
		// 			OnLanguageUpdate();
		// 		}
		// 	}
		// });
	}

	public void RemoveListeners()
	{
		foreach (Transform child in transform)
		{
			if (child.GetComponent<Toggle>()) child.GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleChanged);
		}

		// if (langSelectSubscription != null) langSelectSubscription.Dispose();
	}

	#endregion Public

	#region Private
	private void InitButtons()
	{
		toggleGroup = GetComponent<ToggleGroup>();
		toggleGroup.allowSwitchOff = false;

		foreach (Transform child in transform)
		{
			if (child.GetComponent<Toggle>())
			{
				child.GetComponent<Toggle>().interactable = AppConfig.Instance.ConfigModel.languageSelecterInteractive;
				child.GetComponent<Toggle>().group = toggleGroup;
				toggleGroup.RegisterToggle(child.GetComponent<Toggle>());
			}
		}
	}

	private void SetupLabels()
	{
		LanguageButton langButton = null;
		foreach (Transform child in transform)
		{
			langButton = child.GetComponent<LanguageButton>();
			if (langButton)
			{
				langButton.SetLabelTitle((String)LocalizationManager.Instance.GetLocalizedValue("language_button_" + child.gameObject.name).ToUpper());
				//langButton.SetLayoutGroupPadding(buttonPadding);
			}
			langButton = null;
		}

		StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(gameObject));
	}

	private void OnToggleChanged(bool toggleValue)
	{
		if (toggleValue == true)
		{
			SetLanguage((Language)Enum.Parse(typeof(Language), toggleGroup.ActiveToggles().First().gameObject.name));
		}
	}

	// private void OnLanguageUpdate()
	// {
	// 	SetupLabels();
	// }

	private void ForceTogglesByLang(Language targetLang)
	{
		foreach (Transform child in transform)
		{
			if (child.gameObject.name == targetLang.ToString())
			{
				if (child.GetComponent<Toggle>())
				{
					child.GetComponent<Toggle>().isOn = true;
					if (toggleGroup && (toggleGroup == child.GetComponent<Toggle>().group)) toggleGroup.NotifyToggleOn(child.GetComponent<Toggle>());
				}
				break;
			}
		}
	}

	private void SetLanguage(Language language)
	{
		if (language != ViewManager.Instance.CurrentLanguage)
		{
			StartCoroutine(AppManager.Instance.SetLanguage(language));
		}
	}
	#endregion Private
	#endregion Methods
}
