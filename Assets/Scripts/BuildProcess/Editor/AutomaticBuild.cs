#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.iOS;
using UnityEngine;
static class AutomaticBuild
{
	public static async void Perform()
	{
		List<string> scenes = new List<string>();

		string[] commandLineArgs = System.Environment.GetCommandLineArgs();

		string debugCommand = "Command Line Args : ";
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			debugCommand += commandLineArgs[i] + " ; ";
		}
		Debug.Log(debugCommand);

		for (int i = 0; i < commandLineArgs.Length; i++)
		{
            if (commandLineArgs[i] == "-downloadIconAndSplashFromSettings")
            {
				await SetAppIconNameAndSplash();
            }

			if (commandLineArgs[i] == "AutomaticBuild.Perform")
			{
				int j = i + 1;
				while (j < commandLineArgs.Length && commandLineArgs[j].StartsWith("-") == false)
				{
					Debug.Log("Scene Added : " + commandLineArgs[j]);
					scenes.Add(commandLineArgs[j]);
					j++;
				}

				break;
			}

		}

		if (scenes.Count == 0)
		{
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (scene.enabled)
				{
					scenes.Add(scene.path);
				}
			}
		}
		string productName = PlayerSettings.productName.Replace(" ", "");
		string dataPath = GetBuildPath(productName, EditorUserBuildSettings.activeBuildTarget);

		#if UNITY_ANDROID
        string query = "%AndroidKeystorePassword%"; // TODO: Account for OSX and Linux syntax for environment variables
        string keystorePassword = Environment.ExpandEnvironmentVariables(query);
        Debug.Log("keystorePassword" + keystorePassword);
        PlayerSettings.keystorePass = keystorePassword;
        PlayerSettings.keyaliasPass = keystorePassword;
		#endif

		UnityEditor.BuildPipeline.BuildPlayer(scenes.ToArray(), "./Builds/" + dataPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
		EditorApplication.Exit(0);
	}

	private static string GetBuildPath(string productName, BuildTarget buildTarget)
	{
		string dataPath = "";
		switch (buildTarget)
		{
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				{
					dataPath = productName + "/" + productName + ".exe";
					break;
				}

			case BuildTarget.WebGL:
				{
					dataPath = productName;
					break;
				}

			case BuildTarget.Android:
				{
					dataPath = productName + ".apk";
					break;
				}

			default:
				{
					dataPath = productName;
					break;
				}
		}

		return dataPath;
	}
	
	private static async Task SetAppIconNameAndSplash()
	{
		await AppConfig.Instance.Init();
		await Wezit.ManifestLoader.Init(Wezit.Config.Instance.ConfigModel.manifestUrl, true);
		await Wezit.Settings.Instance.Init(true);
		await Wezit.AssetsLoader.Init(true);

		SetAppName(Wezit.Settings.Instance.GetSettingAsCleanedText("template.app.common.title"));

		string splashSource = Wezit.Settings.Instance.GetSettingAsAssetSourceByTransformation("template.spk.loading.splashscreen");
		await SpriteUtils.SaveTextureFromSource(splashSource, Path.Combine(Application.dataPath, "Resources", "Images"), "splash");
		
		string iconSource = Wezit.Settings.Instance.GetSettingAsAssetSourceByTransformation("template.app.common.icon");
		Texture2D icon = await SpriteUtils.GetTextureFromSource(iconSource);
		Texture2D[] icons = { icon };
		SetAppIconsiOS(icons);
		SetAppIconsAndroid(icons);

		return;
    }

	private static void SetAppName(string appName)
    {
		PlayerSettings.productName = appName;
    }

	// `Adaptive` icons for Android require a background and foreground layer for each icon
	public static void SetAppIconsAndroid(Texture2D[] textures)
	{
		NamedBuildTarget platform = NamedBuildTarget.Android;
		PlatformIconKind kind = AndroidPlatformIconKind.Adaptive;

		PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(platform, kind);

		//Assign textures to each available icon slot.
		for (int i = 0; i < icons.Length; i++)
		{
			icons[i].SetTextures(textures[0]);
		}
		PlayerSettings.SetPlatformIcons(platform, kind, icons);
	}

	// Setting all `App` icons for iOS
	public static void SetAppIconsiOS(Texture2D[] textures)
	{
		NamedBuildTarget platform = NamedBuildTarget.iOS;
		PlatformIconKind kind = iOSPlatformIconKind.Application;

		PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(platform, kind);

		//Assign textures to each available icon slot.
		for (int i = 0; i < icons.Length; i++)
		{
			icons[i].SetTextures(textures[0]);
		}
		PlayerSettings.SetPlatformIcons(platform, kind, icons);
	}
}

#endif