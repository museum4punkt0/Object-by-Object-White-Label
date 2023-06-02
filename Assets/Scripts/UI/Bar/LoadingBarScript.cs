using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

[System.Serializable]
public class _UnityEventLoadingBarScript : UnityEvent { };
public class LoadingBarScript : MonoBehaviour
{
	#region Fields
	#region Serialized
	[SerializeField] private Image _imageBar = null;
	#endregion Serialized

	private float _currentTime = 0f;
	private float _delayValue = 20f;
	private Boolean _loadingFlag = false;

	public float DelayValue { get => _delayValue; set => _delayValue = value; }
	public _UnityEventLoadingBarScript onLoadingBarComplete;
	#endregion Fields

	#region Methods
	#region MonoBehavior
	public void Update()
	{
		if (_loadingFlag)
		{
			_currentTime += Time.deltaTime;
			float percent = _currentTime / DelayValue;
			if (_imageBar) _imageBar.gameObject.transform.localScale = new Vector3(Mathf.Lerp(0, 1, percent), 1, 1);

			if (percent >= 1)
			{
				_loadingFlag = false;
				onLoadingBarComplete?.Invoke();
			}
		}
	}
	#endregion MonoBehavior

	#region Public
	public void Start()
	{
		_currentTime = 0;
		_loadingFlag = true;
	}

	public void Reset()
	{
		_loadingFlag = false;
		_currentTime = 0;
		if (_imageBar) _imageBar.gameObject.transform.localScale = new Vector3(0, 1, 1);
	}
	#endregion Public
	#endregion Methods
}
