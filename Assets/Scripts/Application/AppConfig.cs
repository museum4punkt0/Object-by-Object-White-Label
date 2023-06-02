using SimpleJSON;
using UniRx;
using UniRx.Async;
using System.IO;
using UnityEngine;

public class AppConfig : Singleton<AppConfig>
{
	/*************************************************************/
	/*********************** PROPERTIES **************************/
	/*************************************************************/

	private static string TAG = "<color=red>[AppConfig]</color>";
	private static string CONFIG_FILE_NAME = "config.json";

	private AppConfigModel configModel;

	/*************************************************************/
	/********************** GETTER / SETTER **********************/
	/*************************************************************/

	public AppConfigModel ConfigModel { get => configModel; }

	/*************************************************************/
	/********************** PUBLIC METHODS ***********************/
	/*************************************************************/

	public async UniTask<bool> Init()
	{
		Debug.Log(TAG + " Init");
		JSONNode result = await LoadConfigFile();
		configModel = JsonUtility.FromJson<AppConfigModel>(result["app"].ToString());

		Wezit.Config.Instance.Init(result);
		Unity.FileDebugConfig.Instance.Init(result);
		return true;
	}

	public bool ShowVersion()
	{
		return Wezit.Settings.Instance.GetSettingAsBool(AppConfig.Instance.ConfigModel.showVersionSettingKey, Language.fr_FR);
	}

	/*************************************************************/
	/********************* PRIVATE METHODS ***********************/
	/*************************************************************/

	private async UniTask<JSONNode> LoadConfigFile()
	{
		string configJsonUrl = Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_NAME);
		string configJsonString = await Utils.FileUtils.RequestTextContent(configJsonUrl, 5);

		if (string.IsNullOrEmpty(configJsonString))
		{
			Debug.LogError(TAG + "Can not load configuration file");
			return null;
		}
		else
		{
			Debug.Log(TAG + "ConfigFile loaded");
			return SimpleJSON.JSON.Parse(configJsonString);
		}
	}
}
