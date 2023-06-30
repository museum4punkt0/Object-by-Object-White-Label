using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Wezit
{
    public class Quiz : Activity
    {
        #region Fields
        #region SerializeFields
        [SerializeField] private QuizAnswer _answerPrefab;
        [SerializeField] private Transform _answersRoot;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _instruction;
        [SerializeField] private RawImage _questionImage;
        [SerializeField] private QuizIntermediateScreen _intermediateScreen;
        [Space]
        [SerializeField] private ContrastButton _contrastButton;
        [SerializeField] private Transform _contrastPanelRoot;
        #endregion
        #region Private
        private List<QuizQuestionModel> m_Questions = new List<QuizQuestionModel>();
        private List<QuizAnswer> m_Answers = new List<QuizAnswer>();

        private int m_QuestionIndex;
        private int m_CurrentWins;
        private float m_WinThreshold = 1f;
        #endregion
        #endregion

        #region Properties
        public UnityEvent<bool> QuizOver = new UnityEvent<bool>();
        #endregion

        #region Methods
        #region Monobehaviour
        #endregion

        #region Public
        public override void Inflate(JSONNode activityNode, Language language)
        {
            base.Inflate(activityNode, language);
            
            m_Questions.Clear();
            m_QuestionIndex = -1;

            _intermediateScreen.OkayClicked.RemoveAllListeners();
            _intermediateScreen.OkayClicked.AddListener(GoToNextQuestion);
            _intermediateScreen.Close();

            JSONNode questions = GetKeyNodeForLanguage(language, "template.activity.quiz.questions");
            foreach (JSONNode questionNode in questions)
            {
                QuizQuestionModel question = new QuizQuestionModel(StringUtils.CleanFromWezit(questionNode["text.content"]),
                                                                   StringUtils.CleanFromWezit(questionNode["image"]),
                                                                   questionNode["chrono.activation"],
                                                                   questionNode["template.activity.quiz.item.chrono.value"],
                                                                   questionNode["template.activity.quiz.responses"]);
                m_Questions.Add(question);
            }

            _instruction.text = StringUtils.CleanFromWezit(GetKeyNodeForLanguage(language, "template.activity.quiz.general.instruction.text.content"));

            GoToNextQuestion();
        }
        #endregion

        #region Internal
        #endregion

        #region Private
        private void GoToNextQuestion()
        {
            m_QuestionIndex++;
            if(m_QuestionIndex >= m_Questions.Count)
            {
                float winRate = m_CurrentWins / (float)m_Questions.Count;
                ActivityOver?.Invoke();
                QuizOver?.Invoke(winRate >= m_WinThreshold);
            }
            else
            {
                ResetQuestion();

                string questionImageName = m_Questions[m_QuestionIndex].Image;
                System.Threading.Tasks.Task task = LoadImage(questionImageName, _questionImage);

                _title.text = StringUtils.CleanFromWezit(m_Questions[m_QuestionIndex].Title);
                List<string> instructionAndAnswers = new List<string>();
                instructionAndAnswers.Add(_instruction.text);

                foreach (QuizAnswerModel answer in m_Questions[m_QuestionIndex].Answers)
                {
                    QuizAnswer answerInstance = Instantiate(_answerPrefab, _answersRoot);
                    answerInstance.Inflate(answer);
                    answerInstance.AnswerClicked.AddListener(OnAnswerClicked);
                    m_Answers.Add(answerInstance);
                    instructionAndAnswers.Add(answer.AnswerText);
                }
                _contrastButton.Inflate(_title.text, instructionAndAnswers.ToArray(), _contrastPanelRoot);
            }
        }

        private void ShowIntermediateScreen(QuizAnswerModel answer)
        {
            _intermediateScreen.Inflate(answer);
        }

        private void ResetQuestion()
        {
            foreach (Transform child in _answersRoot)
            {
                Destroy(child.gameObject);
            }
            _title.text = "";
            m_Answers.Clear();
        }

        private void OnAnswerClicked(QuizAnswerModel answer)
        {
            foreach (QuizAnswer answerItem in m_Answers)
            {
                answerItem.DisableButton();
            }
            m_CurrentWins += answer.IsCorrect ? 1 : 0;
            StartCoroutine(ShowResult(answer));
        }

        private IEnumerator ShowResult(QuizAnswerModel answer)
        {
            yield return new WaitForSeconds(2);
            if (answer.ShowIntermediateScreen)
            {
                ShowIntermediateScreen(answer);
            }
            else
            {
                GoToNextQuestion();
            }
        }
        #endregion
        #endregion
    }
}
