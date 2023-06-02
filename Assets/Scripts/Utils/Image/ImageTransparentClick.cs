/**
 * Created by Willy
---------------------
If you ever run into your transparent buttons not working, be sure to try the following steps:
-Make sure read/write is enabled in the texture import settings
-Sprite Mode MeshType to FullRect
-Disable atlassing under Project Settings > Editor > Sprite Packer > Mode
*/

using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
	public class ImageTransparentClick : MonoBehaviour
	{
		public float alphaThreshold = 0.001f;

		private void OnEnable()
		{
			if (TryGetComponent<Image>(out Image image))
			{
				image.alphaHitTestMinimumThreshold = alphaThreshold;
			}
			else
			{
				Debug.LogError(" ImageTransparentClick - Image component not found");
			}
		}
	}
}
