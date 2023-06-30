using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Wezit
{
	public class ManifestLoader : Singleton<ManifestLoader>
	{
		/*************************************************************/
		/*********************** PROPERTIES **************************/
		/*************************************************************/

		private static string TAG = "<color=red>[WezitManifest]</color>";

		private static Manifest manifest;

		/*************************************************************/
		/********************** GETTER / SETTER **********************/
		/*************************************************************/

		public static Manifest Manifest { get => manifest; }

		// Settings
		public static string SettingsUrl
		{
			get { return manifest.settings.url; }
		}

		public static string SettingsPath
		{
			get { return manifest.settings.path; }
		}

		// Assets
		public static string AssetsUrl
		{
			get { return manifest.assets.url; }
		}

		public static string AssetsPath
		{
			get { return manifest.assets.path; }
		}

		// Sqlite
		public static string SqliteUrl
		{
			get { return manifest.contents.toursql.url; }
		}

		public static string SqlitePath
		{
			get { return manifest.contents.toursql.path; }
		}

		// Manifest
		public static string ManifestUrl
		{
			get { return manifest.self.url; }
		}

		public static string ManifestPath
		{
			get { return manifest.self.path; }
		}

		// Ids
		public static string InventoryId
		{
			get 
			{ 
				return manifest.contents.pid.Replace("wzobj:inventory_", ""); 
			}
		}

		public static string EntityId
        {
			get { return "00" + manifest.service.entity.id; }
        }


		public static string ApiBaseUrl
        {
			get { return manifest.service.urlbase; }
        }

		public static string ApplicationId
		{
			get { return manifest.pid; }
        }

		/*************************************************************/
		/********************** PUBLIC METHODS ***********************/
		/*************************************************************/

		public static async UniTask<bool> Init(string manifestUrl, bool online)
		{
			Debug.Log(TAG + " Init");
#if UNITY_WEBGL
			manifest = await LoadWezitManifest(manifestUrl);
#else
			manifest = await Wezit.FilesDownloader.GetManifest(manifestUrl, online);
#endif
			string inventoryID = manifest.contents.pid.Split('_')[1];
			StoreAccessor.Dispatch(Store.Manifest.ActionCreator.SetInventoryID(inventoryID));

			return true;
		}

		/*************************************************************/
		/********************* PRIVATE METHODS ***********************/
		/*************************************************************/

		private static async UniTask<Manifest> LoadWezitManifest(string manifestUrl)
		{
			// Store manifest URL
			StoreAccessor.Dispatch(Store.Manifest.ActionCreator.SetManifestPath(manifestUrl));

			string manifestJsonString = await Utils.FileUtils.RequestTextContent(manifestUrl, 5);

			if (string.IsNullOrEmpty(manifestJsonString))
			{
				Debug.LogError(TAG + "Can not load manifest file from " + manifestUrl);
				return null;
			}
			else
			{
				Debug.Log(TAG + "Manifest loaded");
				return JsonUtility.FromJson<Manifest>(manifestJsonString);
			}
		}
	}
} // End namespace Wezit
