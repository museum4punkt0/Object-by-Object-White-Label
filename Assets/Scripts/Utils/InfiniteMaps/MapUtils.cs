using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Utils
{
	public static class MapUtils
	{
		public static Vector3 CenterMapOnPoints(List<Vector2> points)
		{
			if (points.Count > 0)
			{
				Vector2 minBounds = Mathf.Infinity * Vector2.one;
				Vector2 maxBounds = -Mathf.Infinity * Vector2.one;
				foreach (Vector2 coordinates in points)
				{
					if (coordinates.x < minBounds.x)
					{
						minBounds.x = coordinates.x;
					}
					if (coordinates.y < minBounds.y)
					{
						minBounds.y = coordinates.y;
					}
					if (coordinates.x > maxBounds.x)
					{
						maxBounds.x = coordinates.x;
					}
					if (coordinates.y > maxBounds.y)
					{
						maxBounds.y = coordinates.y;
					}
				}
				return (ZoomToArea(minBounds, maxBounds, 3));
			}
			else return Vector3.zero;
		}

		public static Vector3 ZoomToArea(Vector2 boundsMin, Vector2 boundsMax, float paddingFactor)
		{
			double ry1 = Math.Log((Math.Sin(MathUtils.Deg2Rad(boundsMin.y)) + 1) /
								   Math.Cos(MathUtils.Deg2Rad(boundsMin.y)));
			double ry2 = Math.Log((Math.Sin(MathUtils.Deg2Rad(boundsMax.y)) + 1) /
								   Math.Cos(MathUtils.Deg2Rad(boundsMax.y)));

			double ryc = (ry1 + ry2) / 2f;
			double centerY = MathUtils.Rad2Deg((float)Math.Atan(Math.Sinh(ryc)));

			double resolutionHorizontal = Math.Abs(boundsMax.x - boundsMin.x) / Screen.width;

			double vy0 = Math.Log(Math.Tan(Math.PI * (0.25 + centerY / 360)));
			double vy1 = Math.Log(Math.Tan(Math.PI * (0.25 + boundsMax.y / 360)));
			double viewHeightHalf = Screen.height / 2f;
			double zoomFactorPowered = viewHeightHalf / (40.7436654315252 * (vy1 - vy0));

			double resolutionVertical = 360.0 / (zoomFactorPowered * 256);

			double resolution = Math.Max(resolutionHorizontal, resolutionVertical) * paddingFactor;
			double zoom = Math.Log(360 / (resolution * 256), 2);

			double lon = (boundsMax.x + boundsMin.x) / 2;
			double lat = centerY;

			return(new Vector3((float)lon, (float)lat, (float)zoom));
		}

		const int EARTHRADIUS = 6371000;
		// Calculate distance using Haversine formula
		public static float CalculateDistance(Vector2 pointA, Vector2 pointB)
		{
			float long1 = pointA.y * Mathf.PI / 180;
			float long2 = pointB.y * Mathf.PI / 180;
			float deltaLong = (pointB.y - pointA.y) * Mathf.PI / 180;
			float deltaLat = (pointB.x - pointA.x) * Mathf.PI / 180;

			float a = Mathf.Sin(deltaLong / 2) * Mathf.Sin(deltaLong / 2) +
						Mathf.Cos(long1) * Mathf.Cos(long2) *
						Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2);
			float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
			float distance = EARTHRADIUS * c;
			return (distance);
		}
	}
}
