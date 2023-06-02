using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public class AudioUtils : MonoBehaviour
{
	public async static UniTask<string> GetAudioSource(Wezit.Node wezitData)
	{
		string source = "";
		if (wezitData != null)
		{
			await wezitData.GetRelations(wezitData);
			foreach (Wezit.Relation relation in wezitData.Relations)
			{
				if (relation.relation == Wezit.RelationName.PLAY_TRACK)
				{
					source = relation.GetAssetSourceByTransformation(WezitSourceTransformation.original);
					break;
				}
			}
		}
		return source;
	}

	public static async UniTask<AudioClip> GetAudioClip(Wezit.Node wezitData)
	{
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(await GetAudioSource(wezitData), AudioType.MPEG);
		await www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log(www.error);
			return null;
		}
		else return (DownloadHandlerAudioClip.GetContent(www));
	}

	public static async UniTask<AudioClip> GetAudioClipFromSource(string source)
	{
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(source, AudioType.MPEG);
		await www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log(www.error);
			return null;
		}
		else return (DownloadHandlerAudioClip.GetContent(www));
	}
}
