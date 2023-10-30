using System;
using System.Collections.Generic;

namespace Wezit
{
	[Serializable]
	public class PoiRelation
	{
		public string pid;
		public string relation_name; // WezitStudioVersion.v2
		public string relationName; // WezitStudioVersion.v3
		public string relation;

		public bool IsAsset()
		{
			return IsAsset(relationName);
		}

		private bool IsAsset(string aRelationName)
		{
			return (
			aRelationName == RelationName.SHOW_PICTURE
			|| aRelationName == RelationName.REF_PICTURE
			|| aRelationName == RelationName.PLAY_TRACK
			|| aRelationName == RelationName.PLAY_VIDEO
			|| aRelationName == RelationName.ZIP_FILE
			);
		}
	}

	[Serializable]
	public class Poi : Node
	{
		public List<PoiRelation> relation_list; // WezitStudioVersion.v2
		public List<PoiRelation> relationList; // WezitStudioVersion.v3
		public List<PoiRelation> child_list;
		public string parentPid;

		public Poi(string poiPid, string aParentPid)
		{
			pid = poiPid;
			parentPid = aParentPid;
		}

		public Wezit.Poi parentPoi
        {
			get => PoiStore.GetPoiById(parentPid);
        }

		public void CopyDatasFrom(Poi otherPoi)
		{
			pid = otherPoi.pid;
			language = otherPoi.language;
			title = otherPoi.title;
			subject = otherPoi.subject;
			description = otherPoi.description;
			aspects = otherPoi.aspects;
			tags = otherPoi.tags;
			childs = otherPoi.childs;
			// Relations = otherPoi.Relations;
			relation_list = otherPoi.relation_list; // WezitStudioVersion.v2
			relationList = otherPoi.relationList; // WezitStudioVersion.v3
			child_list = otherPoi.child_list;
			identifier = otherPoi.identifier;
			format = otherPoi.format;
			contributor = otherPoi.contributor;
			author = otherPoi.author;
			type = otherPoi.type;
			date = otherPoi.date;
			source = otherPoi.source;
			creator = otherPoi.creator;
			rights = otherPoi.rights;
			spatial = otherPoi.spatial;
			location = otherPoi.location;
			extent = otherPoi.extent;
		}
	}

}
