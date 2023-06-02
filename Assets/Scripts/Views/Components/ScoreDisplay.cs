using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : Singleton<ScoreDisplay>
{
    #region Fields
    [SerializeField] private TextMeshProUGUI _scoreText;
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void UpdateScore()
    {
        if (StoreAccessor.State.SelectedTour != null)
        {
            _scoreText.text = PlayerManager.Instance.Player.GetTourProgression(StoreAccessor.State.SelectedTour.pid).TourScore.ToString();
        }
    }
    #endregion
    #region Private
    #endregion
    #endregion
}
