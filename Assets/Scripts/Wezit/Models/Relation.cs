using System;
using System.Collections.Generic;
using System.IO;

namespace Wezit
{
	class RelationName
	{
		public const string HAS_NODE = "hasNode";
		public const string SHOW_PICTURE = "relationForShowPicture";
		public const string REF_PICTURE = "relationForSetRefPicture";
		public const string PLAY_VIDEO = "relationForPlayVideo";
		public const string PLAY_TRACK = "relationForPlayTrack";
		public const string ZIP_FILE = "relationForSetDataInZip";
		public const string AUTHORED_BY = "authoredBy";
		public const string SHOW_360_PICTURE = "relationForShow360Picture";
		public const string SHOW_3D_MODEL = "relationForShowModel3D";
		public const string HAS_ACTIVITY = "relationForActivity";
		public const string HAS_COVER = "hasCover";
	}

	[Serializable]
	public class Relation : Base
	{
		public string relation;
		public string usage;
		public int ord;

		public List<AssetInfo> assets;

		public override string ToString()
		{
			return base.ToString() + String.Format(
				"Relation: {0}\n",
				relation
			);
		}

		public AssetInfo GetAssetByTransformation(string transformation)
		{
			if(assets != null)
            {
				AssetInfo asset = assets.Find(info => info.label == transformation);
				if(asset != null)
                {
					return asset;
                }
				else
                {
					return assets.Find(info => info.label == "original");
                }
            }
			else
            {
				return null;
            }
		}

		public string GetAssetSourceByTransformation(string transformation)
		{
			AssetInfo assetInfo = this.GetAssetByTransformation(transformation);
			return assetInfo != null ? assetInfo.GetSource() : "";
		}

		public string GetAssetMimeTypeByTransformation(string transformation)
		{
			AssetInfo assetInfo = this.GetAssetByTransformation(transformation);
			return assetInfo != null ? assetInfo.GetMimeType() : "";
		}
	}
}
