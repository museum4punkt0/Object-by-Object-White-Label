using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Popin : MonoBehaviour
{
    #region Fields
    #region Serialize Fields
    [SerializeField] internal Button _closeButton = null;
    [SerializeField] private Image _closeButtonBG = null;
    [SerializeField] internal Button _popinButton = null;
    [SerializeField] internal Image _popinButtonBG = null;
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private TMPro.TextMeshProUGUI _popinButtonText;
    [SerializeField] internal TMPro.TextMeshProUGUI _title;
    [SerializeField] internal TMPro.TextMeshProUGUI _description;
    [SerializeField] private GameObject _iconRoot;
    [SerializeField] internal RawImage _icon;
    [SerializeField] private ContrastButton _contrastButton;
    [SerializeField] private Transform _contrastPanelRoot;
    #endregion
    #region Private
    #endregion
    #endregion

    #region Properties
    public UnityEvent PopinButtonClicked = new UnityEvent();
    public UnityEvent PopinClosed = new UnityEvent();
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
            Utils.ImageUtils.LoadImage(_icon, this, bank, Wezit.RelationName.SHOW_PICTURE, WezitSourceTransformation.default_base, false, 0.1f, iconType);
        }
        else
        {
            _iconRoot.SetActive(false);
        }

        _title.text = title;
        _description.text = description;
        _popinButtonText.text = buttonText;
        _popinButtonBG.color = _closeButtonBG.color = _title.color = GlobalSettingsManager.Instance.AppColor;

        if(gameObject.activeInHierarchy)
        {
            StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_panelRoot));
        }

        string[] paragraphs = { description };
        _contrastButton.Inflate(title, paragraphs, _contrastPanelRoot);

        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(OnCloseButton);

        _popinButton.onClick.RemoveAllListeners();
        _popinButton.onClick.AddListener(OnPopinButton);
    }

    public void Close(bool invokeEvent = true)
    {
        if(invokeEvent)
        {
            PopinClosed?.Invoke();
        }
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
    internal void OnPopinButton()
    {
        PopinButtonClicked?.Invoke();
    }

    private void OnCloseButton()
    {
        Close(true);
    }
    #endregion
    #endregion
}
