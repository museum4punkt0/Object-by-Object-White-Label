/**
 * Created by Willy
 */

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx;
using System.Linq;

[System.Serializable]
public class _UnityEventVideoButtonSelecter : UnityEvent<VideoPlayState> { };

public class VideoButtonSelecter : MonoBehaviour
{
	#region Fields
	public static string TAG = "<color=white>[VideoButtonSelecter]</color>";

	public _UnityEventVideoButtonSelecter onVideoButtonChange;

	private ToggleGroup toggleGroup;
    private VideoPlayState currentPlayState;
	#endregion Fields

	#region Methods
	#region MonoBehaviour

	#endregion MonoBehaviour

	#region Public
	public void Init()
	{
		InitButtons();
		AddListeners();
	}

	public void AddListeners()
	{
		RemoveListeners();

		foreach (Transform child in transform)
		{
			if (child.GetComponent<Toggle>()) child.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleChanged);
		}
	}

	public void RemoveListeners()
	{
		foreach (Transform child in transform)
		{
			if (child.GetComponent<Toggle>()) child.GetComponent<Toggle>().onValueChanged.RemoveListener(OnToggleChanged);
		}
	}

	#endregion Public

	#region Private
	private void InitButtons()
	{
		toggleGroup = GetComponent<ToggleGroup>();
        if (toggleGroup != null)
        {
		    toggleGroup.allowSwitchOff = false;
        }

		foreach (Transform child in transform)
		{
            Toggle childToggle = child.GetComponent<Toggle>();
			if (childToggle != null)
			{
				childToggle.interactable = true;
				childToggle.group = toggleGroup;
                if (toggleGroup != null)
                {
				    toggleGroup.RegisterToggle(child.GetComponent<Toggle>());
                }
            }
		}
	}

	private void OnToggleChanged(bool toggleValue)
	{
		if (toggleValue == true)
		{
			DispatchToggleState((VideoPlayState)Enum.Parse(typeof(VideoPlayState), toggleGroup.ActiveToggles().First().gameObject.name));
		}
	}

	internal void SetToggleState(VideoPlayState videoState)
	{
		foreach (Transform child in transform)
		{
			if (child.gameObject.name == videoState.ToString())
			{
				if (child.GetComponent<Toggle>())
				{
					if (child.gameObject.activeSelf && child.gameObject.activeInHierarchy)
					{
						child.GetComponent<Toggle>().isOn = true;
						if (toggleGroup) toggleGroup.NotifyToggleOn(child.GetComponent<Toggle>());
					}
				}
				break;
			}
		}
	}

    internal bool IsInState(VideoPlayState videoState)
    {
        foreach (Transform child in transform)
		{
			if (child.gameObject.name == videoState.ToString())
			{
                Toggle toggle = child.GetComponent<Toggle>();
				if (toggle != null)
				{
                    return toggle.isOn;
				}
				break;
			}
		}

        return false;
    }

	internal void DispatchToggleState(VideoPlayState videoState)
	{
		onVideoButtonChange?.Invoke(videoState);
	}
	#endregion Private
	#endregion Methods
}
