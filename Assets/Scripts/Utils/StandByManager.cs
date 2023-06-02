using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventStandByManager : UnityEvent { };

public class StandByManager : MonoBehaviour
{
	/*************************************************************/
	/*********************** PROPERTIES **************************/
	/*************************************************************/

	private static string TAG = "[StandByManager]";
	public static StandByManager instance;

	private Vector3 lastMousePosition;
	private float standByDelay;         // seconds
	private float repeatRate;           // seconds
	private float elapsedTime;          // seconds

	private Stack<bool> blockerStack;
	private bool blockerFlag;

	public UnityEventStandByManager onStandByComplete;
	public UnityEventStandByManager onStandByReset;

	/*************************************************************/
	/******************** INTERNAL METHODS ***********************/
	/*************************************************************/

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			InitInstance();
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	/*************************************************************/
	/********************** PUBLIC METHODS ***********************/
	/*************************************************************/

	public void Init(float delay, float repeatRate = 1f)
	{
		this.standByDelay = delay;
		this.repeatRate = repeatRate;

		lastMousePosition = new Vector3(0, 0, 0);
		elapsedTime = 0;
	}

	public void Begin()
	{
		InvokeRepeating("OnRepeatRate", 0f, repeatRate);
	}

	public void End()
	{
		CancelInvoke("OnRepeatRate");
	}

	public void AddBlockerToStack()
	{
		if (blockerStack != null)
		{
			if ((blockerStack.Count == 0) && (blockerFlag == false))
			{
				blockerFlag = true;
			}

			blockerStack.Push(true);
		}
	}

	public void RemoveBlockerToStack()
	{
		if (blockerStack != null && blockerStack.Count > 0)
		{
			blockerStack.Pop();

			if ((blockerStack.Count == 0) && (blockerFlag == true))
			{
				blockerFlag = false;
				elapsedTime = 0;
			}
		}
	}

	/*************************************************************/
	/********************* PRIVATE METHODS ***********************/
	/*************************************************************/

	private void InitInstance()
	{
		lastMousePosition = new Vector3();

		blockerStack = new Stack<bool>();
		blockerFlag = false;

		onStandByComplete = new UnityEventStandByManager();
		onStandByReset = new UnityEventStandByManager();
	}

	private void OnRepeatRate()
	{
		CheckMousePosition();

		if ((elapsedTime >= standByDelay) && !blockerFlag)
		{
			onStandByComplete.Invoke();
		}
	}

	private void CheckMousePosition()
	{
		if ((lastMousePosition != Input.mousePosition) && !blockerFlag)
		{
			lastMousePosition = Input.mousePosition;
			ResetStandBy();
		}
		else
		{
			elapsedTime += repeatRate;
		}
	}

	private void ResetStandBy()
	{
		elapsedTime = 0;
		onStandByReset.Invoke();
	}
}
