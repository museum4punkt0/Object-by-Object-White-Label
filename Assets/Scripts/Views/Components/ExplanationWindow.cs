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
    [SerializeField] private AudioDescription _audiodescButton;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _bottom;
    #endregion
    #region Private
    private Coroutine m_MovementCoroutine;

    private float m_TopPos;
    #endregion
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(string contrastTitle, string[] contrastParagraphs, Transform contrastPanelRoot, string audioDescriptionSource = "")
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

        bool hasAudioDesc = !string.IsNullOrEmpty(audioDescriptionSource);
        _audiodescButton.gameObject.SetActive(hasAudioDesc);
        if(hasAudioDesc)
        {
            _audiodescButton.Inflate(audioDescriptionSource);
        }

        m_TopPos = Mathf.Min(_transparentFiller.sizeDelta.y, _transparentFiller.sizeDelta.y + _explanationPanel.sizeDelta.y - Screen.safeArea.height + 200);
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

    public void Rebuild()
    {
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_bottom));
        StartCoroutine(Utils.LayoutGroupRebuilder.DisableEnable(_bottom, 2, 0.1f));
    }
    #endregion

    #region Private
    private void OnOpenButton(bool isOpen)
    {
        m_TopPos = Mathf.Min(_transparentFiller.sizeDelta.y, _transparentFiller.sizeDelta.y + _explanationPanel.sizeDelta.y - Screen.safeArea.height + 200);
        m_MovementCoroutine = StartCoroutine(OpenExplanation(isOpen));
        _openArrow.Rotate(180, 0, 0);
    }

    private IEnumerator OpenExplanation(bool isOpen)
    {
        float goalHeight = isOpen ? 0 : m_TopPos;
        Vector2 prevPos = _explanationContent.localPosition;
        Vector2 currentPos = Vector2.negativeInfinity;
        while (((isOpen && _explanationContent.localPosition.y > goalHeight) || (!isOpen && _explanationContent.localPosition.y < goalHeight)) && (currentPos != prevPos))
        {
            _explanationContent.Translate(0, isOpen ? -100 : 100, 0);
            prevPos = currentPos;
            currentPos = _explanationContent.localPosition;
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
