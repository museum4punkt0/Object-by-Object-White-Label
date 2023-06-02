using System;
using System.Collections.Generic;
using System.Linq;
using UniRx.Async;

namespace Wezit
{
	[Serializable]
	public class Node : Base
	{
		public string aspects;
		public List<Poi> childs;

		private List<Relation> relations;
		private bool relationsSetted = false;

		public List<Relation> Relations
		{
			get
			{
				if (!relationsSetted) UnityEngine.Debug.LogWarning("[Node] - relations are not setted. Call GetRelations() function");
				return relations;
			}
			set
			{
				relations = value;
			}
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public async UniTask<List<Relation>> GetRelations(Node node)
		{
			if (relationsSetted == false)
				await InitRelations(node);

			return node.Relations;
		}

		private async UniTask InitRelations(Node node)
		{
			node.Relations = new List<Relation>();
			// Get asset list only if there is some assets to initialize
			//if ((Config.Instance.ConfigModel.studioVersion == WezitStudioVersion.v2 ? node.relation_list : node.relationList).Find(r => r.IsAsset()) != null) 
			node.Relations = await Initializer.GetAssetList("poi", node);

			relationsSetted = true;
		}

		public Relation GetRelation(string relationName)
		{
			return this.Relations.Find((relation) => relation.relation == relationName);
		}

		public List<Relation> GetRelationList(string relationName)
		{
			return this.Relations.Where((relation) => relation.relation == relationName).ToList();
		}

		public Relation GetRelationByUsage(string usage)
		{
			return this.Relations.Find((relation) => relation.usage == usage);
		}
	}
}