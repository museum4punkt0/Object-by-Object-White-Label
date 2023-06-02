using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;

namespace Wezit
{
	public class Initializer
	{
		/*************************************************************/
		/*********************** PROPERTIES **************************/
		/*************************************************************/

		private static Wezit.SqlManager sqlManager = new Wezit.SqlManager();

		/*************************************************************/
		/********************** PUBLIC METHODS ***********************/
		/*************************************************************/

		public static async UniTask<List<Poi>> InitPoiList(List<Poi> datas)
		{
			foreach (Poi poi in datas)
			{
				// Get child list only if there is some childs to initialize
				poi.child_list = new List<PoiRelation>();
				PoiRelation child = poi.relationList.Find(r => r.relationName == RelationName.HAS_NODE);
				if (child != null)
				{
					string result = await sqlManager.GetRelationListByPoiId(poi.language, poi.pid);

					APIResponse<List<PoiRelation>> response = JsonUtility.FromJson<APIResponse<List<PoiRelation>>>(result);
					poi.child_list = response.data;
				}

				// Aspects
				if (!string.IsNullOrWhiteSpace(poi.aspects))
				{
					// LIM: TODO ADD ASPECTS INITIALIZATION (ARTWORK / TIME_INFORMATIONS / ...)
				}
			}

			return datas;
		}

		public static async UniTask<List<Tour>> InitTourList(List<Tour> datas, List<Poi> poiList)
		{
			foreach (Tour tour in datas)
			{
				tour.Relations = await GetAssetList("tour", tour);

				if (poiList != null && poiList.Count > 0)
				{
					tour.childs = await GetTourChilds(tour, poiList);
				}
			};

			return datas;
		}

		public static async UniTask<List<Relation>> GetAssetList(string type, Node node)
		{
			var result = await sqlManager.GetAssetListByNodeId(type, node.pid).ToUniTask();
			APIResponse<List<Relation>> response = JsonUtility.FromJson<APIResponse<List<Relation>>>(result);
			return response.data;
		}

		/*************************************************************/
		/********************** PRIVATE METHODS ***********************/
		/*************************************************************/

		private static async UniTask<List<Poi>> GetTourChilds(Tour tour, List<Poi> poiList)
		{
			var result = await sqlManager.GetPoiListByTourId(tour.pid).ToUniTask();
			APIResponse<List<Poi>> response = JsonUtility.FromJson<APIResponse<List<Poi>>>(result);
			return await FindPoiList(response.data, poiList);
		}

		private static async UniTask<List<Poi>> GetPoiChilds(Poi poi, List<Poi> poiList)
		{
#if !UNITY_WEBGL
			List<PoiRelation> childsRelations = poi.child_list.Where(r => r.relationName == RelationName.HAS_NODE).ToList();
#else
			List<PoiRelation> childsRelations = poi.child_list.Where(r => r.relation == RelationName.HAS_NODE).ToList();
#endif

			List<Poi> childs = new List<Poi>();
			childsRelations.ForEach(relation =>
			{
				childs.Add(new Poi(relation.pid, poi.pid));
			});

			return await FindPoiList(childs, poiList);
		}

		private static async UniTask<List<Poi>> FindPoiList(List<Poi> childList, List<Poi> poiList)
		{
			foreach (Poi poi in childList)
			{
				Poi foundPoi = poiList.Find(p => p.pid == poi.pid);
				poi.CopyDatasFrom(foundPoi);
				// poi.formatDatas();
				if (poiList.Count > 0)
				{
					poi.childs = await GetPoiChilds(poi, poiList);
					foundPoi.childs = poi.childs;
				}
			}

			return childList;
		}
	}

} // End namespace Wezit