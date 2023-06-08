using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

namespace Wezit
{
    public class QuizAnswerModel
    {
        public string AnswerText;
        public string ImageSource;
        public bool IsCorrect;

        public bool ShowIntermediateScreen;
        public string IntermediateScreenTitle;
        public string IntermediateScreenDescription;
        public string IntermediateScreenImageSource;

        public QuizAnswerModel(string answerText, string imageSource, bool isCorrect, bool showIntermediateScreen, string intermediateScreenTitle, string intermediateScreenDescription, string intermediateScreenImageSource)
        {
            ImageSource = imageSource;
            AnswerText = answerText;
            IsCorrect = isCorrect;

            ShowIntermediateScreen = showIntermediateScreen;
            IntermediateScreenTitle = intermediateScreenTitle;
            IntermediateScreenDescription = intermediateScreenDescription;
            IntermediateScreenImageSource = intermediateScreenImageSource;
        }
    }
}
