using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using Utils;

public class PinchableTurnableScrollRect : ScrollRect
{
    #region Fields
    const float PINCHTURNRATIO = Mathf.PI / 2;
    const float MINTURNANGLE = 0;
    const float PINCHRATIO = 0.5f;

    [SerializeField] private float _minZoom = 1f;

    [SerializeField] private float _maxZoom = 5f;
    [SerializeField] private Transform m_Rotator = null;
	public UnityEvent PanRotZoom = new UnityEvent();

	private Rect _thisRect;
	private Rect _contentRect;

	private float _currentZoom = 1f;
	private float _currentLerpedZoom = 1f;
	private float _zoomLerpSpeed = 10f;
	private float _mouseWheelSensitivity = .1f;

	private float _startPinchDist;
	private float _startPinchZoom;
    private float m_TurnAngleDelta; // The delta of the angle between two touch points 
    private float m_TurnAngle; // The angle between two touch points
    private Quaternion m_DesiredRotation;
    private Transform m_ThisTransform;
	private bool _isPinching = false;
	private bool _isRotating = false;
	private bool _blockPan = false;

	private Vector2 _forcePivotPosition;
	private bool _forceXPivotOnCenter;
	private bool _forceYPivotOnCenter;

	private bool _recomputePivot = true;
	#endregion Fields

    #region Properties
    public float MinZoom { get { return _minZoom; } set { _minZoom = value; } }
    public float MaxZoom { get { return _maxZoom; } set { _maxZoom = value; } }
    public float CurrentZoom { get { return _currentZoom; } }
	public float CurrentLerpedZoom { get { return _currentLerpedZoom; } }

	public bool RecomputePivot { get { return _recomputePivot; } set { _recomputePivot = value; } }
	#endregion Properties
    
    #region Methods
	#region Public
	public override void OnScroll(PointerEventData data)
	{
		if (Mathf.Abs(data.scrollDelta.y) > float.Epsilon)
		{
			_currentZoom *= 1 + data.scrollDelta.y * _mouseWheelSensitivity;
			_currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

			_forcePivotPosition = InitPivotPosition(content, (Vector2)Input.mousePosition);
		}
	}

	public void SetCurrentZoom(float a_zoom)
    {
		_currentZoom = Mathf.Clamp(a_zoom, _minZoom, _maxZoom);
	}
	#endregion Public

	#region MonoBehavior
	protected override void Awake()
	{
		Input.multiTouchEnabled = true;
		_thisRect = GetComponent<RectTransform>().rect;
	}

	private void Update()
	{
        if (Application.isPlaying)
        {
            if (Input.touchCount == 2)
            {
				// _isRotating = CalculatePinchAngle();
                if (!_isPinching && IsPinchEnabled() && !_isRotating)
                {
                    _isPinching = true;
                    OnPinchStart();
                }
                OnPinch();
            }
            else
            {
                if (_isPinching) StartCoroutine(KeepsInLimits());

                _isPinching = false;
                if (Input.touchCount == 0)
                {
                    _blockPan = false;
                }
            }

			if(RecomputePivot)
				// SetPivotOnCenter();

            if (Mathf.Abs(content.localScale.x - _currentZoom) > 0.001f)
            {
				_currentLerpedZoom = Mathf.Lerp(content.localScale.x, 1f * _currentZoom, _zoomLerpSpeed * Time.deltaTime);
				SetContentScale(Vector3.one * _currentLerpedZoom);
			}
                
        }
	}

	private void SetPivotOnCenter()
	{
		_forceXPivotOnCenter = ((Math.Round(content.rect.width * content.localScale.x, 4)) <= _thisRect.width || _currentZoom <= _minZoom);
		_forceYPivotOnCenter = ((Math.Round(content.rect.height * content.localScale.y, 4)) <= _thisRect.height || _currentZoom <= _minZoom);
		RectTransformUtils.SetPivot(content, new Vector2(_forceXPivotOnCenter ? 0.5f : _forcePivotPosition.x, _forceYPivotOnCenter ? 0.5f : _forcePivotPosition.y));
	}
	#endregion MonoBehavior

