/**
 * Created by Willy
 */

using System.Collections.Generic;
using UnityEngine;

public class PoiLocationStore : MonoBehaviour
{
	public static string TAG = "[PoiLocationStore]";

	private static Dictionary<string, Wezit.PoiLocation> PoiLocationDict = null;

	public static void Init()
	{
		PoiLocationDict = new Dictionary<string, Wezit.PoiLocation>();
		if (StoreAccessor.State.LocationList != null)
		{
			foreach (Wezit.PoiLocation poiLocation in StoreAccessor.State.LocationList)
				PoiLocationDict.Add(poiLocation.pid, poiLocation);
		}
		else
		{
			Debug.LogError("No LocationList in Store");
		}
	}

	public static Wezit.PoiLocation GetPoiLocationById(string poiLocationPid)
	{
		Wezit.PoiLocation output = null;
		if (PoiLocationDict != null && PoiLocationDict.ContainsKey(poiLocationPid)) output = PoiLocationDict[poiLocationPid];
		return output;
	}

}
