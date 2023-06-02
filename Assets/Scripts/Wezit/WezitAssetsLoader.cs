using UniRx;
using UniRx.Async;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Wezit
{
	public class AssetsLoader
	{
		/*************************************************************/
		/*********************** PROPERTIES **************************/
		/*************************************************************/

		private static string TAG = "<color=red>[WezitManifest]</color>";

		private static List<WezitAssets.Asset> m_Assets;

		/*************************************************************/
		/********************** GETTER / SETTER **********************/
		/*************************************************************/

		public static List<WezitAssets.Asset> Assets { get => m_Assets; }

		public static WezitAssets.Asset GetAssetById(string assetPid)
        {
			return m_Assets.Find(x => x.pid == assetPid);
        }

		/*************************************************************/
		/********************** PUBLIC METHODS ***********************/
		/*************************************************************/

		public static async UniTask<bool> Init(bool online)
		{
			Debug.Log(TAG + " Init");
#if UNITY_WEBGL && !UNITY_EDITOR
			manifest = await LoadWezitManifest(manifestUrl);
#else
			m_Assets = await Wezit.FilesDownloader.GetAssets(online);
#endif
            return true;
		}

		/*************************************************************/
		/********************* PRIVATE METHODS ***********************/
		/*************************************************************/

		private static async UniTask<List<WezitAssets.Asset>> LoadWezitAssets()
		{
			// Store manifest URL

			string manifestJsonString = await Utils.FileUtils.RequestTextContent(ManifestLoader.AssetsUrl, 5);

			if (string.IsNullOrEmpty(manifestJsonString))
			{
				Debug.LogError(TAG + "Can not load assets file from " + ManifestLoader.AssetsUrl);
				return null;
			}
			else
			{
				Debug.Log(TAG + "Assets loaded");
				return Newtonsoft.Json.JsonConvert.DeserializeObject<List<WezitAssets.Asset>>(manifestJsonString);
			}
		}
	}
} // End namespace Wezit
