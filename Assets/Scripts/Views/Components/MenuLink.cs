using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MenuLink : MonoBehaviour
{
    #region Fields
    #region SerializeField
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private GameObject _separatorRoot;
    [SerializeField] private Button _button;
    #endregion
    #region Private
    private KioskState m_KioskState;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<KioskState> MenuLinkClicked = new();
    #endregion

    #region Methods
    #region Monobehaviour
    #endregion
    #region Public
    public void Inflate(string label, KioskState state, bool enableSeparator)
    {
        m_KioskState = state;
        name = _label.text = label;
        _separatorRoot.SetActive(enableSeparator);

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnButtonClicked);
    }
    #endregion
    #region Private
    private void OnButtonClicked()
    {
        MenuLinkClicked?.Invoke(m_KioskState);
    }
    #endregion
    #endregion
}
