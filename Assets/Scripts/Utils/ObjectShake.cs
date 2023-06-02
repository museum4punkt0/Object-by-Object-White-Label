/**
 * Created by Willy
 */

using UnityEngine;

public class ObjectShake : MonoBehaviour
{
	#region Fields
	private Vector3 originLocalPosition;
	private Quaternion originLocalRotation;
	public float shake_decay = 0.002f;
	public float shake_intensity = .25f;

	private float temp_shake_intensity = 0;
	private bool endShakeFlag = true;
	#endregion Fields

	#region Methods
	#region MonoBehavior
	private void Awake()
	{
		Init();
	}

	private void Update()
	{
		if (temp_shake_intensity > 0)
		{
			transform.localPosition = originLocalPosition + Random.insideUnitSphere * temp_shake_intensity;
			transform.localRotation = new Quaternion(
				originLocalRotation.x + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
				originLocalRotation.y + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
				originLocalRotation.z + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f,
				originLocalRotation.w + Random.Range(-temp_shake_intensity, temp_shake_intensity) * .2f);
			temp_shake_intensity -= shake_decay;
		}

		if ((temp_shake_intensity <= 0) && (endShakeFlag == false))
		{
			ResetShake();
		}
	}
	#endregion MonoBehavior

	#region Public
	public void Init()
	{
		endShakeFlag = true;
		originLocalPosition = transform.localPosition;
		originLocalRotation = transform.localRotation;
		temp_shake_intensity = 0;
	}

	public void ResetShake()
	{
		endShakeFlag = true;
		transform.localPosition = originLocalPosition;
		transform.localRotation = originLocalRotation;
		temp_shake_intensity = 0;
	}

	public void Shake()
	{
		if (endShakeFlag == false) ResetShake();

		temp_shake_intensity = shake_intensity;
		endShakeFlag = false;
	}
	#endregion Public
	#endregion Methods
}