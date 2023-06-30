using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MapListToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle = null;
    [Header("Map")]
    [SerializeField] private Image _mapBackground = null;
    [SerializeField] private Image _mapIcon = null;
    [SerializeField] private TextMeshProUGUI _mapText = null;
    [Header("List")]
    [SerializeField] private Image _listBackground = null;
    [SerializeField] private Image _listIcon = null;
    [SerializeField] private TextMeshProUGUI _listText = null;

    private Color m_OnColor;
    private Color m_OffColor = Color.white;
    private string m_MapTextSettingKey = "template.spk.maps.common.selector.map.text";
    private string m_ListTextSettingKey = "template.spk.maps.common.selector.list.text";

    public UnityEvent<bool> ToggleValueChanged = new UnityEvent<bool>();

    public void Reset()
    {
        _toggle.isOn = false;
        _toggle.isOn = true;
    }

    private void Start()
    {
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener(OnValueChanged);
        m_OnColor = GlobalSettingsManager.Instance.AppColor;

        _mapBackground.color = m_OnColor;
        _mapIcon.color = m_OffColor;
        _mapText.color = m_OffColor;
        _mapText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_MapTextSettingKey);

        _listBackground.color = m_OffColor;
        _listIcon.color = m_OnColor;
        _listText.color = m_OnColor;
        _listText.text = Wezit.Settings.Instance.GetSettingAsCleanedText(m_ListTextSettingKey);
    }

    private void OnValueChanged(bool isOn)
    {
        _mapBackground.color = isOn ? m_OnColor : m_OffColor;
        _mapIcon.color = isOn ? m_OffColor : m_OnColor;
        _mapText.color = isOn ? m_OffColor : m_OnColor;

        _listBackground.color = isOn ? m_OffColor : m_OnColor;
        _listIcon.color = isOn ? m_OnColor : m_OffColor;
        _listText.color = isOn ? m_OnColor : m_OffColor;

        ToggleValueChanged?.Invoke(isOn);
    }
}
