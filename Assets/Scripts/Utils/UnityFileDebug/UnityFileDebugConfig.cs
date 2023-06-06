using UnityEngine;
using SimpleJSON;

namespace Unity
{
	public class FileDebugConfig : Singleton<FileDebugConfig>
	{
		/*************************************************************/
		/*********************** PROPERTIES **************************/
		/*************************************************************/


		private FileDebugConfigModel fileDebugConfig;
		private FileDebug fileDebugObject;

		/*************************************************************/
		/********************** PUBLIC METHODS ***********************/
		/*************************************************************/

		public void Init(JSONNode result)
		{
			fileDebugConfig = JsonUtility.FromJson<FileDebugConfigModel>(result["unityFileDebugConfig"].ToString());

			fileDebugObject = gameObject.AddComponent<FileDebug>();
			fileDebugObject.gameObject.SetActive(false);
			fileDebugObject.Init(fileDebugConfig);
			fileDebugObject.gameObject.SetActive(true);
		}
	}

} // End namespace UnityFileDebug
