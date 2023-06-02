/**
 * Created by Willy
 */

using System.Collections;

namespace Utils
{
	public static class FrameUtils
	{
		public static IEnumerator WaitForFrames(int frameCount)
		{
			while (frameCount > 0)
			{
				frameCount--;
				yield return null;
			}
		}
	}
}
