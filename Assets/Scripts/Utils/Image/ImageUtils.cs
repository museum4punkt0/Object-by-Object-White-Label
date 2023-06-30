/**
 * Created by Willy
 */

using System;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Utils
{
	public static class ImageUtils
	{
		private const int _imagesLoadBeforeResourceClean = 10;
		private static int _currentImagesLoadCount = 0;

		private static IEnumerator getImageRoutine = null;
		private static Dictionary<MonoBehaviour, IEnumerator> routineDict = new Dictionary<MonoBehaviour, IEnumerator>();
		private static Dictionary<MonoBehaviour, string> requestsDict = new Dictionary<MonoBehaviour, string>();

        #region Image
        public static IEnumerator SetImage(Image imageComponent, string assetSource, string mimeType = "", bool envelopeParent = true, MonoBehaviour context = null, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
		{
			if (_currentImagesLoadCount >= _imagesLoadBeforeResourceClean) Resources.UnloadUnusedAssets();
			_currentImagesLoadCount++;

			string requestKey = assetSource;
			if (context != null)
			{
				requestKey += "_" + context.GetInstanceID();
			}

			imageComponent.CrossFadeAlpha(0, 0f, false);

			if (String.IsNullOrEmpty(assetSource))
			{
				ResetImage(imageComponent);
				yield break;
			}

			if (context != null && routineDict.ContainsKey(context))
			{
				if (requestsDict.ContainsKey(context))
				{
					SpriteUtils.CleanRequestBySourceUri(requestsDict[context]);
					requestsDict.Remove(context);
				}

				context.StopCoroutine(routineDict[context]);
				routineDict.Remove(context);
			}

			getImageRoutine = SpriteUtils.GetSpriteFromSource(assetSource, (result) => ApplySprite(imageComponent, envelopeParent, result, crossFadeAlphaDuration, enableImageOnLoaded, alphaOnLoaded), requestKey); ;

			if (context != null) routineDict.Add(context, getImageRoutine);
			if (context != null) requestsDict.Add(context, requestKey);

			yield return getImageRoutine;

			if (context != null && routineDict.ContainsKey(context)) routineDict.Remove(context);
			if (context != null && requestsDict.ContainsKey(context)) requestsDict.Remove(context);
		}

		public static void ResetImage(Image imageComponent)
		{
			if (imageComponent)
			{
				imageComponent.sprite = null;
				imageComponent.enabled = false;
			}
		}

		private static void ApplySprite(Image imageComponent, bool envelopeParent, Sprite sprite, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
		{
			if (sprite != null && imageComponent != null)
			{
				imageComponent.sprite = sprite;
				imageComponent.enabled = true;
				imageComponent.preserveAspect = true;

				if (imageComponent.GetComponent<AspectRatioFitter>() != null)
				{
					if (envelopeParent)
					{
						// Fill parent method (centered and cropped)
						imageComponent.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
					}
					else
					{
						// Fit in parent
						imageComponent.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
					}

					imageComponent.GetComponent<AspectRatioFitter>().aspectRatio = (float)Math.Round((double)sprite.bounds.size.x / sprite.bounds.size.y, 2);
				}
				else
				{
					UnityEngine.Debug.LogWarning("[ImageUtils.SetImage] - Need 'AspectRatioFitter' component");
				}

				if (enableImageOnLoaded) imageComponent.CrossFadeAlpha(alphaOnLoaded, crossFadeAlphaDuration, false);
				else imageComponent.enabled = false;
			}
		}

		public async static void LoadImage(Image imageComponent, MonoBehaviour monoBehaviourInstance, Wezit.Node wzData, string targetRelation = "relationForShowPicture", string targetWzSourceTransformation = "default", bool envelopeParent = true, float crossFadeAlphaDuration = 0.25f)
		{
			if (imageComponent != null)
			{
				if (wzData != null)
				{
					bool hasImage = false;
					await wzData.GetRelations(wzData);
					foreach (Wezit.Relation relation in wzData.Relations)
					{
						if (relation.relation == targetRelation)
						{
							hasImage = true;
							monoBehaviourInstance.StartCoroutine(SetImage(
								imageComponent,
								relation.GetAssetSourceByTransformation(targetWzSourceTransformation),
								relation.GetAssetMimeTypeByTransformation(targetWzSourceTransformation),
								envelopeParent,
								null,
								crossFadeAlphaDuration));
							break;
						}
					}
					if (!hasImage) ResetImage(imageComponent);
				}
				else
				{
					ResetImage(imageComponent);
				}
			}
		}
		#endregion

		#region SpriteRenderer
		public static IEnumerator SetImage(SpriteRenderer spriteRenderer, string assetSource, int refHeight, float refScale, string mimeType = "", MonoBehaviour context = null, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
		{
			if (_currentImagesLoadCount >= _imagesLoadBeforeResourceClean) Resources.UnloadUnusedAssets();
			_currentImagesLoadCount++;

			string requestKey = assetSource;
			if (context != null)
			{
				requestKey += "_" + context.GetInstanceID();
			}

			if (string.IsNullOrEmpty(assetSource))
			{
				ResetImage(spriteRenderer);
				yield break;
			}

			if (context != null && routineDict.ContainsKey(context))
			{
				if (requestsDict.ContainsKey(context))
				{
					SpriteUtils.CleanRequestBySourceUri(requestsDict[context]);
					requestsDict.Remove(context);
				}

				context.StopCoroutine(routineDict[context]);
				routineDict.Remove(context);
			}

			getImageRoutine = SpriteUtils.GetSpriteFromSource(assetSource, (result) => ApplySprite(spriteRenderer, result, refHeight, refScale), requestKey);

			if (context != null) routineDict.Add(context, getImageRoutine);
			if (context != null) requestsDict.Add(context, requestKey);

			yield return getImageRoutine;

			if (context != null && routineDict.ContainsKey(context)) routineDict.Remove(context);
			if (context != null && requestsDict.ContainsKey(context)) requestsDict.Remove(context);
		}

		public static void ResetImage(SpriteRenderer imageComponent)
		{
			if (imageComponent)
			{
				imageComponent.sprite = null;
				imageComponent.enabled = false;
			}
		}

		private static void ApplySprite(SpriteRenderer spriteRenderer, Sprite sprite, int refHeight, float refScale)
		{
			if (sprite != null && spriteRenderer != null)
			{
				spriteRenderer.sprite = sprite;
				spriteRenderer.enabled = true;
				spriteRenderer.transform.localScale = refScale * refHeight / sprite.texture.height * Vector3.one;
			}
		}

		public async static void LoadImage(SpriteRenderer spriteRenderer, MonoBehaviour monoBehaviourInstance, Wezit.Node wzData, int refHeight, float refScale, string targetRelation = "relationForShowPicture", string targetWzSourceTransformation = "default")
		{
			if (spriteRenderer != null)
			{
				if (wzData != null)
				{
					bool hasImage = false;
					await wzData.GetRelations(wzData);
					foreach (Wezit.Relation relation in wzData.Relations)
					{
						if (relation.relation == targetRelation)
						{
							hasImage = true;
							monoBehaviourInstance.StartCoroutine(SetImage(
								spriteRenderer,
								relation.GetAssetSourceByTransformation(targetWzSourceTransformation),
								refHeight,
								refScale,
								relation.GetAssetMimeTypeByTransformation(targetWzSourceTransformation)));
							break;
						}
					}
					if (!hasImage) ResetImage(spriteRenderer);
				}
				else
				{
					ResetImage(spriteRenderer);
				}
			}
		}
		#endregion

		#region RawImage
		public static IEnumerator SetImage(RawImage imageComponent, string assetSource, string mimeType = "", bool envelopeParent = true, MonoBehaviour context = null, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
		{

			if (_currentImagesLoadCount >= _imagesLoadBeforeResourceClean) Resources.UnloadUnusedAssets();
			_currentImagesLoadCount++;

			string requestKey = assetSource;
			if (context != null)
			{
				requestKey += "_" + context.GetInstanceID();
			}

			imageComponent.CrossFadeAlpha(0, 0f, false);

			if (string.IsNullOrEmpty(assetSource))
			{
				ResetImage(imageComponent);
				yield break;
			}

			if (context != null && routineDict.ContainsKey(context))
			{
				if (requestsDict.ContainsKey(context))
				{
					SpriteUtils.CleanRequestBySourceUri(requestsDict[context]);
					requestsDict.Remove(context);
				}

				context.StopCoroutine(routineDict[context]);
				routineDict.Remove(context);
			}

			IEnumerator getTextureRoutine = SpriteUtils.GetTextureFromSource(assetSource, (result) => ApplyTexture(imageComponent, envelopeParent, result, crossFadeAlphaDuration, enableImageOnLoaded, alphaOnLoaded), requestKey);

			if (context != null) routineDict.Add(context, getTextureRoutine);
			if (context != null) requestsDict.Add(context, requestKey);

			yield return getTextureRoutine;

			if (context != null && routineDict.ContainsKey(context)) routineDict.Remove(context);
			if (context != null && requestsDict.ContainsKey(context)) requestsDict.Remove(context);
		}

		public static void ResetImage(RawImage imageComponent)
		{
			if (imageComponent)
			{
				imageComponent.texture = null;
				imageComponent.enabled = false;
			}
		}

		private static void ApplyTexture(RawImage imageComponent, bool envelopeParent, Texture texture, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
		{
			if (texture != null && imageComponent != null)
			{
				imageComponent.texture = texture;
				imageComponent.enabled = true;

				if (imageComponent.GetComponent<ImageAspectRatioSetter>() != null)
				{
					imageComponent.gameObject.AddComponent<ImageAspectRatioSetter>().InitSetter();
				}

				if (imageComponent.TryGetComponent(out AspectRatioFitter aspectRatioFitter))
				{
					if (envelopeParent)
					{
						// Envelope parent method (centered and cropped)
						aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
					}
					else
					{
						// Fit in parent
						aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
					}
				}
				else
				{
					UnityEngine.Debug.LogWarning("[ImageUtils.SetImage] - Need 'AspectRatioFitter' component");
				}

				if (enableImageOnLoaded) imageComponent.CrossFadeAlpha(alphaOnLoaded, crossFadeAlphaDuration, false);
				else imageComponent.enabled = false;
			}
		}

		public async static void LoadImage(RawImage imageComponent, MonoBehaviour monoBehaviourInstance, Wezit.Node wzData, string targetRelation = "relationForShowPicture", string targetWzSourceTransformation = "default", bool envelopeParent = true, float crossFadeAlphaDuration = 0.25f, string type = "")
		{
			if (imageComponent != null)
			{
				if (wzData != null)
				{
					bool hasImage = false;
					await wzData.GetRelations(wzData);
					foreach (Wezit.Relation relation in wzData.Relations)
					{
						if (relation.relation == targetRelation)
						{
							if (string.IsNullOrEmpty(type) || (!string.IsNullOrEmpty(type) && relation.type == type))
                            {
								hasImage = true;
								monoBehaviourInstance.StartCoroutine(SetImage(
									imageComponent,
									relation.GetAssetSourceByTransformation(targetWzSourceTransformation),
									relation.GetAssetMimeTypeByTransformation(targetWzSourceTransformation),
									envelopeParent,
									null,
									crossFadeAlphaDuration));
								break;
                            }
						}
					}
					if (!hasImage) ResetImage(imageComponent);
				}
				else
				{
					ResetImage(imageComponent);
				}
			}
		}

		public async static void LoadCover(RawImage imageComponent, MonoBehaviour monoBehaviourInstance, Wezit.Node wzData, string targetRelation, string targetWzSourceTransformation = "default", bool envelopeParent = true, float crossFadeAlphaDuration = 0.25f)
		{
			if (imageComponent != null)
			{
				if (wzData != null)
				{
					bool hasMedia = false;
					bool hasCover = false;
					await wzData.GetRelations(wzData);
					foreach (Wezit.Relation relation in wzData.Relations)
					{
						if (relation.relation == targetRelation)
						{
							hasMedia = true;
							Wezit.WezitAssets.Asset asset = CoverStore.GetCoverAssetForPid(relation.pid);
							if(asset != null)
                            {
								hasCover = true;
								monoBehaviourInstance.StartCoroutine(SetImage(
									imageComponent,
									asset.GetAssetSourceByTransformation(targetWzSourceTransformation),
									asset.GetAssetMimeTypeByTransformation(targetWzSourceTransformation),
									envelopeParent,
									null,
									crossFadeAlphaDuration));
								break;
                            }
						}
					}
					if (!hasMedia || !hasCover) ResetImage(imageComponent);
				}
				else
				{
					ResetImage(imageComponent);
				}
			}
		}
		#endregion
	}
}
