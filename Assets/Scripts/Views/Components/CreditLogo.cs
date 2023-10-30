using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UniRx;

public class CreditLogo : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private RawImage _logo;
    [SerializeField] private Button _button;
    #endregion
    #region Private
    private string m_url;
    private RectTransform m_rectTransform;
    #endregion
    #endregion

    #region Properties
    public UnityEvent LogoResized = new UnityEvent();
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public async void Inflate(string imageSource, string url, MonoBehaviour activeMonoBehaviour)
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_url = url;
        _button.onClick.AddListener(OnLogoClick);

        await activeMonoBehaviour.StartCoroutine(Utils.ImageUtils.SetImage(_logo, imageSource, "", false));
        await Task.Delay(TimeSpan.FromSeconds(0.1f));

        float newHeight = _logo.texture.height * m_rectTransform.sizeDelta.x / _logo.texture.width;
        m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, newHeight);

        LogoResized?.Invoke();
    }
    #endregion
    #region Private
    private void OnLogoClick()
    {
        if(!string.IsNullOrEmpty(m_url))
        {
            Application.OpenURL(m_url);
        }
    }
    #endregion
    #endregion
}
