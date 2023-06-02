using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ExplanationWindow : MonoBehaviour
{
    #region Fields
    #region Serialize Fields
    [SerializeField] private Transform _explanationContent = null;
    [SerializeField] private Toggle _openToggle = null;
    [SerializeField] private Image _openToggleBG = null;
    [SerializeField] private Transform _openArrow = null;
    [SerializeField] private RectTransform _transparentFiller = null;
    [SerializeField] private RectTransform _explanationPanel = null;
    [SerializeField] private ContrastButton _contrastButton = null;
    #endregion
    #region Private
    #endregion
    #endregion

    #region Methods
    #region Public
    public void Inflate(string contrastTitle, string[] contrastParagraphs, Transform contrastPanelRoot)
    {
        _explanationContent.localPosition = Vector3.zero;
        _openToggleBG.color = GlobalSettingsManager.Instance.AppColor;
        _openToggle.onValueChanged.RemoveAllListeners();
        _openToggle.onValueChanged.AddListener(OnOpenButton);
        _contrastButton.Inflate(contrastTitle, contrastParagraphs, contrastPanelRoot);
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_explanationContent.gameObject));
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_explanationPanel.gameObject));
    }
    #endregion

    #region Private

    private void OnOpenButton(bool isOpen)
    {
        StartCoroutine(OpenExplanation(isOpen));
        _openArrow.Rotate(180, 0, 0);
    }

    private IEnumerator OpenExplanation(bool isOpen)
    {
        float goalHeight = isOpen ? 0 : Mathf.Min(_transparentFiller.sizeDelta.y, _transparentFiller.sizeDelta.y + _explanationPanel.sizeDelta.y - 2200);
        while ((isOpen && _explanationContent.localPosition.y > goalHeight) || (!isOpen && _explanationContent.localPosition.y < goalHeight))
        {
            _explanationContent.Translate(0, isOpen ? -50 : 50, 0);
            yield return null;
        }
        yield return null;
    }
    #endregion
    #endregion
}
