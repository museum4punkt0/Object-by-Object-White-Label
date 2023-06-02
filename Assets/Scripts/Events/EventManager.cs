/**
 * Created by Willy
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
	//Re-usable structure/ Can be a class to. Add all parameters you need inside it
	public struct EventParam
	{
		public object any;
		public GameObject gameObject;
		public RectTransform areaRect;
		public string stringParam;
		public int intParam;
		public float floatParam;
		public bool boolParam;
	}

	#region Fields
	public static string TAG = "<color=blue>[EventManager]</color>";

	#region Private m_Variables
	private Dictionary<string, UnityEvent<EventParam?>> m_EventDictionary;
	#endregion Private m_Variables
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	#region Public
	public void Init()
	{
		Debug.Log(TAG + " Init ");
		if (m_EventDictionary == null) m_EventDictionary = new Dictionary<string, UnityEvent<EventParam?>>();
	}
	#endregion Public

	#region Public Static
	public void StartListening(string eventName, UnityAction<EventParam?> listener)
	{
		UnityEvent<EventParam?> thisEvent = null;
		if (m_EventDictionary != null && m_EventDictionary.TryGetValue(eventName, out thisEvent))
		{
			// Add more event to the existing one
			thisEvent.AddListener(listener);
			// Update the Dictionary
			m_EventDictionary[eventName] = thisEvent;
		}
		else
		{
			thisEvent = new UnityEvent<EventParam?>();
			thisEvent.AddListener(listener);
			// Add event to the Dictionary for the first time
			m_EventDictionary.Add(eventName, thisEvent);
		}
	}

	public void StopListening(string eventName, UnityAction<EventParam?> listener, bool disposeEvent = false)
	{
		UnityEvent<EventParam?> thisEvent = null;
		if (m_EventDictionary != null && m_EventDictionary.TryGetValue(eventName, out thisEvent))
		{
			// Remove event from the existing one
			thisEvent.RemoveListener(listener);

			if (disposeEvent) DisposeEvent(eventName);
		}
	}

	public void TriggerEvent(string eventName, EventParam? eventParam = null)
	{
		UnityEvent<EventParam?> thisEvent = null;
		if (m_EventDictionary != null && m_EventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(eventParam);
		}
	}

	public void DisposeEvent(string eventName)
	{
		if (m_EventDictionary != null && m_EventDictionary.ContainsKey(eventName))
		{
			m_EventDictionary.Remove(eventName);
		}
	}
	#endregion Public Static
	#endregion Methods
}