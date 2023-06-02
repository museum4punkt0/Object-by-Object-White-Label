using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using System;

public class VideoManager : MonoBehaviour
{
	#region Fields
	#region SerializeFields
	[SerializeField] private RenderTexture _videoRenderTexture = null;
	[SerializeField] private VideoPlayer _videoPlayer = null;
	[SerializeField] private Slider _slider;
    [Space]
	[SerializeField] private Button _playButton = null;
	[SerializeField] private Image _playButtonImage = null;
	[SerializeField] private Sprite _pauseIcon = null;
	[SerializeField] private Sprite _playIcon = null;
    [Space]
	[SerializeField] private TMPro.TextMeshProUGUI _timeText = null;
	[SerializeField] private TMPro.TextMeshProUGUI _titleText = null;

	[Space]
	[SerializeField] private Button _openButton;
	[SerializeField] private Button _closeButton;
	[SerializeField] private GameObject _playerRoot;
	#endregion
	#region Private
	private const string TIMER_TEXT = "{0}:{1}";
	private ScreenOrientation m_UserOrientation;
	#endregion
	#endregion

	#region Properties
	public UnityEvent<bool> VideoPlayerOpen = new();
	public UnityEvent VideoLoopPointReached = new();
    #endregion

    #region Methods
    #region Monobehaviours
    private void OnDisable()
    {
		_videoRenderTexture.Release();
    }
    #endregion

    #region Public
    public void Inflate(string videoSource, string title)
	{
		_playerRoot.SetActive(false);
		_openButton.gameObject.SetActive(true);
		m_UserOrientation = Screen.orientation;

		_videoRenderTexture.Release();
		_videoPlayer.url = videoSource;
		_videoPlayer.isLooping = false;
		_titleText.text = title;

		_slider.onValueChanged.AddListener(OnSliderValueChanged);

		_playButton.onClick.RemoveAllListeners();
		_playButton.onClick.AddListener(OnPlayButton);

		_openButton.onClick.RemoveAllListeners();
		_closeButton.onClick.RemoveAllListeners();
		_openButton.onClick.AddListener(OnOpen);
		_closeButton.onClick.AddListener(OnClose);

	}
	#endregion

	#region Private
	private void OnPlayButton()
	{
		if (_videoPlayer != null)
		{
			if (!_videoPlayer.isPlaying)
			{
				if(_videoPlayer.time >= _videoPlayer.length)
                {
					_videoPlayer.time = 0;
					_slider.SetValueWithoutNotify(0);
                }
				_videoPlayer.Play();
				_playButtonImage.sprite = _pauseIcon;
			}
			else
			{
				_videoPlayer.Pause();
				_playButtonImage.sprite = _playIcon;
			}
		}
	}

	private void OnSliderValueChanged(float value)
	{
		_videoPlayer.time = value;
	}


	private IEnumerator VideoProgressRoutine()
	{
		_slider.value = 0;
		while (!_videoPlayer.isPlaying)
        {
			yield return null;
        }
		_slider.minValue = 0;
		_slider.maxValue = (float)_videoPlayer.length;
		TimeSpan time = TimeSpan.FromSeconds(_videoPlayer.length);
		string totalTime = time.ToString(@"mm\:ss");
		_timeText.text = string.Format(TIMER_TEXT, "00:00", totalTime);
		while (_videoPlayer.time < _videoPlayer.length)
		{
			_timeText.text = string.Format(TIMER_TEXT, TimeSpan.FromSeconds(_videoPlayer.time).ToString(@"mm\:ss"), totalTime);
            _slider.SetValueWithoutNotify((float)_videoPlayer.time);
            yield return null;
		}
		_playButtonImage.sprite = _playIcon;
		_videoPlayer.Stop();
		VideoLoopPointReached?.Invoke();
	}

	private void OnOpen()
	{
		VideoPlayerOpen?.Invoke(true);
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Hidden);
		_playerRoot.SetActive(true);
		_openButton.gameObject.SetActive(false);
		_videoPlayer.Play();
		StartCoroutine(VideoProgressRoutine());
	}

	private void OnClose()
	{
		VideoPlayerOpen?.Invoke(false);
		Screen.orientation = m_UserOrientation;
		MenuManager.Instance.SetPreviousStatus();
		_playerRoot.SetActive(false);
		_openButton.gameObject.SetActive(true);
	}
	#endregion
	#endregion
}
