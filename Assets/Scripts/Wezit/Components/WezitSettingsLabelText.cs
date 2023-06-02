using System;
using TMPro;
using UnityEngine;
using UniRx;

/// <summary>
/// Set the label text depending on a key from the Wezit Settings of the app. Change with the current language.
/// </summary>
public class WezitSettingsLabelText : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _text = null;
	[SerializeField] private string _settingsKey = "";
	private IDisposable _storeSubscription;
	private Language _currentLanguage = Language.none;

	private void Awake()
	{
		if (AppManager.Instance.loadingOver)
		{
			OnLoadingOver();
		}
		else
		{
			AppManager.Instance.onLoadingOver.AddListener(OnLoadingOver);
		}
	}

	private void OnStoreStateChanged(State state)
	{
		if (state.Language != _currentLanguage)
		{
			string label = Wezit.Settings.Instance.GetSetting(_settingsKey, state.Language);
			_text.text = label;

			_currentLanguage = state.Language;
		}
	}

	private void OnLoadingOver()
	{
		string label = Wezit.Settings.Instance.GetSetting(_settingsKey, StoreAccessor.State.Language);
		_text.text = label;

		_currentLanguage = StoreAccessor.State.Language;

		if (_storeSubscription != null)
		{
			_storeSubscription.Dispose();
		}
		_storeSubscription = StoreAccessor.Subject.Subscribe((state) =>
		{
			OnStoreStateChanged(state);
		});
	}
}