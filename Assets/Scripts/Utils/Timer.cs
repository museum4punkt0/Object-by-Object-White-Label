/**
 * Created by Willy
 */

using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public delegate void FinishAction();
	public static event FinishAction OnFinish;

	public bool Reverse = false;
	public float TotalTime = 60.0f;

	private float Time;
	private IEnumerator TimerLoopCoroutine;

	void Start()
	{
		InitTime();
	}

	public void InitTime()
	{
		Time = Reverse ? 0.0f : TotalTime;
	}

	public void StartTimer()
	{
		InitTime();
		ResumeTimer();
	}

	public void PauseTimer()
	{
		StopTimer();
	}

	public void ResumeTimer()
	{
		StopTimer();

		TimerLoopCoroutine = TimerLoop();
		StartCoroutine(TimerLoopCoroutine);
	}

	public void StopTimer()
	{
		if (TimerLoopCoroutine != null)
		{
			StopCoroutine(TimerLoopCoroutine);
			TimerLoopCoroutine = null;
		}
	}

	public void ResetTimer()
	{
		StopTimer();
		InitTime();
	}

	void Update()
	{
		TimeSpan ts = TimeSpan.FromSeconds(Time);
	}

	IEnumerator TimerLoop()
	{
		if (Reverse)
		{
			while (Time < TotalTime)
			{
				yield return new WaitForSecondsRealtime(1.0f);
				Time++;
			}
		}
		else
		{
			while (Time > 0)
			{
				yield return new WaitForSecondsRealtime(1.0f);
				Time--;
			}
		}

		if (OnFinish != null)
		{
			OnFinish();
		}
	}
}
