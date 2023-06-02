/**
 * Created by Willy
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
	public class LayoutGroupRebuilder
	{
		public static IEnumerator Rebuild(GameObject container)
		{
			RectTransform layoutToRebuild = container.GetComponentsInParent<RectTransform>(true)[0];

			if (container && layoutToRebuild)
			{
				while (!(layoutToRebuild.gameObject.activeSelf && layoutToRebuild.gameObject.activeInHierarchy))
					yield return new WaitForSeconds(0.1f);

				LayoutRebuilder.ForceRebuildLayoutImmediate(layoutToRebuild);
			}
		}
	}


}
