using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;

[System.Serializable]
public class _UnityEventStreamVideo : UnityEvent { };

public class StreamVideo : MonoBehaviour
{
	public static string TAG = "[StreamVideo]";

	public _UnityEventStreamVideo onVideoPlay;
	public _UnityEventStreamVideo onVideoPause;
	public _UnityEventStreamVideo onEndVideoReached;

	public RawImage rawImage;
	public Button screenButton;
	public VideoPlayer videoPlayer;
	public VideoButtonSelecter videoButtonSelecter;
	public Button skipVideoButton;
	public Text currentSeconds;
	public Text currentMinutes;
	public Text totalSeconds;
	public Text totalMinutes;
	public Slider progressBar;
	public GameObject loader;

	private VideoClickHandler progressBarClickHandler;
	private ulong frameCount;

	private Texture originalImage;

	private Coroutine videoPlayCoroutine;
	private bool videoBuffered;

	public bool isSkipVideoButtonEnabled = false;
	private bool isLooping = false;

	[SerializeField]
	private bool playPauseEnabled;
	public bool PlayPauseEnabled { get => playPauseEnabled; set => playPauseEnabled = value; }

	[SerializeField]
	private bool isProgressBarInteractive;
	public bool IsProgressBarInteractive
	{
		get => isProgressBarInteractive;
		set
		{
			isProgressBarInteractive = value;
			if (progressBar) progressBar.interactable = isProgressBarInteractive;
		}
	}

	public bool considerNetworkCommunication = false;

	void Awake()
	{
		if (videoButtonSelecter != null)
		{
			videoButtonSelecter.Init();
			videoButtonSelecter.SetToggleState(VideoPlayState.pause);
		}

		if (progressBar)
		{
			progressBarClickHandler = progressBar.GetComponent<VideoClickHandler>();
		}

		rawImage.color = new Color(1, 1, 1, 1);

		PlayPauseEnabled = false;
		IsProgressBarInteractive = false;
		videoBuffered = false;
	}

	private void Start()
	{

	}

