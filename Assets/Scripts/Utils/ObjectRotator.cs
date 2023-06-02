using System.Collections;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
	#region Const
	private const float STOP_ROTATION_MIN_ANGLE = 0.5f;
	#endregion Const

	#region Fields
	[SerializeField] private Transform _objectToRotate;
	[SerializeField] private float _smoothTime = 0.2f;
	[NaughtyAttributes.MinMaxSlider(-360, 360)]
	[SerializeField] private Vector2 _clampedRotationX = new Vector2(-60, 60);
	[NaughtyAttributes.MinMaxSlider(-360, 360)]
	[SerializeField] private Vector2 _clampedRotationY = new Vector2(-360, 360);

	private Vector2 _lastRotationFromOrigin = Vector2.zero;
	private Vector2 _lastRotationAngles = Vector2.zero;
	private Vector2 _currentRotationVelocity = Vector2.zero;
	private Coroutine _smoothStopCoroutine = null;
	private Quaternion _originalRotation = Quaternion.identity;

	private bool _isRotatingTo = false;
	private Vector2 _rotatingToSpeed = Vector2.zero;
	private Quaternion _toRotation = Quaternion.identity;

	private bool _isInitialized = false;
	#endregion Fields

	#region Properties
	public Transform ObjectToRotate
	{
		get { return _objectToRotate; }
		set
		{
			_objectToRotate = value;
			Initialize();
		}
	}

	public Vector2 ClampedRotationX
	{
		get { return _clampedRotationX; }
		set { _clampedRotationX = new Vector2(Mathf.Clamp(value.x, -360, 360), Mathf.Clamp(value.y, -360, 360)); }
	}

	public Vector2 ClampedRotationY
	{
		get { return _clampedRotationY; }
		set { _clampedRotationY = new Vector2(Mathf.Clamp(value.x, -360, 360), Mathf.Clamp(value.y, -360, 360)); }
	}
	#endregion Properties

	#region Methods
	#region Public
	public void RotateToOrigin(float rotationTime)
	{
		if (_isInitialized == false)
		{
			Initialize();
		}

		if (rotationTime > 0)
		{
			_isRotatingTo = true;
			Vector2 rotationToOrigin = _lastRotationFromOrigin * -1;
			rotationToOrigin.x = ClampAngle360(rotationToOrigin.x);
			rotationToOrigin.y = ClampAngle360(rotationToOrigin.y);

			_toRotation = _originalRotation;
			_rotatingToSpeed = rotationToOrigin / rotationTime;
		}
		else
		{
			_objectToRotate.localRotation = _originalRotation;
			_lastRotationAngles = Vector2.zero;
			_lastRotationFromOrigin = Vector2.zero;
		}
	}

	/// <summary>
	/// Will rotate an object given an angleSpeed. 
	/// </summary>
	public void RotateObject(Vector2 currentAngleSpeed)
	{
		Vector2 rotationAngles = Time.deltaTime * currentAngleSpeed;
		rotationAngles = Vector2.SmoothDamp(_lastRotationAngles, rotationAngles, ref _currentRotationVelocity, _smoothTime);
		_lastRotationAngles = rotationAngles;

		Vector2 newRotation = _lastRotationFromOrigin + rotationAngles;
		newRotation.x = ClampAngle(newRotation.x, _clampedRotationX);
		newRotation.y = ClampAngle(newRotation.y, _clampedRotationY);

		_lastRotationFromOrigin = newRotation;

		Quaternion xRotation = Quaternion.AngleAxis(newRotation.x, Vector3.right);
		Quaternion yRotation = Quaternion.AngleAxis(newRotation.y, Vector3.up);

		_objectToRotate.localRotation = _originalRotation * yRotation * xRotation;
	}

	public void StopRotationSmooth()
	{
		_smoothStopCoroutine = StartCoroutine(SmoothStopRotation());
	}

	public void StopRotationCut()
	{
		if (_isInitialized == false)
		{
			Initialize();
		}

		_isRotatingTo = false;

		if (_smoothStopCoroutine != null)
		{
			StopCoroutine(_smoothStopCoroutine);
		}
	}
	#endregion Public

	#region MonoBehaviour
	private void Awake()
	{
		Initialize();
	}

	private void Update()
	{
		if (_isRotatingTo)
		{
			RotateObject(_rotatingToSpeed);

			if (Quaternion.Angle(_toRotation, _objectToRotate.localRotation) < STOP_ROTATION_MIN_ANGLE)
			{
				_objectToRotate.localRotation = _toRotation;
				_isRotatingTo = false;
			}

		}
	}
	#endregion MonoBehaviour

	#region Private
	private void Initialize()
	{
		if (_objectToRotate != null)
		{
			_originalRotation = _objectToRotate.localRotation;
			_isInitialized = true;
		}
	}

	private IEnumerator SmoothStopRotation()
	{
		while (_currentRotationVelocity.magnitude > Mathf.Epsilon)
		{
			RotateObject(Vector2.zero);

			yield return null;
		}
	}

	private float ClampAngle360(float angle)
	{
		return ClampAngle(angle, new Vector2(-360, 360));
	}

	private float ClampAngle(float angle, Vector2 angleMinMax)
	{
		angle = angle % 360f;
		if ((angle >= -360F) && (angle <= 360F))
		{
			if (angle < -180F)
			{
				angle += 360F;
			}
			if (angle > 180F)
			{
				angle -= 360F;
			}
		}
		return Mathf.Clamp(angle, angleMinMax.x, angleMinMax.y);
	}
	#endregion Private
	#endregion Methods
}
