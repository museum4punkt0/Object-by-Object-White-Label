using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicAlphaSetter : MonoBehaviour
{
    [SerializeField] private Graphic _graphic = null;
    [SerializeField] private float _alpha = 1f;
    [SerializeField] private bool _setOnEnable = false;

    public void SetAlpha()
    {
        if (_graphic)
        {
            Color graphicColor = _graphic.color;
            graphicColor.a = _alpha;
            _graphic.color = graphicColor;
        }
    }

    private void OnEnable() 
    {
        if (_setOnEnable)
        {
            SetAlpha();
        }
    }
}
