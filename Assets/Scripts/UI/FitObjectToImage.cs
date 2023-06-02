using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class FitObjectToImage : MonoBehaviour
{

    private Image _image;
    private float _width;

    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
        _width = _image.mainTexture.width*(_image.rectTransform.rect.height/_image.mainTexture.height);
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width,_image.rectTransform.rect.height);
    }

    public void ResizeImage()
    {
        _image = GetComponent<Image>();
        _width = _image.mainTexture.width*(_image.rectTransform.rect.height/_image.mainTexture.height);
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_width,_image.rectTransform.rect.height);
    }
}
