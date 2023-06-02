using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Wezit;
using TMPro;

public class InventoryItemContent : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Button _button;
    [Space]
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _pictureSprite;
    [SerializeField] private Sprite _audioSprite;
    [SerializeField] private Sprite _videoSprite;
    [SerializeField] private Sprite _3DSprite;
    [Space]
    [SerializeField] private TextMeshProUGUI _title;
    #endregion
    #region Private
    private Poi m_Poi;
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(Poi poi)
    {
        m_Poi = poi;
        _button.onClick.AddListener(OnButtonClick);
        _title.color = _icon.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = StringUtils.CleanFromWezit(poi.title);
        switch(poi.type)
        {
            case "picture":
                _icon.sprite = _pictureSprite;
                break;
            case "audio":
                _icon.sprite = _audioSprite;
                break;
            case "video":
                _icon.sprite = _videoSprite;
                break;
            case "_3D":
                _icon.sprite = _3DSprite;
                break;
        }
    }
    #endregion
    #region Private
    private void OnButtonClick()
    {
        AppManager.Instance.SelectPoi(m_Poi);
        AppManager.Instance.GoToState(KioskState.CONTENT);
    }
    #endregion
    #endregion
}
