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
			return maps.Find((info) => info.label == transformation);
		}

		public string GetMapSourceByTransformation(string transformation)
		{
			AssetInfo assetInfo = maps.Find((info) => info.label == transformation);

			string source = "";

			if(assetInfo != null)
            {
				source = assetInfo.GetSource();
				if(transformation == WezitSourceTransformation.tilesZip && source.Contains("http"))
                {
					assetInfo = maps.Find((info) => info.label == WezitSourceTransformation.tiles);
					source = assetInfo.GetSource();
                }
            }

			return source;
		}
	}

}
