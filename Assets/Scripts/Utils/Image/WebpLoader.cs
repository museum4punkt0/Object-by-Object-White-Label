using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using unity.libwebp;
using unity.libwebp.Interop;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebP;
using UniRx;
using UniRx.Async;
using System.Threading.Tasks;

public class WebpAnimation : MonoBehaviour
{
    #region Fields
    #region Private
    private static bool m_Flip;
    #endregion
    #endregion

    #region Methods
    #region Public
    public static async void LoadAndApplyWebp(string uri, RawImage rawImage, bool fillParent, bool loop = false, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
    {
        Debug.Log("WebpLoader - GetWebpFromSource - uri : " + uri);
        LoadAnimation(await LoadBytes(uri), rawImage, fillParent, loop, crossFadeAlphaDuration, enableImageOnLoaded, alphaOnLoaded);
    }
    #endregion
    #region Private
    private static async UniTask<byte[]> LoadBytes(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            await webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    return webRequest.downloadHandler.data;
            }
        }
        return null;
    }

    private static unsafe void LoadAnimation(byte[] imageBytes, RawImage rawImage, bool fillParent, bool loop, float crossFadeAlphaDuration, bool enableImageOnLoaded, float alphaOnLoaded)
    {
        List<(Texture2D frameTexture, int frameIndex)> webPFrames = new List<(Texture2D, int)>();

        WebPAnimDecoderOptions option = new WebPAnimDecoderOptions
        {
            use_threads = 1,
            color_mode = WEBP_CSP_MODE.MODE_RGBA,
        };
        option.padding[5] = 1;

        NativeLibwebpdemux.WebPAnimDecoderOptionsInit(&option);
        fixed (byte* bytes = imageBytes)
        {
            WebPData webpdata = new WebPData
            {
                bytes = bytes,
                size = new UIntPtr((uint)imageBytes.Length)
            };
            WebPAnimDecoder* dec = NativeLibwebpdemux.WebPAnimDecoderNew(&webpdata, &option);
            //dec->config_.options.flip = 1;

            WebPAnimInfo anim_info = new WebPAnimInfo();

            NativeLibwebpdemux.WebPAnimDecoderGetInfo(dec, &anim_info);

            Debug.Log($"{anim_info.frame_count} {anim_info.canvas_width}/{anim_info.canvas_height}");

            uint size = anim_info.canvas_width * 4 * anim_info.canvas_height;

            int timestamp = 0;

            IntPtr pp = new IntPtr();
            byte** unmanagedPointer = (byte**)&pp;
            for (int i = 0; i < anim_info.frame_count; ++i)
            {
                int result = NativeLibwebpdemux.WebPAnimDecoderGetNext(dec, unmanagedPointer, &timestamp);
                Assert.AreEqual(1, result);

                int lWidth = (int)anim_info.canvas_width;
                int lHeight = (int)anim_info.canvas_height;
                bool lMipmaps = false;
                bool lLinear = false;

                Texture2D texture = Texture2DExt.CreateWebpTexture2D(lWidth, lHeight, lMipmaps, lLinear);
                texture.LoadRawTextureData(pp, (int)size);

                if (m_Flip)
                {
                    {// Flip updown.
                     // ref: https://github.com/netpyoung/unity.webp/issues/25
                     // ref: https://github.com/netpyoung/unity.webp/issues/21
                     // ref: https://github.com/webmproject/libwebp/blob/master/src/demux/anim_decode.c#L309
                        Color[] pixels = texture.GetPixels();
                        Color[] pixelsFlipped = new Color[pixels.Length];
                        for (int y = 0; y < anim_info.canvas_height; y++)
                        {
                            Array.Copy(pixels, y * anim_info.canvas_width, pixelsFlipped, (anim_info.canvas_height - y - 1) * anim_info.canvas_width, anim_info.canvas_width);
                        }
                        texture.SetPixels(pixelsFlipped);
                    }
                }

                texture.Apply();
                webPFrames.Add((texture, timestamp));
            }
            NativeLibwebpdemux.WebPAnimDecoderReset(dec);
            NativeLibwebpdemux.WebPAnimDecoderDelete(dec);
        }

        ApplyTexture(rawImage, fillParent, webPFrames[0].frameTexture, crossFadeAlphaDuration, enableImageOnLoaded, alphaOnLoaded);
        if (webPFrames.Count > 1) PlayAnimation(rawImage, webPFrames, loop);
        return;
    }

    private static async UniTask PlayAnimation(RawImage rawImage, List<(Texture2D, int)> animationList, bool loop = false)
    {
        int prevTimestamp = 0;
        for (int i = 0; i < animationList.Count; ++i)
        {
            (Texture2D texture, int timestamp) = animationList[i];
            if (rawImage == null)
            {
                return;
            }
            rawImage.texture = texture;
            int delay = timestamp - prevTimestamp;
            prevTimestamp = timestamp;

            if (delay < 0)
            {
                delay = 0;
            }

            await Task.Delay(delay);
            if (i == animationList.Count - 1 && animationList.Count > 1 && loop)
            {
                i = -1;
            }
        }
    }

    private static void ApplyTexture(RawImage rawImage, bool fillParent, Texture texture, float crossFadeAlphaDuration = 0.5f, bool enableImageOnLoaded = true, float alphaOnLoaded = 1)
    {
        if (texture != null && rawImage != null)
        {
            Transform rawImageTransform = rawImage.transform;
            rawImageTransform.localScale = new Vector3(rawImageTransform.localScale.x,
                                                       rawImageTransform.localScale.y < 0 ? rawImageTransform.localScale.y : -rawImageTransform.localScale.y,
                                                       rawImageTransform.localScale.z);
            rawImage.texture = texture;
            rawImage.enabled = true;

            if (rawImage.GetComponent<ImageAspectRatioSetter>() != null)
            {
                rawImage.gameObject.AddComponent<ImageAspectRatioSetter>().InitSetter();
            }

            if (rawImage.TryGetComponent<AspectRatioFitter>(out AspectRatioFitter aspectRatioFitter))
            {
                if (fillParent)
                {
                    // Fill parent method (centered and cropped)
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

            if (enableImageOnLoaded) rawImage.CrossFadeAlpha(alphaOnLoaded, crossFadeAlphaDuration, false);
            else rawImage.enabled = false;
        }
    }
    #endregion
    #endregion

}
