/**
 * Created by Willy
 */

using System.Collections.Generic;
using UnityEngine;

public class PoiStore : MonoBehaviour
{
	public static string TAG = "[PoiStore]";

	public static void Init()
	{
	}

	public static Wezit.Poi GetPoiById(string poiPid)
	{
		// List<Wezit.Poi> poiList = StoreAccessor.State.PoiList;
		// return poiList.Find(poi => poi.pid == poiPid);

		List<Wezit.Tour> tourList = StoreAccessor.State.TourList;

		foreach (Wezit.Tour tour in tourList)
		{
			Wezit.Poi poi = tour.FindPoiInTour(poiPid);
			if (poi != null)
			{
				return poi;
			}
		}

		Debug.LogError(TAG + " GetPoiById : Could not find poi of id " + poiPid);
		return null;
	}

	public static Wezit.Poi GetParentPoiByChildId(string childPid)
	{
		List<Wezit.Poi> poiList = StoreAccessor.State.PoiList;
		foreach (Wezit.Poi poi in poiList)
		{
			if (poi.child_list.Find(relation => relation.pid == childPid) != null)
			{
				return poi;
			}
		}

		return null;
	}
}
