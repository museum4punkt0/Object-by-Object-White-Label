/**
 * Created by Willy
 */

using UnityEngine;

public class Interpolator : MonoBehaviour
{
	public enum Types
	{
		easeIn, easeOut, expo, smoothInOut, customSmoothInOut
	};

	public static float Interpolate(float valueIn, Types type)
	{
		float valueOut = 0;
		switch (type)
		{
			case Types.easeOut:
				valueOut = Mathf.Sin(valueIn * Mathf.PI * 0.5f);
				break;

			case Types.easeIn:
				valueOut = 1f - Mathf.Cos(valueIn * Mathf.PI * 0.5f);
				break;

			case Types.expo:
				valueOut = valueIn * valueIn;
				break;

			case Types.smoothInOut:
				valueOut = valueIn * valueIn * (3f - 2f * valueIn);
				break;

			case Types.customSmoothInOut:
				valueOut = valueIn * valueIn * valueIn * (valueIn * (6f * valueIn - 15f) + 10f);
				break;
		}
		return valueOut;
	}
}
