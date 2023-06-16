using System.IO;
using UnityEngine;

public class AppStartUp : Singleton<AppStartUp>
{
	/*************************************************************/
	/*********************** PROPERTIES **************************/
	/*************************************************************/

	private static GameObject loader = null;

	/*************************************************************/
	/********************** STATIC METHODS ***********************/
	/*************************************************************/

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static async void OnBeforeSceneLoadRuntimeMethod()
	{
        ShowLoader(true);

        // ORDER IS IMPORTANT
        // Get app config
        await AppConfig.Instance.Init();

		// Init Wezit using the app config parameters
		if (AppConfig.Instance.ConfigModel.loadWezit)
		{
			Wezit.ConfigModel wezitConfigModel = Wezit.Config.Instance.ConfigModel;
			await Wezit.DataInitializer.Init(wezitConfigModel.manifestUrl,
											 wezitConfigModel.online,
										     wezitConfigModel.loadImages,
										     wezitConfigModel.downloadImagesOnStartup,
										     wezitConfigModel.downloadTransformation);
		}

		// Init app
		AppManager.Instance.Init();
		GlobalSettingsManager.Instance.Init();
        PlayerManager.Instance.Init();
        PlayerManager.Instance.Player.Load();

		SplashView.Instance.Hide();
        ShowLoader(false);
    }

	/*************************************************************/
	/***************** PUBLIC STATIC METHODS *********************/
	/*************************************************************/

	public static void ShowLoader(bool value)
	{
		if (value && loader == null)
		{
			loader = Instantiate(Resources.Load("Prefabs/UI/Loader")) as GameObject;
		}

		if (loader != null)
		{
			loader.SetActive(value);
		}
	}
}
