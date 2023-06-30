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
	[SerializeField] private SliderWithPointerEvents _slider;
    [Space]
	[SerializeField] private Button _playButton = null;
	[SerializeField] private Button _screenPlayButton = null;
	[SerializeField] private Image _playButtonImage = null;
	[SerializeField] private Sprite _pauseIcon = null;
	[SerializeField] private Sprite _playIcon = null;
    [Space]
	[SerializeField] private TMPro.TextMeshProUGUI _timeText = null;
	[SerializeField] private TMPro.TextMeshProUGUI _titleText = null;

	[Space]
	[SerializeField] private Button _openButton;
	[SerializeField] private Button _closeButton;
	[SerializeField] private Image _closeButtonBG;
	[SerializeField] private GameObject _playerRoot;
	#endregion
	#region Private
	private const string TIMER_TEXT = "{0}:{1}";
	private ScreenOrientation m_UserOrientation;
	private bool m_isSeeking;
	#endregion
	#endregion

	#region Properties
	public UnityEvent<bool> VideoPlayerToggled = new();
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

		_closeButtonBG.color = GlobalSettingsManager.Instance.AppColor;

		_videoRenderTexture.Release();
		_videoPlayer.url = videoSource;
		_videoPlayer.isLooping = false;
		_videoPlayer.Prepare();
		_titleText.text = title;
		_playButtonImage.sprite = _pauseIcon;

		_slider.onSliderPointerDown.RemoveAllListeners();
		_slider.onSliderPointerDown.AddListener(BeginScrub);
		_slider.onValueChanged.RemoveAllListeners();
		_slider.onValueChanged.AddListener(OnSliderValueChanged);
		_slider.onSliderPointerUp.RemoveAllListeners();
		_slider.onSliderPointerUp.AddListener(EndScrub);

		_playButton.onClick.RemoveAllListeners();
		_playButton.onClick.AddListener(OnPlayButton);
		_screenPlayButton.onClick.RemoveAllListeners();
		_screenPlayButton.onClick.AddListener(OnPlayButton);

		_openButton.onClick.RemoveAllListeners();
		_closeButton.onClick.RemoveAllListeners();
		_openButton.onClick.AddListener(OnOpen);
		_closeButton.onClick.AddListener(OnClose);

	}

	public void Toggle(bool isOn)
    {
		gameObject.SetActive(isOn);
		_openButton.gameObject.SetActive(isOn);
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

	private void BeginScrub()
	{
		//It is recommended to pause the player when seeking as otherwise,
		//you will continuously fight the VideoPlayer from playing and buffering frames.
		_videoPlayer.Pause();

		//To know when the player has finished seeking      
		_videoPlayer.seekCompleted += PlayerSeekCompleted;
		m_isSeeking = false;
	}

	private void OnSliderValueChanged(float value)
	{
		//If you are currently seeking there is no point to seek again.
		if (m_isSeeking)
			return;

		//Don't seek, if the time between the slider value and the current player time is too small.
		//We will seek to the closest frame so if the delta is 0.00001f you will most likely seek the same frame.
		//Change the value to fit your use case.
		if (Mathf.Abs((float)_videoPlayer.time - value) < 0.01f)
			return;

		_videoPlayer.time = value;
		m_isSeeking = true;
	}

	public void EndScrub()
	{
		//You don't want random event when you are not using this script
		_videoPlayer.seekCompleted -= PlayerSeekCompleted;
		_videoPlayer.Play();
	}

	private IEnumerator VideoProgressRoutine()
	{
		_slider.value = 0;
		while (!_videoPlayer.isPrepared)
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

	private void PlayerSeekCompleted(VideoPlayer source)
    {
		m_isSeeking = false;
    }

	private void OnOpen()
	{
		VideoPlayerToggled?.Invoke(true);
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		MenuManager.Instance.SetMenuStatus(MenuManager.MenuStatus.Hidden);
		_playerRoot.SetActive(true);
		_openButton.gameObject.SetActive(false);
		_videoPlayer.Play();
		_playButtonImage.sprite = _pauseIcon;
		StartCoroutine(VideoProgressRoutine());
	}

	private void OnClose()
	{
		VideoPlayerToggled?.Invoke(false);
		Screen.orientation = m_UserOrientation;
		MenuManager.Instance.SetPreviousStatus();
		_playerRoot.SetActive(false);
		_openButton.gameObject.SetActive(true);
	}
	#endregion
	#endregion
}
