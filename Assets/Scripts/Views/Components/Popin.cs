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
    [SerializeField] private GameObject _iconRoot;
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
    public void Inflate(string title, string description, string buttonText, string settingKey = "", string iconType = "")
    {
        gameObject.SetActive(true);
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Darken);

        if (!string.IsNullOrEmpty(Wezit.Settings.Instance.GetSetting(settingKey)))
        {
            _iconRoot.SetActive(true);
            Wezit.Settings.Instance.SetImageFromSetting(_icon, settingKey);
        }
        else if (!string.IsNullOrEmpty(iconType) && StoreAccessor.State.SelectedTourBank != null)
        {
            _iconRoot.SetActive(true);
            Wezit.Poi bank = StoreAccessor.State.SelectedTourBank;
            Utils.ImageUtils.LoadImage(_icon, this, bank, Wezit.RelationName.SHOW_PICTURE, WezitSourceTransformation.original, false, 0.1f, iconType);
        }
        else
        {
            _iconRoot.SetActive(false);
        }

        _title.text = title;
        _description.text = description;
        _popinButtonText.text = buttonText;
        _popinButtonBG.color = _closeButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

        string[] paragraphs = { description };
        _contrastButton.Inflate(title, paragraphs, _contrastPanelRoot);

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(Close);

        _popinButton.onClick.RemoveAllListeners();
        _popinButton.onClick.AddListener(OnPopinButton);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.SetPreviousStatus();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Darken);
    }
    #endregion

    #region Private
    private void OnPopinButton()
    {
        PopinButtonClicked?.Invoke();
    }
    #endregion
    #endregion
}
