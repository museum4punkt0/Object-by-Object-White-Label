using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public class VideoUtils : MonoBehaviour
{
	public async static UniTask<string> GetVideoSourceByTransformation(Wezit.Node wezitData, string transformation = "original")
	{
		string source = "";
		if (wezitData != null)
		{
			await wezitData.GetRelations(wezitData);
			foreach (Wezit.Relation relation in wezitData.Relations)
			{
				if (relation.relation == Wezit.RelationName.PLAY_VIDEO)
				{
					source = relation.GetAssetSourceByTransformation(transformation);
					break;
				}
			}
		}
		return source;
	}
}