	#region Private
	private IEnumerator KeepsInLimits()
	{
		this.movementType = MovementType.Elastic;
		yield return new WaitForSeconds(0.25f);
		this.movementType = MovementType.Clamped;
	}

	private bool IsPinchEnabled()
	{
		Vector2 localTouchPos0;
		Vector2 localTouchPos1;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.GetTouch(0).position, null, out localTouchPos0);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.GetTouch(1).position, null, out localTouchPos1);

		return (GetComponent<RectTransform>().rect.Contains(localTouchPos0) && GetComponent<RectTransform>().rect.Contains(localTouchPos1));
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		PanRotZoom?.Invoke();
		base.OnBeginDrag(eventData);
	}

	private void OnPinchStart()
	{
		_startPinchDist = RectTransformUtils.CalculateDistanceBetweenPoints(content, Input.GetTouch(0).position, Input.GetTouch(1).position) * content.localScale.x;
		_startPinchZoom = _currentZoom;

		_blockPan = true;
	}

	private void OnPinch()
	{
		if (!_isPinching) return;

		float currentPinchDist = RectTransformUtils.CalculateDistanceBetweenPoints(content, Input.GetTouch(0).position, Input.GetTouch(1).position) * content.localScale.x;
		_currentZoom = (currentPinchDist / _startPinchDist) * _startPinchZoom;
		_currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);

		_forcePivotPosition = InitPivotPosition(content, (Input.GetTouch(0).position + Input.GetTouch(1).position) * 0.5f);
		
		PanRotZoom?.Invoke();
	}

	private Vector2 InitPivotPosition(RectTransform _content, Vector2 startZoomScreenPosition)
	{
		Vector2 zoomCenterPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, startZoomScreenPosition, null, out zoomCenterPoint);
		Vector2 pivotPosition = new Vector3(_content.pivot.x * _content.rect.size.x, _content.pivot.y * _content.rect.size.y);
		Vector2 posFromBottomLeft = pivotPosition + zoomCenterPoint;

		return new Vector2(posFromBottomLeft.x / _content.rect.width, posFromBottomLeft.y / _content.rect.height);
	}

	protected override void SetContentAnchoredPosition(Vector2 position)
	{
		if (_isPinching || _blockPan) return;
		base.SetContentAnchoredPosition(position);
	}

	private void SetContentScale(Vector3 scaleValue)
	{
		content.localScale = scaleValue;
	}
	
	/// <summary> ///   Calculates Pinch and Turn /// </summary>
    private bool CalculatePinchAngle ()  
    {
            m_TurnAngle = m_TurnAngleDelta = 0;

            // if two fingers are touching the screen at the same time ... 
            if (Input.touchCount == 2)  
            { 
                Touch touch1 = Input.touches[0]; 
                Touch touch2 = Input.touches[1];

                // ... if at least one of them moved ... 
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)  
                {
                        // ... check the delta angle between them ... 
                        m_TurnAngle = Angle(touch1.position, touch2.position); 
                        float prevTurn = Angle(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition); 
                        m_TurnAngleDelta = Mathf.DeltaAngle(prevTurn, m_TurnAngle);

                        // ... if it's greater than a minimum threshold, it's a turn! 
                        if (Mathf.Abs(m_TurnAngleDelta) > MINTURNANGLE)  
                        { 
                                m_TurnAngleDelta *= PINCHTURNRATIO; 
				                m_Rotator.rotation *= Quaternion.Euler(0, 0, -m_TurnAngleDelta); 
								_forcePivotPosition = InitPivotPosition(content, (Input.GetTouch(0).position + Input.GetTouch(1).position) * 0.5f);
								PanRotZoom?.Invoke();
								return(true);
                        }  
                        else  
                        { 
                                m_TurnAngle = m_TurnAngleDelta = 0;
								return(false); 
                        } 
                    } 
            }
			return(false);
    }

    private float Angle (Vector2 pos1, Vector2 pos2)  
    { 
            Vector2 from = pos2 - pos1; 
            Vector2 to = new Vector2(1, 0);
            float result = Vector2.Angle( from, to ); 
            Vector3 cross = Vector3.Cross( from, to );

            if (cross.y > 0)  
            { 
                result = 360f - result; 
            }
            return result; 
    }
	#endregion Private
	#endregion Methods
}
