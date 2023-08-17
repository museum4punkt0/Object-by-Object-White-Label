using System.IO;
using System.Threading.Tasks;

namespace Wezit
{
	public class DataInitializer
	{
		/*************************************************************/
		/********************** STATIC METHODS ***********************/
		/*************************************************************/

		public static async Task Init(string manifestUrl, bool online, bool loadImages, bool downloadImagesOnStartup, string downloadTransformation, bool downloadSettingsImagesOnStartup)
		{
			await ManifestLoader.Init(manifestUrl, online);
#if !UNITY_WEBGL
			if(!online)
            {
				await FilesDownloader.GetSqlite();
            }
#endif
			await Settings.Instance.Init(online);
			await AssetsLoader.Init(online);
			await StoreInitializer.Init();
#if !UNITY_WEBGL
			if (loadImages && !online)
			{
				DataGrabber.Instance.Load();
				DataGrabber.Instance.AppDefaultTransformation = downloadTransformation;
				if (downloadImagesOnStartup)
				{
					await DataGrabber.Instance.GetAllAssets(downloadTransformation);
				}
				else if(downloadSettingsImagesOnStartup)
                {
					await DataGrabber.Instance.GetSettingsAssets(downloadTransformation);
                }
				SplashUtils.DownloadSplashImages();
			}
#endif
		}
	}
}