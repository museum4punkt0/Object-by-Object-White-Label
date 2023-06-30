using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Wezit
{
    public class QuizAnswer : MonoBehaviour
    {
        #region Fields
        #region SerializeFields
        [SerializeField] private TextMeshProUGUI _answerText;
        [SerializeField] private ButtonWithSelectEvent _button;
        [SerializeField] private Image _outline;
        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;
        [SerializeField] private Color _correctColor;
        [SerializeField] private Sprite _correctSprite;
        [SerializeField] private Color _wrongColor;
        [SerializeField] private Sprite _wrongSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _unselectedSprite;
        #endregion
        #region Private
        private QuizAnswerModel m_Answer;
        #endregion
        #endregion

        #region Properties
        public UnityEvent<QuizAnswerModel> AnswerClicked = new UnityEvent<QuizAnswerModel>();
        #endregion

        #region Methods
        #region Monobehaviour
        #endregion

        #region Public
        public void Inflate(QuizAnswerModel answer)
        {
            m_Answer = answer;
            _answerText.text = StringUtils.CleanFromWezit(m_Answer.AnswerText);
            _button.onClick.AddListener(OnButtonClicked);
            _button.ButtonSelected.AddListener(OnButtonSelected);


            _icon.sprite = _unselectedSprite;
            _outline.color = _answerText.color = _icon.color = GlobalSettingsManager.Instance.AppColor;
            _background.color = Color.white;
        }

        public void DisableButton()
        {
            _button.enabled = false;
        }
        #endregion
        #region Internal
        #endregion
        #region Private
        private void OnButtonClicked()
        {
            AnswerClicked?.Invoke(m_Answer);
            _icon.sprite = m_Answer.IsCorrect ? _correctSprite : _wrongSprite;
            _icon.color = Color.white;
            _answerText.color = Color.white;
            _background.color = m_Answer.IsCorrect ? _correctColor : _wrongColor;
            _outline.color = m_Answer.IsCorrect ? _correctColor : _wrongColor;
        }

        private void OnButtonSelected(bool selected)
        {
            _icon.sprite = selected ? _selectedSprite : _unselectedSprite;
            _icon.color = selected ? Color.white : GlobalSettingsManager.Instance.AppColor;
            _answerText.color = selected ? Color.white : GlobalSettingsManager.Instance.AppColor;
            _background.color = selected ? GlobalSettingsManager.Instance.AppColor : Color.white;
        }
        #endregion
        #endregion
    }
}
