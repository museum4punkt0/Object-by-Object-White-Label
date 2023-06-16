﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
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

            if (commandLineArgs[i] == "DownloadIconAndSplashFromSettings")
            {
				await DownloadIconsAndSplash();
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

		BuildPipeline.BuildPlayer(scenes.ToArray(), "./Builds/" + dataPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
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
	
	private static async Task DownloadIconsAndSplash()
    {
		await SpriteUtils.SaveTextureFromSource("", Path.Combine(Application.dataPath, "Resources", "Images"), "splash");
		Texture2D icon = await SpriteUtils.GetTextureFromSource("");
		return;
    }
}

#endif