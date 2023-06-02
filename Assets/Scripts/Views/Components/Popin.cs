using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Popin : MonoBehaviour
{
    #region Fields
    #region Serialize Fields
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Image _closeButtonBG = null;
    [SerializeField] private Button _popinButton = null;
    [SerializeField] private Image _popinButtonBG = null;
    [SerializeField] private TMPro.TextMeshProUGUI _popinButtonText;
    [SerializeField] private TMPro.TextMeshProUGUI _title;
    [SerializeField] private TMPro.TextMeshProUGUI _description;
    [SerializeField] private RawImage _icon;
    [SerializeField] private ContrastButton _contrastButton;
    [SerializeField] private Transform _contrastPanelRoot;
    #endregion
    #region Private
    #endregion
    #endregion

    #region Properties
    public UnityEvent PopinButtonClicked = new UnityEvent();
    #endregion

    #region Methods
    #region Public
    public void Inflate(string title, string description, string buttonText, string settingKey = "", string iconTag = "")
    {
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Darken);

        if (!string.IsNullOrEmpty(Wezit.Settings.Instance.GetSetting(settingKey)))
        {
            _icon.transform.parent.gameObject.SetActive(true);
            Wezit.Settings.Instance.SetImageFromSetting(_icon, settingKey);
        }
        else if(!string.IsNullOrEmpty(iconTag))
        {

        }
        else
        {
            _icon.transform.parent.gameObject.SetActive(false);
        }

        _title.text = title;
        _description.text = description;
        _popinButtonText.text = buttonText;
        _popinButtonBG.color = _closeButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

        string[] paragraphs = { description };
        _contrastButton.Inflate(title, paragraphs, _contrastPanelRoot);

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(OnCloseButton);

        _popinButton.onClick.RemoveAllListeners();
        _popinButton.onClick.AddListener(OnPopinButton);
    }
    #endregion

    #region Private
    private void OnCloseButton()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.SetPreviousStatus();
    }

    private void OnPopinButton()
    {
        PopinButtonClicked?.Invoke();
    }
    #endregion
    #endregion
}
