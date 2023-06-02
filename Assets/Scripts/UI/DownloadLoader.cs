using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DownloadLoader : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private Image _loadingBar = null;
    [SerializeField] private GameObject _loadingRoot = null;
    [SerializeField] private TextMeshProUGUI _waitingText = null;
    [SerializeField] private string m_DownloadText = "Téléchargement des médias  : ";
    [SerializeField] private string m_DefaultText = "Chargement du dispositif";
    #endregion
    #region Parameters
    private int m_DownloadSize = 0;
    #endregion
    #endregion

    #region Methods

    private void Start()
    {
        _loadingRoot.SetActive(false);
        m_DownloadSize = 0;
        _loadingBar.fillAmount = 0;
        _waitingText.text = m_DefaultText;
        Wezit.DataGrabber.Instance.DownloadProgress.RemoveAllListeners();
        Wezit.DataGrabber.Instance.DownloadProgress.AddListener(UpdateProgress);
        Wezit.DataGrabber.Instance.DownloadOver.RemoveListener(OnDownloadOver);
        Wezit.DataGrabber.Instance.DownloadOver.AddListener(OnDownloadOver);
    }

    private void UpdateProgress(int progress)
    {
        if(!_loadingRoot.activeSelf) _loadingRoot.SetActive(true);
        if (m_DownloadSize == 0) m_DownloadSize = Wezit.FilesDownloader.SqliteUpdated ? Wezit.DataGrabber.Instance.GetUpdateSize() : Wezit.DataGrabber.Instance.GetDownloadSize();
        _loadingBar.fillAmount = progress / (float)m_DownloadSize;
        _waitingText.text = m_DownloadText + string.Format("{0:0.00}", progress / 1024f / 1024f) + "/" + string.Format("{0:0.00}Mo", m_DownloadSize / 1024f / 1024f);
    }

    private void OnDownloadOver()
    {
        _loadingRoot.SetActive(false);
        _waitingText.text = m_DefaultText;
    }
    #endregion
}
