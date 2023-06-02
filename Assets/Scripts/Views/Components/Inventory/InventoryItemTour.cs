using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Wezit;
using Utils;

public class InventoryItemTour : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Button _tourButton;
    [SerializeField] private RawImage _image;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private CompletionBars _completionBars;
    [Space]
    [SerializeField] private GameObject _bonusRoot;
    [SerializeField] private RawImage _bonusImage;
    [SerializeField] private Image _bonusImageBG;
    [SerializeField] private RawImage _pictureImage;
    [SerializeField] private Image _pictureImageBG;
    [SerializeField] private Image _cameraButtonBG;
    #endregion
    #region Private
    private Tour m_Tour;
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public void Inflate(Wezit.Tour tour)
    {
        ResetComponent();

        m_Tour = tour;

        ImageUtils.LoadImage(_image, this, m_Tour);
        _title.color = _score.color = _bonusImageBG.color = _cameraButtonBG.color = _pictureImageBG.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = StringUtils.CleanFromWezit(m_Tour.title);

        TourProgressionData tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(m_Tour.pid);
        _completionBars.Inflate(tourProgressionData.GetTourMaxProgression(), tourProgressionData.GetTourCurrentProgression());
        _bonusRoot.SetActive(tourProgressionData.HasBeenCompleted);
        if (tourProgressionData.IsChallengeMode)
        {
            _score.gameObject.SetActive(true);
            _score.text = tourProgressionData.GetTourScore() + "pts";
        }

        _tourButton.onClick.AddListener(OnTourButtonClicked);
    }
    #endregion
    #region Private
    private void OnTourButtonClicked()
    {
        StoreAccessor.State.SelectedTour = m_Tour;
        AppManager.Instance.GoToState(KioskState.INVENTORY_POI);
    }

    private void ResetComponent()
    {
        _score.gameObject.SetActive(false);
    }
    #endregion
    #endregion
}
