using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ExplanationWindow : MonoBehaviour, IBeginDragHandler
{
    #region Fields
    #region Serialize Fields
    [SerializeField] private Transform _explanationContent;
    [SerializeField] private Toggle _openToggle;
    [SerializeField] private Image _openToggleBG;
    [SerializeField] private Transform _openArrow;
    [SerializeField] private RectTransform _transparentFiller;
    [SerializeField] private RectTransform _explanationPanel;
    [SerializeField] private ContrastButton _contrastButton;
    [SerializeField] private ScrollRect _scrollRect;
    #endregion
    #region Private
    private Coroutine m_MovementCoroutine;

    private float m_TopPos;
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
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_explanationContent.gameObject));
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_explanationPanel.gameObject));
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_explanationPanel.gameObject));

        m_TopPos = Mathf.Min(_transparentFiller.sizeDelta.y, _transparentFiller.sizeDelta.y + _explanationPanel.sizeDelta.y - 2200);
        _openArrow.localEulerAngles = Vector3.zero;

        _scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_MovementCoroutine != null)
        {
            StopCoroutine(m_MovementCoroutine);
            m_MovementCoroutine = null;
        }
    }
    #endregion

    #region Private
    private void OnOpenButton(bool isOpen)
    {
        m_MovementCoroutine = StartCoroutine(OpenExplanation(isOpen));
        _openArrow.Rotate(180, 0, 0);
    }

    private IEnumerator OpenExplanation(bool isOpen)
    {
        float goalHeight = isOpen ? 0 : m_TopPos;
        while ((isOpen && _explanationContent.localPosition.y > goalHeight) || (!isOpen && _explanationContent.localPosition.y < goalHeight))
        {
            _explanationContent.Translate(0, isOpen ? -100 : 100, 0);
            yield return null;
        }
        yield return null;
        m_MovementCoroutine = null;
    }

    private void OnScrollRectValueChanged(Vector2 position)
    {
        bool isAtBottom = _explanationContent.localPosition.y <= 1;
        bool isAtTop = _explanationContent.localPosition.y >= m_TopPos;
        if(isAtBottom || isAtTop)
        {
            if (isAtBottom)
            {
                _openToggle.SetIsOnWithoutNotify(true);
                _openArrow.localEulerAngles = Vector3.zero;
            }
            else
            {
                if (isAtTop)
                {
                    _openToggle.SetIsOnWithoutNotify(false);
                    _openArrow.localEulerAngles = 180f * Vector3.right;
                }
            }
        }
    }
    #endregion
    #endregion
}
