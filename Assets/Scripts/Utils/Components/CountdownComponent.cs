/**
 * Created by Willy
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;

[System.Serializable]
public class UnityEventCountdownComponent : UnityEvent { };

public class CountdownComponent : MonoBehaviour
{
	/*************************************************************/
	/*********************** PROPERTIES **************************/
	/*************************************************************/

	public int countdownDelay;
	public TextMeshProUGUI countdownText = null;
	public UnityEventCountdownComponent onCountdownComplete;

	private Coroutine countdownCoroutine = null;


	/*************************************************************/
	/******************** INTERNAL METHODS ***********************/
	/*************************************************************/

	// private void Awake()
	// {
	// }

	/*************************************************************/
	/********************** PUBLIC METHODS ***********************/
	/*************************************************************/

	public void StartTimer()
	{
		StopTimer();
		countdownCoroutine = StartCoroutine(CountdownRoutine());
	}

	public void StopTimer()
	{
		if (countdownCoroutine != null)
		{
			StopCoroutine(countdownCoroutine);
			countdownCoroutine = null;
		}
	}

	/*************************************************************/
	/********************* PRIVATE METHODS ***********************/
	/*************************************************************/

	private IEnumerator CountdownRoutine()
	{

		while (countdownDelay > 0)
		{
			countdownText.text = countdownDelay.ToString();
			yield return new WaitForSeconds(1f);
			countdownDelay--;
		}

		onCountdownComplete?.Invoke();
		StopTimer();
	}
}
