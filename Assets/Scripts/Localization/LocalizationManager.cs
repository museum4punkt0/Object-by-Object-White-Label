using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utils;

public class LocalizationManager : Singleton<LocalizationManager>
{
	#region Fields
	public static string TAG = "<color=magenta>[LocalizationManager]</color>";

	private Language _currentLanguageLoaded = Language.none;
	private bool _isReady = false;
	private Dictionary<string, string> _localizedText;
	private const string _missingTextString = "Localized text not found";
	#endregion Fields

	#region Methods
	#region Public
	public bool IsReady { get => _isReady; }
	public Language CurrentLanguageLoaded { get => _currentLanguageLoaded; }

	public IEnumerator LoadLocalizedText(string fileName, Language language)
	{
		_isReady = false;

		_localizedText = new Dictionary<string, string>();
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
		string dataJsonString = null;

		yield return FileUtils.RequestTextContent(filePath, 5);

		if (string.IsNullOrEmpty(dataJsonString))
		{
			Debug.LogError(TAG + "Cannot find file!");
		}
		else
		{
			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataJsonString);

			for (int i = 0; i < loadedData.items.Length; i++)
				_localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);

			_currentLanguageLoaded = language;
			Debug.Log(TAG + "Data loaded, dictionary contains: " + _localizedText.Count + " entries");
		}

		_isReady = true;
	}

	public string GetLocalizedValue(string key)
	{
		string result = _missingTextString;
		if (_localizedText != null && _localizedText.ContainsKey(key))
		{
			result = _localizedText[key];
		}

		return result;
	}
	#endregion Public

	#region MonoBehaviour
	protected override void Awake()
	{
		base.Awake();

		_isReady = false;
		_currentLanguageLoaded = Language.none;
	}
	#endregion MonoBehaviour
	#endregion Methods

}