using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopinPoints : MonoBehaviour
{
    #region Fields
    #region Serialize Fields
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Image _closeButtonBG = null;
    [SerializeField] private TMPro.TextMeshProUGUI _title;
    [SerializeField] private TMPro.TextMeshProUGUI _poiName;
    [SerializeField] private TMPro.TextMeshProUGUI _description;
    [Space]
    [SerializeField] private Image _iconBG;
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _pictureSprite;
    [SerializeField] private Sprite _audioSprite;
    [SerializeField] private Sprite _videoSprite;
    [SerializeField] private Sprite _3DSprite;
    [Space]
    [SerializeField] private ContrastButton _contrastButton;
    [SerializeField] private Transform _contrastPanelRoot;
    #endregion
    #region Private
    private string m_PointsTitleSettingKey = "points.earned.title.text";
    private string m_PointsDescriptionSettingKey = "points.earned.title.description";
    #endregion
    #endregion

    #region Properties
    public UnityEvent PopinButtonClicked = new UnityEvent();
    #endregion

    #region Methods
    #region Public
    public void Inflate(Wezit.Poi poi)
    {
        gameObject.SetActive(true);
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Darken);

        switch (poi.type)
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

        string pointsEarned = Wezit.Settings.Instance.GetSettingAsCleanedText(m_PointsTitleSettingKey, StoreAccessor.State.Language);
        pointsEarned = "{0} points earned";
        _title.text = string.Format(pointsEarned, GlobalSettingsManager.Instance.PointsEarnedContent);
        _poiName.text = StringUtils.CleanFromWezit(poi.title);
        _description.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_PointsDescriptionSettingKey, StoreAccessor.State.Language); ;
        _iconBG.color = _closeButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

        string[] paragraphs = { _poiName.text, _description.text };
        _contrastButton.Inflate(_title.text, paragraphs, _contrastPanelRoot);

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(OnCloseButton);
    }
    #endregion

    #region Private
    private void OnCloseButton()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.SetPreviousStatus();
    }
    #endregion
    #endregion
}
