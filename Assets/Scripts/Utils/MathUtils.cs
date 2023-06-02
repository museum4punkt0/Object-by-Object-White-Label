/**
 * Created by Willy
 */

using System;
using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static float Map(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			 // accepts e.g. -80, 80
			 if (angle < 0f) angle = 360 + angle;
			 if (angle > 180f) return Mathf.Max(angle, 360+min);
			 return Mathf.Min(angle, max);
		}

		public static float Deg2Rad(float degAngle)
		{
			return degAngle * Mathf.PI / 180f;
		}

		public static float Rad2Deg(float degAngle)
		{
			return degAngle * 180f / Mathf.PI;
		}
	}
}
