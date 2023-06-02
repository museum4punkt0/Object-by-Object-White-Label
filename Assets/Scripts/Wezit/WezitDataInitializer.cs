using System.IO;
using System.Threading.Tasks;

namespace Wezit
{
	public class DataInitializer
	{
		/*************************************************************/
		/********************** STATIC METHODS ***********************/
		/*************************************************************/

		public static async Task Init(string manifestUrl, bool online, bool loadImages, bool downloadImagesOnStartup, string downloadTransformation)
		{
			await ManifestLoader.Init(manifestUrl, online);
#if !UNITY_WEBGL
			await FilesDownloader.GetSqlite();
#endif
			await Settings.Instance.Init(online);
			await AssetsLoader.Init(online);
			await StoreInitializer.Instance.Init();
#if !UNITY_WEBGL
			if (loadImages)
			{
				DataGrabber.Instance.Load();
				if (downloadImagesOnStartup)
				{
					await DataGrabber.Instance.GetAllAssets(downloadTransformation);
				}
			}
#endif
		}
	}
}