using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : Singleton<ScoreDisplay>
{
    #region Fields
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Image _colorBG;
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Init()
    {
        if(GlobalSettingsManager.Instance != null && GlobalSettingsManager.Instance.AppColor != null)
        { 
            _colorBG.color = GlobalSettingsManager.Instance.AppColor;
        }
    }

    public void SetScore(int score)
    {
        if(_scoreText != null)
        {
            _scoreText.text = score.ToString();
        }
    }

    public void UpdateScore()
    {
        if(_scoreText != null)
        {
            if (StoreAccessor.State.SelectedTour != null)
            {
                _scoreText.text = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).TourScore.ToString();
            }
        }
    }
    #endregion
    #region Private
    #endregion
    #endregion
}
