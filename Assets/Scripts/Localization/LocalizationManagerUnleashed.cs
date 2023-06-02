using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utils;

public class LocalizationManagerUnleashed : Singleton<LocalizationManagerUnleashed>
{
	#region Fields
	public static string TAG = "<color=magenta>[LocalizationManagerUnleashed]</color>";

	#region Serialize Fields
	#endregion Serialize Fields

	#region Public Variables
	#endregion Public Variables

	#region Private m_Variables
	private List<Language> m_LanguageList;
	private Dictionary<Language, Dictionary<string, string>> m_LocalizationDict; // key : Language (Language) // value : Dictionary<string, string> (key + value from file)
	private const string m_MissingTextString = "Localized text not found";
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region MonoBehaviour
	protected override void Awake()
	{
		base.Awake();

		m_LanguageList = new List<Language>();
		m_LocalizationDict = new Dictionary<Language, Dictionary<string, string>>();
	}
	#endregion MonoBehaviour

	#region Public
	public IEnumerator AddLanguageAndRegister(Language language, string fileName)
	{
		Dictionary<string, string> m_LocalizedText = new Dictionary<string, string>();
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
				m_LocalizedText.Add(loadedData.items[i].key, loadedData.items[i].value);


			m_LanguageList.Add(language);
			m_LocalizationDict.Add(language, m_LocalizedText);

			Debug.Log(TAG + "Data loaded for language : " + language + " -- dictionary contains: " + m_LocalizedText.Count + " entries");
		}
	}

	public string GetLocalizedValue(Language language, string key)
	{
		string result = m_MissingTextString;
		if (m_LocalizationDict != null && m_LocalizationDict.ContainsKey(language) && m_LocalizationDict[language].ContainsKey(key)) result = m_LocalizationDict[language][key];
		return result;
	}
	#endregion Public
	#endregion Methods

}