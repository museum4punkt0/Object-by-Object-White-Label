using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

[System.Serializable]
public class _UnityEventRadialBarScript : UnityEvent { };
public class RadialBarScript : MonoBehaviour
{
	#region Fields
	#region Serialized
	[SerializeField] private Image _imageBar = null;
	#endregion Serialized

	private float _currentTime = 0f;
	private float _delayValue = 5f;
	private Boolean _loadingFlag = false;

	public float DelayValue { get => _delayValue; set => _delayValue = value; }
	public _UnityEventRadialBarScript onLoadingBarComplete;
	#endregion Fields

	#region Methods
	#region MonoBehavior
	private void Awake()
	{
		ResetLoading();
	}

	public void Update()
	{
		if (_loadingFlag)
		{
			_currentTime += Time.deltaTime;
			float percent = _currentTime / DelayValue;
			if (_imageBar) _imageBar.fillAmount = Mathf.Lerp(0, 1, percent);

			if (percent >= 1)
			{
				_loadingFlag = false;
				onLoadingBarComplete?.Invoke();
			}
		}
	}
	#endregion MonoBehavior

	#region Public
	public void StartLoading()
	{
		_currentTime = 0;
		_loadingFlag = true;
	}

	public void ResetLoading()
	{
		_loadingFlag = false;
		_currentTime = 0;
		if (_imageBar) _imageBar.fillAmount = 0;
	}
	#endregion Public
	#endregion Methods
}
