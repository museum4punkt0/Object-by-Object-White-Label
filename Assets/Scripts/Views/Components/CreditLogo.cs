using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditLogo : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private RawImage _logo;
    [SerializeField] private Button _button;
    #endregion
    #region Private
    private string m_url;
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(string imageSource, string url)
    {
        m_url = url;
        _button.onClick.AddListener(OnLogoClick);
        Utils.ImageUtils.SetImage(_logo, imageSource, "", false);
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
