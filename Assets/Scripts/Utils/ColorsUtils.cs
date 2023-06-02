/**
 * Created by Willy
 */

using UnityEngine;

namespace Utils
{
	public class ColorsUtils : MonoBehaviour
	{
		private static string TAG = "ColorsUtils";

		public static Color GetColorByHtmlString(string htmlColor)
		{
			Color resultColor;
			ColorUtility.TryParseHtmlString(htmlColor, out resultColor);
			return resultColor;
		}
	}
}
