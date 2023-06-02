/**
 * Created by Willy
 */

using System.Collections;
using UnityEngine;

namespace Utils
{
	public static class LerpObjectUtils
	{
		public static IEnumerator LerpObjectPosition(float delayValue, GameObject gameObject, Vector3 startPos, Vector3 endPos, float lerpTime)
		{
			if (!gameObject) yield return null;
			if (delayValue > 0) yield return new WaitForSeconds(delayValue);

			if (lerpTime > 0)
			{
				float currentLerpTime = 0f;
				float lerpValue = 0;
				while (currentLerpTime < lerpTime)
				{
					currentLerpTime += Time.deltaTime;
					if (currentLerpTime > lerpTime) currentLerpTime = lerpTime;
					lerpValue = currentLerpTime / lerpTime;
					lerpValue = Interpolator.Interpolate(lerpValue, Interpolator.Types.customSmoothInOut);

					gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, lerpValue);
					yield return null;
				}
			}
			else
			{
				gameObject.transform.localPosition = endPos;
				yield return null;
			}
		}

		public static IEnumerator LerpRectTransformAnchoredPosition(float delayValue, RectTransform rectTransform, Vector2 startPos, Vector2 endPos, float lerpTime)
		{
			if (!rectTransform) yield return null;
			if (delayValue > 0) yield return new WaitForSeconds(delayValue);

			if (lerpTime > 0)
			{
				float currentLerpTime = 0f;
				float lerpValue = 0;
				while (currentLerpTime < lerpTime)
				{
					currentLerpTime += Time.deltaTime;
					if (currentLerpTime > lerpTime) currentLerpTime = lerpTime;
					lerpValue = currentLerpTime / lerpTime;
					lerpValue = Interpolator.Interpolate(lerpValue, Interpolator.Types.customSmoothInOut);

					rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, lerpValue);
					yield return null;
				}
			}
			else
			{
				rectTransform.anchoredPosition = endPos;
				yield return null;
			}
		}

		public static IEnumerator LerpObjectLocalRotation(float delayValue, Transform objectTarget, Quaternion startRot, Quaternion endRot, float lerpTime)
		{
			if (!objectTarget) yield return null;
			if (delayValue > 0) yield return new WaitForSeconds(delayValue);

			if (lerpTime > 0)
			{
				float currentLerpTime = 0f;
				float lerpValue = 0;
				while (currentLerpTime < lerpTime)
				{
					currentLerpTime += Time.deltaTime;
					if (currentLerpTime > lerpTime) currentLerpTime = lerpTime;
					lerpValue = currentLerpTime / lerpTime;
					lerpValue = Interpolator.Interpolate(lerpValue, Interpolator.Types.customSmoothInOut);

					objectTarget.localRotation = Quaternion.Lerp(startRot, endRot, lerpValue);
					yield return null;
				}
			}
			else
			{
				objectTarget.localRotation = endRot;
				yield return null;
			}
		}
	}
}