using UnityEngine;

/// <summary>
/// Makes a rotation of a camera depending of an incoming position of a joystick. 
/// Right now limited to XY rotation.
/// Uses Euler Angles.
/// </summary>
public class CameraRotationJoystickController : CameraRotator, IJoystickController
{
	#region Fields
	[Tooltip("Max Speed for each rotation axis. In degrees/s")]
	[SerializeField] private Vector2 _maxSpeed = new Vector2(100, 100);
	[SerializeField] private Matrix4x4 _rotationTranspositionMatrix = new Matrix4x4(new Vector4(0, -1, 0, 0), new Vector4(1, 0, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1));
	#endregion

	#region Methods
	public void OnJoystickReleased()
	{
		StopRotationSmooth();
	}

	public void OnJoystickStarted()
	{
		StopRotationCut();
	}

	public void UpdateJoystick(Vector2 normalizedPosition)
	{
		Vector2 angleSpeed = Vector2.Scale(normalizedPosition, _maxSpeed);
		angleSpeed = _rotationTranspositionMatrix.MultiplyVector(angleSpeed);
		RotateObject(angleSpeed);
	}
	#endregion Methods
}