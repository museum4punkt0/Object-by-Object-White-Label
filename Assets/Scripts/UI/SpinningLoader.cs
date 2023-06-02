using UnityEngine;

public class SpinningLoader : MonoBehaviour
{
	/*************************************************************/
	/*********************** PROPERTIES **************************/
	/*************************************************************/

	public RectTransform mainIcon;
	public float timeStep = 0.01f;
	public float oneStepAngle = 10f;

	private float startTime;

	/*************************************************************/
	/******************** INTERNAL METHODS ***********************/
	/*************************************************************/

	void Start()
	{
		startTime = Time.time;
	}

	void Update()
	{
		if (Time.time - startTime >= timeStep)
		{
			Vector3 iconAngle = mainIcon.localEulerAngles;
			iconAngle.z += oneStepAngle;

			mainIcon.localEulerAngles = iconAngle;

			startTime = Time.time;
		}
	}
}
