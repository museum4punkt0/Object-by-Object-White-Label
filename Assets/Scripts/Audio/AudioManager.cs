using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    #region Fields
    #region SerializeFields
    [SerializeField] private AudioSource _audioSource = null;
	[SerializeField] private Slider _slider;
	[SerializeField] private Button _playButton = null;
	[SerializeField] private Image _playButtonImage = null;
	[SerializeField] private Image _playButtonBG = null;
	[SerializeField] private Sprite _pauseIcon = null;
	[SerializeField] private Sprite _playIcon = null;
	[SerializeField] private TMPro.TextMeshProUGUI _timeText = null;
	#endregion
	#region Private
	private const string TIMER_TEXT = "{0} / {1}";
	#endregion
	#endregion

	#region Properties
	public UnityEvent AudioLoopPointReached = new();
	#endregion

	#region Methods
	#region Monobehaviours
	#endregion

	#region Public
	public void Inflate(AudioClip audioClip)
    {
		_audioSource.clip = audioClip;
		_audioSource.loop = false;

		_slider.minValue = 0;
		_slider.maxValue = audioClip.length;
		_slider.onValueChanged.AddListener(OnSliderValueChanged);
		_slider.fillRect.GetComponent<Image>().color = GlobalSettingsManager.Instance.AppColor;
		_slider.handleRect.GetComponent<Image>().color = GlobalSettingsManager.Instance.AppColor;
		_slider.handleRect.GetComponent<Image>().color = GlobalSettingsManager.Instance.AppColor;

		_playButton.onClick.RemoveAllListeners();
		_playButton.onClick.AddListener(OnPlayButton);
		_playButtonBG.color = _timeText.color = GlobalSettingsManager.Instance.AppColor;

		_audioSource.Play();
		StartCoroutine(AudioProgressRoutine());
    }
    #endregion

    #region Private
    private void OnPlayButton()
	{
		if (_audioSource != null)
		{
			if (!_audioSource.isPlaying)
			{
				if(_audioSource.time == 0)
				{
					_slider.SetValueWithoutNotify(0);
					_audioSource.time = 0;
					StartCoroutine(AudioProgressRoutine());
                }
				_audioSource.Play();
				_playButtonImage.sprite = _pauseIcon;
			}
			else
			{
				_audioSource.Pause();
				_playButtonImage.sprite = _playIcon;
			}
		}
	}

	private void OnSliderValueChanged(float value)
    {
		_audioSource.time = value;
    }

	private IEnumerator AudioProgressRoutine()
    {
		TimeSpan time = TimeSpan.FromSeconds(_audioSource.clip.length);
		string totalTime = time.ToString(@"mm\:ss");
		_timeText.text = string.Format(TIMER_TEXT, "00:00", totalTime);
		_slider.value = 0;
		while (_audioSource.time < _audioSource.clip.length)
		{
			_slider.SetValueWithoutNotify(_audioSource.time);
			_timeText.text = string.Format(TIMER_TEXT, TimeSpan.FromSeconds(_audioSource.time).ToString(@"mm\:ss"), totalTime);
			yield return null;
		}
		AudioLoopPointReached?.Invoke();
		_playButtonImage.sprite = _playIcon;
	}
	#endregion
	#endregion
}
