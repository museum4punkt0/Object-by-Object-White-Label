using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Numpad : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private TextMeshProUGUI m_SelectionText = null;
    [SerializeField] private GameObject m_CodeRoot = null;
    [SerializeField] private Button[] m_DigitButtons = null;
    [SerializeField] private Button m_ConfirmButton = null;
    [SerializeField] private Button m_BackButton = null;
    [SerializeField] private Button m_ExitButton = null;
    #endregion
    #region Private
    private int m_Selection = 0;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<int> NumpadEnter = new UnityEvent<int>();
    #endregion

    #region Methods
    #region MonoBehaviours
    void Awake()
    {
        CloseNumpad();
        for (int i = 0; i < m_DigitButtons.Length; i++)
        {
            int j = i; // avoid reference problems
            m_DigitButtons[j].onClick.AddListener(delegate { InputDigit(j); });
        }
        m_BackButton.onClick.AddListener(Delete);
        m_ConfirmButton.onClick.AddListener(Confirm);
        m_ExitButton.onClick.AddListener(CloseNumpad);
    }
    #endregion

    #region Private
    private void CloseNumpad()
    {
        m_CodeRoot.SetActive(false);
        m_Selection = 0;
        m_SelectionText.text = "";
    }

    private void InputDigit(int digit)
    {
        if (m_Selection > Mathf.Pow(10, 6)) return;
        m_Selection = m_Selection == 0 ? digit : m_Selection * 10 + digit;
        m_SelectionText.text = m_Selection.ToString();
    }

    private void Delete()
    {
        if (m_Selection > 0)
        {
            m_Selection = Mathf.FloorToInt(m_Selection / 10f);
            m_SelectionText.text = m_Selection.ToString();
            if (m_Selection == 0) m_SelectionText.text = "";
        }
        else m_SelectionText.text = "";
    }

    private void Confirm()
    {
        NumpadEnter?.Invoke(m_Selection);
        m_Selection = 0;
        m_SelectionText.text = "";
    }
    #endregion
    #endregion
}
