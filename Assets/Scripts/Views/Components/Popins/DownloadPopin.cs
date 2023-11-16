using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DownloadPopin : Popin
{
    #region Fields
    #region SerializeFields
    [SerializeField] private GameObject _progressRoot;
    [SerializeField] private Image _progressBar = null;
    [SerializeField] private Image _progressBarColor = null;
    [SerializeField] private TextMeshProUGUI _downloadText;
    #endregion
    #region Private
    private string m_downloadTextSettingKey = "template.spk.tours.download.progress.text";
    private string m_downloadText = "Laden";
    private int m_downloadSize;
    private string m_tourPid;
    #endregion
    #endregion

    #region Properties
    public UnityEvent<bool> DownloadOver = new UnityEvent<bool>();
    #endregion

    #region Methods
    #region Monobehaviours

    #endregion
    #region Public
    public void Inflate(string title, string description, string buttonText, string tourPid, int size, string iconType = "")
    {
        base.Inflate(title, description, buttonText, "", iconType);
        _popinButton.gameObject.SetActive(true);
        _closeButton.gameObject.SetActive(true);
        _description.gameObject.SetActive(true);
        _downloadText.gameObject.SetActive(false);

        _progressRoot.SetActive(false);
        _progressBarColor.color = GlobalSettingsManager.Instance.AppColor;
        _progressBar.fillAmount = 0;

        m_downloadSize = size;
        m_downloadText = Wezit.Settings.Instance.GetSettingAsCleanedText(m_downloadTextSettingKey);
        m_tourPid = tourPid;


        _popinButton.onClick.RemoveAllListeners();
        _popinButton.onClick.AddListener(OnPopinButton);
    }
    #endregion
    #region Private
    internal new void OnPopinButton()
    {
        base.OnPopinButton();
        _popinButton.gameObject.SetActive(false);
        _closeButton.gameObject.SetActive(false);
        _description.gameObject.SetActive(false);
        _downloadText.gameObject.SetActive(true);

        Wezit.DataGrabber.Instance.DownloadProgress.RemoveAllListeners();
        Wezit.DataGrabber.Instance.DownloadProgress.AddListener(UpdateProgress);
        Wezit.DataGrabber.Instance.DownloadOver.RemoveListener(OnDownloadOver);
        Wezit.DataGrabber.Instance.DownloadOver.AddListener(OnDownloadOver);

        UniRx.Async.UniTask task = Wezit.DataGrabber.Instance.GetAssetsForTour(m_tourPid);
    }

    private void UpdateProgress(int progress)
    {
        if (_progressRoot != null)
        {
            if (!_progressRoot.activeSelf) _progressRoot.SetActive(true);
            _progressBar.fillAmount = progress / (float)m_downloadSize;
            _downloadText.text = m_downloadText + string.Format(" {0:0.00}", progress / 1024f / 1024f) + "/" + string.Format("{0:0.00}Mo", m_downloadSize / 1024f / 1024f);
        }
    }

    private void OnDownloadOver()
    {
        Close();
        DownloadOver?.Invoke(true);
        PlayerManager.Instance.Player.GetTourProgression(m_tourPid).HasBeenDownloaded = true;
        PlayerManager.Instance.Player.Save();
    }
    #endregion
    #endregion
}
