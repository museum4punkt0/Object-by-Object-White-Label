using SimpleJSON;
using UnityEngine;

namespace Wezit
{
	public class Config : Singleton<Config>
	{
		/*************************************************************/
		/*********************** PROPERTIES **************************/
		/*************************************************************/

		private static string TAG = "<color=red>[WezitConfig]</color>";

		private ConfigModel configModel;

		/*************************************************************/
		/********************** GETTER / SETTER **********************/
		/*************************************************************/

		public ConfigModel ConfigModel { get => configModel; }

		/*************************************************************/
		/********************** PUBLIC METHODS ***********************/
		/*************************************************************/

		public void Init(JSONNode result)
		{
			Debug.Log(TAG + " Init");
			configModel = JsonUtility.FromJson<ConfigModel>(result["wezit"].ToString());
		}
	}
} // end namespace Wezit
