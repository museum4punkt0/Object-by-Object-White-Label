using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public class WezitDataUtils
{
	#region Poi
	public static Wezit.Tour GetWezitTourByLang(Language language)
	{
		Wezit.Tour wezitTour = null;
		wezitTour = StoreAccessor.State.TourList.Find(x => x.language == language.ToString());
		return wezitTour;
	}

	public static List<Wezit.Tour> GetWezitToursByLang(Language language)
    {
		return StoreAccessor.State.TourList.FindAll(x => x.language == language.ToString());
    }

	public static Wezit.Poi GetWezitPoiByTag(Language language, string tag)
	{
		Wezit.Poi wzPoiResult = null;
		Wezit.Tour wezitTour = GetWezitTourByLang(language);

		if (wezitTour != null)
		{
			foreach (Wezit.Poi wzPoi in wezitTour.childs)
			{
				if (wzPoi.tags == tag)
				{
					wzPoiResult = wzPoi;
					break;
				}
			}
		}

		return wzPoiResult;
	}

	public static Wezit.Poi GetWezitChildPoiByTag(Wezit.Poi wzPoiParent, string tag)
	{
		Wezit.Poi wzPoiResult = null;

		if (wzPoiParent != null)
		{
			foreach (Wezit.Poi wzPoi in wzPoiParent.childs)
			{
				if (wzPoi.tags == tag)
				{
					wzPoiResult = wzPoi;
					break;
				}
			}
		}

		return wzPoiResult;
	}

	public static Wezit.Poi GetWezitChildPoiByType(Wezit.Poi wzPoiParent, string type)
	{
		Wezit.Poi wzPoiResult = null;

		if (wzPoiParent != null)
		{
			foreach (Wezit.Poi wzPoi in wzPoiParent.childs)
			{
				if (wzPoi.type == type)
				{
					wzPoiResult = wzPoi;
					break;
				}
			}
		}

		return wzPoiResult;
	}

	public static Wezit.Poi GetWezitChildPoiByType(Wezit.Tour wzTourParent, string type)
	{
		Wezit.Poi wzPoiResult = null;

		if (wzTourParent != null)
		{
			foreach (Wezit.Poi wzPoi in wzTourParent.childs)
			{
				if (wzPoi.type == type)
				{
					wzPoiResult = wzPoi;
					break;
				}
			}
		}

		return wzPoiResult;
	}

	public static List<Wezit.Poi> GetPoiChildren(Wezit.Poi wzPoiParent)
	{
		if (wzPoiParent != null)
		{
			if (wzPoiParent.childs != null && wzPoiParent.childs.Count > 0)
			{
				return wzPoiParent.childs;
			}
			else
			{
				if (wzPoiParent.child_list != null && wzPoiParent.child_list.Count > 0)
				{
					List<Wezit.Poi> childPois = new List<Wezit.Poi>();
					foreach (Wezit.PoiRelation childRelation in wzPoiParent.child_list)
					{
						if (childRelation.relationName == Wezit.RelationName.HAS_NODE)
						{
							childPois.Add(PoiStore.GetPoiById(childRelation.pid));
						}
					}
					return childPois;
				}
			}
		}
		return null;
	}

	protected async UniTask<Wezit.Poi> GetCorrespondingPoiByLanguage(Wezit.Poi currentPoi, Language language)
	{
		List<Wezit.Poi> resultData = await Wezit.StoreInitializer.GetPoiVersions(currentPoi.pid);

		if (resultData != null)
		{
			Wezit.Poi newPoi = resultData.Find(poi => poi.language == language.ToString());
			if (newPoi != null) return PoiStore.GetPoiById(newPoi.pid);
		}

		Debug.LogError("Could not find corresponding poi of poi : " + currentPoi.pid + "for language : " + language.ToString());
		return null;
	}

	public static int ForceMediaListOrderByUsage(Wezit.Relation r1, Wezit.Relation r2)
	{
		return Convert.ToInt32(r1.usage.Split('/')[1]).CompareTo(Convert.ToInt32(r2.usage.Split('/')[1]));
	}
	#endregion
}
