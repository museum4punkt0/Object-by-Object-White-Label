using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class QuizIntermediateScreen : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Button _okayButton;
    [SerializeField] private GameObject _textContainer;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    #endregion
    #region Private

    #endregion
    #endregion

    #region Properties
    public UnityEvent OkayClicked = new UnityEvent();
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(Wezit.QuizAnswerModel answer)
    {
        gameObject.SetActive(true);
        _title.text = answer.IntermediateScreenTitle;
        _description.text = answer.IntermediateScreenDescription;
        StartCoroutine(Utils.LayoutGroupRebuilder.Rebuild(_textContainer));

        _okayButton.onClick.RemoveAllListeners();
        _okayButton.onClick.AddListener(OnOkayButtonClicked);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    #endregion
    #region Private
    private void OnOkayButtonClicked()
    {
        OkayClicked?.Invoke();
        gameObject.SetActive(false);
    }
    #endregion
    #endregion
}
