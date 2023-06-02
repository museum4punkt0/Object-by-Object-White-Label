using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJoystickController
{
	#region Methods
	void UpdateJoystick(Vector2 normalizedPosition);
	void OnJoystickStarted();
	void OnJoystickReleased();
	#endregion Methods
}