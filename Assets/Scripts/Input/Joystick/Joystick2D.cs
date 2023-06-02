using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick2D : UIBehaviour
{
	#region Fields
	[ValidateInput("ValidateJoystickController", "One of these objects doesn't contain a IJoystickController")]
	[SerializeField] private GameObject[] _joystickControllerObjects = null;
	[Tooltip("The maximum distance between the origin and the follow point, where the joystick's normalized position will be set.")]
	[SerializeField] private float _maxRadius = 100f;
	[SerializeField] private Transform _originPointImage = null;
	[SerializeField] private Transform _followPointImage = null;
	[SerializeField] private UnityEvent _onJoystickStarted = null;
	[SerializeField] private UnityEvent _onJoystickReleased = null;

	private RectTransform _joystickRectTransform = null;
	private Camera _currentCamera = null;
	private bool _wasTouched = false;
	private Vector2 _originPosition = Vector2.zero;
	private List<IJoystickController> _joystickControllers = null;
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	protected override void Awake()
	{
		base.Awake();

		if (_joystickRectTransform == null)
		{
			_joystickRectTransform = GetComponent<RectTransform>();
		}

		Canvas canvas = GetComponentInParent<Canvas>();
		if (canvas != null)
		{
			switch(canvas.renderMode)
			{
				case RenderMode.ScreenSpaceCamera:
				case RenderMode.WorldSpace:
				{
					_currentCamera = canvas.worldCamera;
					break;
				}
			}
		}

		_joystickControllers = new List<IJoystickController>();

		foreach (GameObject obj in _joystickControllerObjects)
		{
			Component[] components = obj.GetComponents<Component>();

			foreach (Component component in components)
			{
				IJoystickController controller = component as IJoystickController;
				if (controller != null)
				{
					_joystickControllers.Add(controller);
				}
			}
		}

	}


	private void Update()
	{
		if (_joystickRectTransform != null)
		{
			bool touchOrMouseDown = false;
			Vector2 currentScreenPosition = Vector2.zero;

			if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
			{
				currentScreenPosition = Input.mousePosition;
				touchOrMouseDown = true;
			}
			else if (Input.touchCount > 0)
			{
				touchOrMouseDown = true;
				currentScreenPosition = Input.GetTouch(0).position;
			}

			if (touchOrMouseDown)
			{
				
				if (RectTransformUtility.RectangleContainsScreenPoint(_joystickRectTransform, currentScreenPosition, _currentCamera))
				{
					Vector2 localPoint = Vector2.zero;
					RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickRectTransform, currentScreenPosition, _currentCamera, out localPoint);
					Vector3 worldPoint = _joystickRectTransform.TransformPoint(localPoint);

					if (_wasTouched == false)
					{
						_originPosition = localPoint;

						if (_originPointImage != null)
						{
							_originPointImage.gameObject.SetActive(true);
							_originPointImage.position = worldPoint;
						}

						OnJoystickStarted();
						_wasTouched = true;
					}

					if (_followPointImage != null)
					{
						if (_followPointImage.gameObject.activeInHierarchy == false)
						{
							_followPointImage.gameObject.SetActive(true);
						}
						_followPointImage.position = worldPoint;
					}

					UpdateJoystickControllers(localPoint);
				}
			}
			else
			{
				if (_wasTouched)
				{
					OnJoystickReleased();
				}

				_wasTouched = false;

				if (_originPointImage != null && _originPointImage.gameObject.activeInHierarchy)
				{
					_originPointImage.gameObject.SetActive(false);
				}
				if (_followPointImage != null && _followPointImage.gameObject.activeInHierarchy)
				{
					_followPointImage.gameObject.SetActive(false);
				}
			}
		}
	}

	private void UpdateJoystickControllers(Vector2 newLocalPosition)
	{
		Vector2 clampedPositionVector = Vector2.ClampMagnitude(newLocalPosition - _originPosition, _maxRadius);
		Vector2 normalizedJoystickPosition = clampedPositionVector / _maxRadius;

		foreach (IJoystickController joystickController in _joystickControllers)
		{
			joystickController.UpdateJoystick(normalizedJoystickPosition);
		}
	}

	private void OnJoystickStarted()
	{
		foreach (IJoystickController joystickController in _joystickControllers)
		{
			joystickController.OnJoystickStarted();
		}

		_onJoystickStarted.Invoke();
	}

	private void OnJoystickReleased()
	{
		foreach (IJoystickController joystickController in _joystickControllers)
		{
			joystickController.OnJoystickReleased();
		}

		_onJoystickReleased.Invoke();
	}

	private bool ValidateJoystickController(GameObject[] objects)
	{
		bool haveAllJoystickControllers = true;
		foreach (GameObject obj in objects)
		{
			if (obj == null)
			{
				haveAllJoystickControllers = false;
				continue;
			}

			bool hasJoystickController = false;
			Component[] components = obj.GetComponents<Component>();

			foreach (Component component in components)
			{
				if (component is IJoystickController)
				{
					hasJoystickController = true;
					break;
				}
			}

			if (hasJoystickController == false)
			{
				Debug.LogError("Joystick Controller not found in object " + obj.name);
			}

			haveAllJoystickControllers &= hasJoystickController;
		}

		return haveAllJoystickControllers;
	}
	#endregion Methods
}