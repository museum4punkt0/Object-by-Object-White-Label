using System;
using System.Collections.Generic;

namespace Wezit
{

	[Serializable]
	public class PoiLocation
	{
		public string pid;
		public float x;
		public float y;
		public float lng;
		public float lat;
		public float relX;
		public float relY;
		public float radius;
		public string tour_pid;
		public string language;
		public string map_name;
		public List<AssetInfo> maps;

		public AssetInfo GetMapByTransformation(string transformation)
		{
			return this.maps.Find((info) => info.label == transformation);
		}

		public string GetMapSourceByTransformation(string transformation)
		{
			AssetInfo assetInfo = this.maps.Find((info) => info.label == transformation);
			return ((assetInfo != null) ? assetInfo.GetSource() : "");
		}
	}

}
