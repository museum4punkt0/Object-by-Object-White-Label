using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Graphic), typeof(AspectRatioFitter))]
public class ImageAspectRatioSetter : MonoBehaviour
{
    private Graphic m_Graphic = null;
    private AspectRatioFitter m_AspectRatioFitter = null;
    private Texture m_CurrentTexture = null;

    private void OnEnable() 
    {
        m_Graphic = GetComponent<Graphic>();
        m_AspectRatioFitter = GetComponent<AspectRatioFitter>();
        if (m_Graphic != null)
        {
            if (m_CurrentTexture != m_Graphic.mainTexture && m_Graphic.mainTexture != null)
            {
                m_CurrentTexture = m_Graphic.mainTexture;
                float newRatio = (float)m_CurrentTexture.width / m_CurrentTexture.height;
                if (m_AspectRatioFitter != null)
                {
                    m_AspectRatioFitter.aspectRatio = newRatio;
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Graphic != null)
        {
            if (m_CurrentTexture != m_Graphic.mainTexture && m_Graphic.mainTexture != null)
            {
                m_CurrentTexture = m_Graphic.mainTexture;
                float newRatio = (float)m_CurrentTexture.width / m_CurrentTexture.height;

                if (m_AspectRatioFitter != null)
                {
                    m_AspectRatioFitter.aspectRatio = newRatio;
                }
            }
        }
    }

    public void InitSetter()
    {
        if (m_Graphic != null)
        {
            if (m_CurrentTexture != m_Graphic.mainTexture && m_Graphic.mainTexture != null)
            {
                m_CurrentTexture = m_Graphic.mainTexture;
                float newRatio = (float)m_CurrentTexture.width / m_CurrentTexture.height;

                if (m_AspectRatioFitter != null)
                {
                    m_AspectRatioFitter.aspectRatio = newRatio;
                }
            }
        }
    }
}
