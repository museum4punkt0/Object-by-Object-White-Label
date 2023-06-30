using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using SimpleJSON;

namespace Wezit
{
    public class ActivityLoader: Singleton<ActivityLoader>
    {
        public async static Task<Relation> LookForActivity(Poi a_poi)
        {
            await a_poi.GetRelations(a_poi);
			Relation result = null;
			foreach (Relation relation in a_poi.Relations)
			{
				if (relation.relation == RelationName.HAS_ACTIVITY)
				{
					result = relation;
					break;
				}
			}
			return result;
		}

		public static async Task<JSONNode> LoadActivity(Relation an_activity)
        {
			string source = an_activity.GetAssetSourceByTransformation(WezitSourceTransformation.default_base);

			JSONNode activitySettings = null;
			string settingsJsonString = await Utils.FileUtils.RequestTextContent(source, 5);

			if (string.IsNullOrEmpty(settingsJsonString))
			{
				Debug.LogError("Cannot load settings from " + source);
				return null;
			}
			else
			{
				activitySettings = JSON.Parse(settingsJsonString);
				return activitySettings;
			}
		}

		private const string PREFABS_PATH = "Prefabs/ActivityPrefabs/";
		public Activity InstantiateActivity(JSONNode activitySettings, Language language, Transform activityRoot)
		{
			string type = activitySettings["default"]["template.app.common.type"];
            Activity instance = type switch
            {
                ActivityType.SCRATCH => Instantiate(Resources.Load<ScratchAndReveal>(PREFABS_PATH + "ScratchAndReveal"), activityRoot),
                ActivityType.QUIZ => Instantiate(Resources.Load<Quiz>(PREFABS_PATH + "Quiz"), activityRoot),
                _ => Instantiate(Resources.Load<Activity>(PREFABS_PATH + "Default"), activityRoot),
            };
			instance.Inflate(activitySettings, language);
			return instance;
		}
    }
}
