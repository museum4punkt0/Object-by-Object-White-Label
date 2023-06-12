using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UniRx.Async;

public class SpriteUtils
{
	private static Dictionary<string, UnityWebRequest> webRequestDict;

	public static void CleanRequestBySourceUri(string requestKey)
	{
		if (webRequestDict.ContainsKey(requestKey))
		{
			webRequestDict[requestKey].Abort();
			webRequestDict[requestKey].Dispose();
			webRequestDict[requestKey] = null;
			webRequestDict.Remove(requestKey);
		}
	}

	public static IEnumerator GetSpriteFromSource(string source, Action<Sprite> result, string requestKey)
	{
		if (webRequestDict == null) webRequestDict = new Dictionary<string, UnityWebRequest>();
		//CleanRequestBySourceUri(requestKey);

		Texture2D texture = new Texture2D(1, 1);
		Debug.Log(" SpriteUtils - GetSpriteFromSource - uri : " + source);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(source))
		{
			//webRequestDict.Add(requestKey, webRequest);
			yield return webRequest.SendWebRequest();

#if UNITY_EDITOR
#endif

			if (string.IsNullOrEmpty(webRequest.error))
			{
				if (texture != null && webRequest.downloadHandler.data != null)
				{
					texture.LoadImage(webRequest.downloadHandler.data);
					result(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100, 0, SpriteMeshType.FullRect));
				}
				else
				{
					Debug.Log("SpriteUtils - GetSpriteFromSource - File does not exist: " + source);
					result(Resources.Load<Sprite>("Images/DefaultImage/default"));
				}

			}
			else
			{
				Debug.LogError("SpriteUtils - GetSpriteFromSource - Error when downloading " + source + ": " + webRequest.error);
				result(Resources.Load<Sprite>("Images/DefaultImage/default"));
				//CleanRequestBySourceUri(requestKey);
				yield break;
			}
		}
		Resources.UnloadUnusedAssets();
	}

	public static IEnumerator GetTextureFromSource(string source, Action<Texture2D> result, string requestKey)
	{
		if (webRequestDict == null) webRequestDict = new Dictionary<string, UnityWebRequest>();
		//CleanRequestBySourceUri(requestKey);

		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		Debug.Log(" SpriteUtils - GetTextureFromSource - uri : " + source);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(source))
		{
			//webRequestDict.Add(requestKey, webRequest);
			yield return webRequest.SendWebRequest();

#if UNITY_EDITOR
#endif

			if (string.IsNullOrEmpty(webRequest.error))
			{
				if (texture != null && webRequest.downloadHandler.data != null)
				{
					texture.LoadImage(webRequest.downloadHandler.data);
					texture.Apply();
					result(texture);
				}
				else
				{
					Debug.Log("SpriteUtils - GetTextureFromSource - File does not exist: " + source);
					result(Resources.Load<Texture2D>("Images/DefaultImage/default"));
				}

			}
			else
			{
				Debug.LogError("SpriteUtils - GetSpriteFromSource - Error when downloading " + source + ": " + webRequest.error);
				result(Resources.Load<Texture2D>("Images/DefaultImage/default"));
				CleanRequestBySourceUri(requestKey);
				yield break;
			}
		}
		Resources.UnloadUnusedAssets();

	}

	public static async UniTask<Texture2D> GetTextureFromSource(string source)
	{
		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		Debug.Log(" SpriteUtils - GetTextureFromSource - uri : " + source);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(source))
		{
			await webRequest.SendWebRequest();

			if (string.IsNullOrEmpty(webRequest.error))
			{
				if (texture != null && webRequest.downloadHandler.data != null)
				{
					texture.LoadImage(webRequest.downloadHandler.data);
					texture.Apply();
					return(texture);
				}
				else
				{
					Debug.Log("SpriteUtils - GetTextureFromSource - File does not exist: " + source);
					return(Resources.Load<Texture2D>("Images/DefaultImage/default"));
				}

			}
			else
			{
				Debug.LogError("SpriteUtils - GetSpriteFromSource - Error when downloading " + source + ": " + webRequest.error);
				return(Resources.Load<Texture2D>("Images/DefaultImage/default"));
			}
		}
		Resources.UnloadUnusedAssets();
	}

	public static async UniTask SaveTextureFromSource(string source, string path, string fileName)
    {
		Texture2D texture = await GetTextureFromSource(source);
		byte[] bytes = texture.EncodeToPNG();
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		File.WriteAllBytes(path + fileName + ".png", bytes);
	}
}
