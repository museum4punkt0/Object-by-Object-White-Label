/**
 * Created by Willy
 */

using UnityEngine;

namespace Utils
{
	public class ColorsUtils : MonoBehaviour
	{
		public static Color GetColorByHtmlString(string htmlColor)
		{
			Color resultColor;
			ColorUtility.TryParseHtmlString(htmlColor, out resultColor);
			return resultColor;
		}
	}
}
