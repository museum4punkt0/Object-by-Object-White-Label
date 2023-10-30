using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDescription : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Toggle _audioToggle;
    [SerializeField] private Image _toggleBG;
    [SerializeField] private Image _toggleIcon;
    [SerializeField] private Sprite _playSprite;
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Image _progressBG;
    [SerializeField] private Image _progress;
    #endregion
    #region Private
    private AudioClip m_audioClip;
    #endregion
    #endregion

    #region Properties
    #endregion

    #region Methods
    #region Monobehaviours
    #endregion
    #region Public
    public async void Inflate(string audioSourceUri)
    {
        m_audioClip = await AudioUtils.GetAudioClipFromSource(audioSourceUri);
        _audioSource.clip = m_audioClip;

        _audioToggle.onValueChanged.AddListener(OnAudioToggled);
        _audioToggle.SetIsOnWithoutNotify(false);
        _toggleIcon.sprite = _playSprite;

        _progress.fillAmount = 0;

        _progressBG.color = GlobalSettingsManager.Instance.AppColor;
        _toggleBG.color = GlobalSettingsManager.Instance.AppColor;
    }
    #endregion
    #region Private
    private void OnAudioToggled(bool isPlay)
    {
        if(isPlay)
        {
            _toggleIcon.sprite = _pauseSprite;

            if(_audioSource.time == 0)
            {
                _audioSource.Play();
                StartCoroutine(ProgressCoroutine(_audioSource.clip.length));
            }
            else
            {
                _audioSource.UnPause();
            }
        }
        else
        {
            _toggleIcon.sprite = _playSprite;
            _audioSource.Pause();
        }
    }

    private IEnumerator ProgressCoroutine(float audioLength)
    {
        while (_audioSource.time < audioLength)
        {
            _progress.fillAmount = _audioSource.time / audioLength;
            yield return null;
        }

        _audioSource.Stop();
        _audioSource.time = 0;
        _toggleIcon.sprite = _playSprite;
    }
    #endregion
    #endregion
}
