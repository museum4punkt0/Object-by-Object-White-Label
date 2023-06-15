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
    [SerializeField] private Button _bonusButton;
    [SerializeField] private RawImage _bonusImage;
    [SerializeField] private Image _bonusImageBG;
    [SerializeField] private Button _selfieButton;
    [SerializeField] private RawImage _selfieImage;
    [SerializeField] private Image _selfieImageBG;
    [SerializeField] private Button _cameraButton;
    [SerializeField] private Image _cameraButtonBG;
    #endregion
    #region Private
    private Tour m_Tour;
    private TourProgressionData m_tourProgressionData;
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
        _title.color = _score.color = _bonusImageBG.color = _cameraButtonBG.color = _selfieImageBG.color = GlobalSettingsManager.Instance.AppColor;
        _title.text = StringUtils.CleanFromWezit(m_Tour.title);

        m_tourProgressionData = PlayerManager.Instance.Player.GetTourProgression(m_Tour.pid);
        _completionBars.Inflate(m_tourProgressionData.GetTourMaxProgression(), m_tourProgressionData.GetTourCurrentProgression());
        _bonusRoot.SetActive(m_tourProgressionData.HasBeenCompleted);
        if (m_tourProgressionData.IsChallengeMode)
        {
            _score.gameObject.SetActive(true);
            _score.text = m_tourProgressionData.GetTourScore() + "pts";
        }

        StartCoroutine(ImageUtils.SetImage(_bonusImage, m_tourProgressionData.TourScratchImagePath));
        StartCoroutine(ImageUtils.SetImage(_selfieImage, m_tourProgressionData.TourSelfiePath));

        AddListeners();
    }
    #endregion
    #region Private
    private void AddListeners()
    {
        RemoveListeners();

        _tourButton.onClick.AddListener(OnTourButtonClicked);
        _cameraButton.onClick.AddListener(OnCameraButtonClicked);
        _selfieButton.onClick.AddListener(OnSelfieButtonClicked);
        _bonusButton.onClick.AddListener(OnBonusButtonClicked);
    }

    private void RemoveListeners()
    {
        _tourButton.onClick.RemoveAllListeners();
        _cameraButton.onClick.RemoveAllListeners();
        _selfieButton.onClick.RemoveAllListeners();
        _bonusButton.onClick.RemoveAllListeners();
    }

    private void OnTourButtonClicked()
    {
        SelectTourAndChangeTitle();
        AppManager.Instance.GoToState(KioskState.INVENTORY_POI);
    }

    private void OnCameraButtonClicked()
    {
        SelectTourAndChangeTitle();
        foreach (Poi poi in m_Tour.childs)
        {
            if(poi.type == "secret")
            {
                StoreAccessor.State.SelectedPoi = poi;
                break;
            }
        }
        AppManager.Instance.GoToState(KioskState.SELFIE);
    }

    private void OnSelfieButtonClicked()
    {
        SelectTourAndChangeTitle();
        AppManager.Instance.GoToState(KioskState.IMAGE_VIEWER);
        StoreAccessor.State.ImageViewerImageSource = m_tourProgressionData.TourSelfiePath;
    }

    private void OnBonusButtonClicked()
    {
        SelectTourAndChangeTitle();
        AppManager.Instance.GoToState(KioskState.IMAGE_VIEWER);
        StoreAccessor.State.ImageViewerImageSource = m_tourProgressionData.TourScratchImagePath;
    }

    private void SelectTourAndChangeTitle()
    {
        StoreAccessor.State.SelectedTour = m_Tour;
        MenuManager.Instance.SetTitle(m_Tour.title);
    }

    private void ResetComponent()
    {
        _score.gameObject.SetActive(false);
    }
    #endregion
    #endregion
}
