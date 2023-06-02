/**
 * Created by Willy
 */

namespace Utils
{
	public static class ArrayUtils
	{
		public static void ShuffleArray<T>(T[] arr)
		{
			for (int i = arr.Length - 1; i > 0; i--)
			{
				int r = UnityEngine.Random.Range(0, i);
				T tmp = arr[i];
				arr[i] = arr[r];
				arr[r] = tmp;
			}
		}
	}
}
