using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;

namespace Wezit
{
	public class StoreInitializer
	{
		private static Wezit.SqlManager sqlManager = new Wezit.SqlManager();

		public static async UniTask<bool> Init()
		{
			Debug.Log("[Wezit Data] - Init");
			var (inventoryListResult,
				 tourListResult,
				 poiListResult,
				 locationListResult,
				 threeDPositionsListResult,
				 categoryListResult,
				 coverListResult) = await UniTask.WhenAll(sqlManager.GetInventoryList().ToUniTask(),
				 											  sqlManager.GetTourList().ToUniTask(),
															  sqlManager.GetPoiList().ToUniTask(),
															  sqlManager.GetLocationList().ToUniTask(),
															  sqlManager.Get3DPositions().ToUniTask(),
															  sqlManager.GetCategoryList().ToUniTask(),
															  sqlManager.GetCovers().ToUniTask());


			APIResponse<List<Poi>> poiListResponse = JsonUtility.FromJson<APIResponse<List<Poi>>>(poiListResult);
			List<Poi> poiList = await Initializer.InitPoiList(poiListResponse.data);
			StoreAccessor.Dispatch(Store.PoiList.ActionCreator.Set(poiList));

			APIResponse<List<Inventory>> inventoryListResponse = JsonUtility.FromJson<APIResponse<List<Inventory>>>(inventoryListResult);
			StoreAccessor.Dispatch(Store.InventoryList.ActionCreator.Set(inventoryListResponse.data));

			APIResponse<List<Tour>> tourListResponse = JsonUtility.FromJson<APIResponse<List<Tour>>>(tourListResult);
			List<Tour> tourList = await Initializer.InitTourList(tourListResponse.data, poiList);
			StoreAccessor.Dispatch(Store.TourList.ActionCreator.Set(tourList));

			APIResponse<List<PoiLocation>> locationListResponse = JsonUtility.FromJson<APIResponse<List<PoiLocation>>>(locationListResult);
			StoreAccessor.Dispatch(Store.LocationList.ActionCreator.Set(locationListResponse.data));

			APIResponse<List<ThreeDPosition>> threeDPositionsResponse = JsonUtility.FromJson<APIResponse<List<ThreeDPosition>>>(threeDPositionsListResult);
			StoreAccessor.Dispatch(Store.ThreeDPositionsList.ActionCreator.Set(threeDPositionsResponse.data));

			APIResponse<List<Category>> categoriesResponse = JsonUtility.FromJson<APIResponse<List<Category>>>(categoryListResult);
			StoreAccessor.Dispatch(Store.CategoryList.ActionCreator.Set(categoriesResponse.data));

			APIResponse<List<Cover>> coversResponse = JsonUtility.FromJson<APIResponse<List<Cover>>>(coverListResult);
			StoreAccessor.Dispatch(Store.CoverList.ActionCreator.Set(coversResponse.data));
			return true;
		}

		public static async UniTask<List<Poi>> GetPoiVersions(string id)
		{
			var result = await sqlManager.GetVersionByNode(id).ToUniTask();
			APIResponse<List<Poi>> response = JsonUtility.FromJson<APIResponse<List<Poi>>>(result);
			return response.data;
		}
	}

} // End namespace Wezit
