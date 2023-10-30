using SimpleJSON;
using UniRx;
using UniRx.Async;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Wezit
{
	public class Settings : Singleton<Settings>
	{
		/*************************************************************/
		/*********************** PROPERTIES **************************/
		/*************************************************************/

		private static string TAG = "<color=red>[WezitSettings]</color>";

		private JSONNode settingsNode;

		/*************************************************************/
		/********************** GETTER / SETTER **********************/
		/*************************************************************/

		public JSONNode SettingsNode { get => settingsNode; }

		public JSONNode GetSettingArray(string arrayKey, Language language = Language.none)
		{
			language = language == Language.none ? StoreAccessor.State.Language : language;

			var value = settingsNode[language.ToString()][arrayKey];
			if (value == null)
			{
				value = settingsNode["default"][arrayKey];
			}

			return value;
		}

		public string GetSetting(string key, Language language = Language.none)
		{
			if (string.IsNullOrEmpty(key)) return null;

			language = language == Language.none ? StoreAccessor.State.Language : language;

			var value = settingsNode[language.ToString()][key];
			if (value == null)
			{
				value = settingsNode["default"][key];
			}
			return value;
		}

		public Color GetSettingAsColor(string key, Language language = Language.none)
		{
			string colorStr = GetSetting(key, language);
			if (string.IsNullOrEmpty(colorStr)) return Color.black;
			colorStr = colorStr.Substring(4, colorStr.Length - 5);
			string[] colorRgb = colorStr.Split(new Char[] { ',' });

			Color color = new Color(float.Parse(colorRgb[0]) / 255, float.Parse(colorRgb[1]) / 255, float.Parse(colorRgb[2]) / 255);
			return color;
		}

		public bool GetSettingAsBool(string key, Language language = Language.none)
		{
			language = language == Language.none ? StoreAccessor.State.Language : language;

			var value = settingsNode[language.ToString()][key];
			if (value == null)
			{
				value = settingsNode["default"][key];
			}
			return value;
		}

		public float GetSettingAsFloat(string key, Language language = Language.none)
		{
			language = language == Language.none ? StoreAccessor.State.Language : language;
			string value = GetSettingAsCleanedText(key, language);
			if (string.IsNullOrEmpty(value)) return 0;

			float.TryParse(value, 
						   System.Globalization.NumberStyles.AllowDecimalPoint,
						   new System.Globalization.CultureInfo("en-US"),
						   out float output);
			return output;
		}

		public string GetSettingAsCleanedText(string key, Language language = Language.none)
        {
			return StringUtils.CleanFromWezit(GetSetting(key, language));
        }

		public string GetSettingAsTaggedText(string key, Language language = Language.none)
        {
			return StringUtils.AddCustomTagsFromWezit(GetSettingAsCleanedText(key, language));
        }

		public WezitAssets.Asset GetSettingAsAsset(string key, Language language = Language.none)
		{
			language = language == Language.none ? StoreAccessor.State.Language : language;
			WezitAssets.Asset asset = AssetsLoader.GetAssetById(GetSettingAsCleanedText(key, language)?.Replace("wzasset://", ""));
			return asset;
		}

		public string GetSettingAsAssetSourceByTransformation(string key, Language language = Language.none, string transformation = "default")
		{
			language = language == Language.none ? StoreAccessor.State.Language : language;
			WezitAssets.Asset asset = GetSettingAsAsset(key, language);
			string source = "";
			if(asset != null)
            {
				source = asset.GetAssetSourceByTransformation(transformation);
            }
			return source;
        }

		public void SetImageFromSetting(RawImage rawImage, string key, Language language = Language.none, string transformation = "default", bool envelopeParent = true)
        {
			StartCoroutine(Utils.ImageUtils.SetImage(rawImage, GetSettingAsAssetSourceByTransformation(key, language, transformation), "", envelopeParent));
        }

		public async void SetAudioClipFromSetting(AudioSource audioSource, string key, Language language = Language.none, string transformation = "default", bool loop = false)
        {
			audioSource.clip = await AudioUtils.GetAudioClipFromSource(GetSettingAsAssetSourceByTransformation(key, language, transformation));
			audioSource.loop = loop;
        }

		/*************************************************************/
		/********************** PUBLIC METHODS ***********************/
		/*************************************************************/

		public async UniTask<bool> Init(bool online)
		{
			Debug.Log(TAG + " Init");
			settingsNode = await Wezit.FilesDownloader.GetSettings(online);
			return true;
		}
	}
} // End namespace Wezit