	private void OnEnable()
	{
		videoPlayer.playOnAwake = false;
		videoPlayer.isLooping = isLooping = false;
		FadeScreen(false, false);

		if (loader) loader.SetActive(false);

		if (skipVideoButton) skipVideoButton.enabled = isSkipVideoButtonEnabled;
		if (skipVideoButton && skipVideoButton.GetComponent<Image>()) skipVideoButton.GetComponent<Image>().CrossFadeAlpha(isSkipVideoButtonEnabled ? 1 : 0, 0, false);

		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void FadeScreen(bool show, bool tween)
	{
		rawImage.CrossFadeAlpha(show ? 1 : 0, tween ? .5f : 0, false);
	}

	void Update()
	{
		if (videoPlayer.isPlaying)
		{
			SetCurrentTimeUI();
			SetProgressBarTimeUI();
		}
		else
		{
			if (videoBuffered)
			{
			}
			else
			{
			}
		}
	}

	public void SetVideoSource(string source, bool loop = false)
	{
		videoPlayer.isLooping = isLooping = loop;
		videoPlayer.url = source;
	}

	public void Launch(bool playVideo = true)
	{
		Stop();
		FadeScreen(false, false);

		if (this.gameObject.activeInHierarchy && this.gameObject.activeSelf)
		{
			videoPlayCoroutine = StartCoroutine(PrepareAndPlayVideo(playVideo));
		}
	}

	public void Stop()
	{
		if (videoPlayCoroutine != null)
		{
			StopCoroutine(videoPlayCoroutine);
			videoPlayCoroutine = null;
		}

		videoPlayer.Stop();
		// Send_VideoOnStop();
		rawImage.texture = originalImage;
		FadeScreen(false, false);

		if (videoButtonSelecter)
		{
			videoButtonSelecter.SetToggleState(VideoPlayState.pause);
		}

		SetCurrentTimeUI();
		SetProgressBarTimeUI();
	}

	public void PlayPauseVideo()
	{
		if (videoPlayCoroutine == null)
		{
			Launch();
		}
		else if (videoPlayer.isPlaying)
		{
			if (videoButtonSelecter != null && videoButtonSelecter.IsInState(VideoPlayState.pause) == false) videoButtonSelecter.SetToggleState(VideoPlayState.pause);

			videoPlayer.Pause();
			onVideoPause.Invoke();
			// Send_VideoOnPause();
		}
		else
		{
			if (videoButtonSelecter != null && videoButtonSelecter.IsInState(VideoPlayState.play) == false) videoButtonSelecter.SetToggleState(VideoPlayState.play);

			videoPlayer.Play();
			onVideoPlay.Invoke();
			// Send_VideoOnPlay();
		}
	}

	public void PauseVideo()
	{
		if (videoButtonSelecter != null && videoButtonSelecter.IsInState(VideoPlayState.pause) == false) videoButtonSelecter.SetToggleState(VideoPlayState.pause);

		videoPlayer.Pause();
		onVideoPause.Invoke();
		// Send_VideoOnPause();
	}

	public void PlayVideo()
	{
		if (videoPlayCoroutine == null) Launch();
		if (videoButtonSelecter != null && videoButtonSelecter.IsInState(VideoPlayState.play) == false) videoButtonSelecter.SetToggleState(VideoPlayState.play);

		videoPlayer.Play();
		onVideoPlay.Invoke();
		// Send_VideoOnPlay();
	}

	IEnumerator PrepareAndPlayVideo(bool playVideo = true)
	{
		videoBuffered = false;
		if (videoButtonSelecter != null) videoButtonSelecter.SetToggleState(VideoPlayState.pause);

		if (loader != null) loader.SetActive(true);


		videoPlayer.Prepare();
		WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

		while (!videoPlayer.isPrepared)
		{
			// Debug.Log(TAG + "Video not prepared");
			yield return waitForSeconds;
		}

		// Debug.Log(TAG + "Video prepared");
		videoBuffered = true;
		if (loader != null) loader.SetActive(false);
		FadeScreen(true, true);

		originalImage = rawImage.texture;
		rawImage.texture = videoPlayer.texture;

		if (playVideo)
		{
			if (videoButtonSelecter != null) videoButtonSelecter.SetToggleState(VideoPlayState.play);
			videoPlayer.Play();
			onVideoPlay.Invoke();
			// Send_VideoOnPlay();
		}

		SetTotalTimeUI();
		frameCount = videoPlayer.frameCount;
	}

	void EndReached(VideoPlayer pVideoPlayer)
	{
		if (!isLooping) ForceEndVideo();
	}

	private void ForceEndVideo()
	{
		Stop();
		onEndVideoReached.Invoke();
	}

	void SetCurrentTimeUI()
	{
		string minutes = Mathf.Floor((int)videoPlayer.time / 60).ToString("00");
		string seconds = ((int)videoPlayer.time % 60).ToString("00");

		if (currentMinutes) currentMinutes.text = minutes;
		if (currentSeconds) currentSeconds.text = seconds;
	}

	void SetTotalTimeUI()
	{
		string minutes = Mathf.Floor((int)videoPlayer.length / 60).ToString("00");
		string seconds = ((int)videoPlayer.length % 60).ToString("00");

		if (totalMinutes) totalMinutes.text = minutes;
		if (totalSeconds) totalSeconds.text = seconds;
	}

	void SetProgressBarTimeUI()
	{
		if (progressBar && (progressBarClickHandler == null || !progressBarClickHandler.IsClicked()))
		{
			float fraction = (float)videoPlayer.frame / (float)frameCount;
			progressBar.value = fraction;
		}
	}

	void UpdateVideoFrame()
	{
		if (progressBarClickHandler != null && progressBarClickHandler.IsClicked() && IsProgressBarInteractive)
		{
			float value = progressBar.value;

			double finalValue = (double)value * (double)frameCount;
			videoPlayer.frame = (long)finalValue;
		}
	}

	private void AddListeners()
	{
		RemoveListeners();

		videoPlayer.loopPointReached += EndReached;
		if (progressBarClickHandler) progressBarClickHandler.OnClicked += UpdateVideoFrame;

		if (videoButtonSelecter != null) videoButtonSelecter.onVideoButtonChange.AddListener(OnVideoToggle);
		if (screenButton != null) screenButton.onClick.AddListener(OnClickPlayPauseVideo);

		if (skipVideoButton) skipVideoButton.onClick.AddListener(ForceEndVideo);

		if (progressBar != null) progressBar.onValueChanged.AddListener(delegate { UpdateVideoFrame(); });
	}

	private void RemoveListeners()
	{
		videoPlayer.loopPointReached -= EndReached;
		if (progressBarClickHandler) progressBarClickHandler.OnClicked -= UpdateVideoFrame;

		if (videoButtonSelecter != null) videoButtonSelecter.onVideoButtonChange.RemoveAllListeners();
		if (screenButton != null) screenButton.onClick.RemoveAllListeners();

		if (skipVideoButton) skipVideoButton.onClick.RemoveAllListeners();

		if (progressBar != null) progressBar.onValueChanged.RemoveAllListeners();
	}

	private void OnVideoToggle(VideoPlayState videoState)
	{
		OnClickPlayPauseVideo(videoState);
	}

	private void OnClickPlayPauseVideo()
	{
		if (PlayPauseEnabled)
		{
			PlayPauseVideo();
		}
	}

	private void OnClickPlayPauseVideo(VideoPlayState videoState)
	{
		if (PlayPauseEnabled)
		{
			if (videoState == VideoPlayState.play)
			{
				PlayVideo();
			}
			else
			{
				PauseVideo();
			}
		}
	}

	// private void Send_VideoLoaded()
	// {
	// 	if (considerNetworkCommunication && NetworkManager.instance && NetworkManager.TCP_ROLE == TCPRole.client)
	// 		NetworkManager.instance._SendMessage(
	// 			TCPMessageHelper.ToString(TCPMessageType.client_video_loaded)
	// 		);
	// }

	// private void Send_VideoOnPlay()
	// {
	// 	if (considerNetworkCommunication && NetworkManager.instance && NetworkManager.TCP_ROLE == TCPRole.client)
	// 		NetworkManager.instance._SendMessage(
	// 			TCPMessageHelper.ToString(TCPMessageType.client_video_onPlay)
	// 		);
	// }

	// private void Send_VideoOnPause()
	// {
	// 	if (considerNetworkCommunication && NetworkManager.instance && NetworkManager.TCP_ROLE == TCPRole.client)
	// 		NetworkManager.instance._SendMessage(
	// 			TCPMessageHelper.ToString(TCPMessageType.client_video_onPause)
	// 		);
	// }

	// private void Send_VideoOnStop()
	// {
	// 	if (considerNetworkCommunication && NetworkManager.instance && NetworkManager.TCP_ROLE == TCPRole.client)
	// 		NetworkManager.instance._SendMessage(
	// 			TCPMessageHelper.ToString(TCPMessageType.client_video_onStop)
	// 		);
	// }
}
