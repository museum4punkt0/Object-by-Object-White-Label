using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionBars : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Image _stepPrefab = null;
    [SerializeField] private Transform _stepRoot = null;
    [SerializeField] private Image _star = null;
    #endregion
    #region Private
    private Color m_InactiveColor = new Color(0.5f, 0.5f, 0.5f);
    private Color m_ActiveColor = Color.black;
    #endregion
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(int numberOfSteps, int progress)
    {
        m_ActiveColor = GlobalSettingsManager.Instance.AppColor;
        foreach(Transform child in _stepRoot)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfSteps; i++)
        {
            Instantiate(_stepPrefab, _stepRoot).color = i < progress ? m_ActiveColor : m_InactiveColor;
        }
        _star.color = numberOfSteps == progress ? m_ActiveColor : m_InactiveColor;
    }
    #endregion
    #region Private
    #endregion
    #endregion
}
