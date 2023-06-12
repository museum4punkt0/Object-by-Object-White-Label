using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Manipulation3D : MonoBehaviour
{
    #region Fields
    public bool smooth;
    [SerializeField] private float _minDistanceDelta = 1f; //Minimum finger movement to cause a zoom out/in
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _verticalAngle = 30f; //The maximum vertical angle

    [Space]
    [SerializeField] private float _zoomSpeed = 0.1f;
    [SerializeField] private float _zoomMin = -10;
    [SerializeField] private float _zoomMax = -1;

    [Tooltip("The GameObject that will be turned around and zoomed in. If there are several GameObjects, group them in an empty one")]
    [SerializeField] private GameObject _focusObject;
    [SerializeField] private bool _useMouseControl;
    [SerializeField] private Transform _pivot = null; //The transform of the camera pivot for the y-axis rotation

    #endregion

    #region Properties
    private Transform _verticalPivot; //The transform of the camera pivot for the x-axis rotation
    private Transform _cameraTransform;
    private float _lastTouchDelta; //The distance between the two fingers at the previous frame
    private Vector2 _lastPosition; //Position the touch/mouse was at the previous frame
    private Vector3 _initialCameraPosition; //Initial position of the camera
    private Quaternion _initialCameraRotation; //Initial position of the camera
    private Vector3 _initialPivotPosition; //Initial position of the camera pivot
    private Quaternion _initialPivotRotation; //Initial position of the camera pivot
    private float _initialGODistance; //Initial distance from the camera to the focusObject
    private bool _isRotating; //Have we started rotating?
    private bool _isZooming; //Have we started zooming?

    private bool _isFirstManipulation = true;
    #endregion

    #region Properties
    public UnityEvent FirstManipulated = new();
    #endregion

    #region Methods
    #region Public
    public void SetFocusObject(GameObject gameObject)
    {
        _focusObject = gameObject;
    }
    #endregion

    #region Private
    // Start is called before the first frame update
    void Start()
    {
        _isFirstManipulation = true;
        _verticalPivot = transform.GetChild(0);
        _cameraTransform = transform.GetChild(0).GetChild(0);
        _initialPivotPosition = _pivot.position;
        _initialPivotRotation = _pivot.rotation;
        _initialCameraPosition = _cameraTransform.localPosition;
        _initialCameraRotation = _cameraTransform.rotation;
        _initialGODistance = Vector3.Distance(_focusObject.transform.position, _initialCameraPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if(_focusObject == null)
        {
            return;
        }

        #region MouseControl
        if (_useMouseControl && Input.touchCount == 0)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                Zoom(Input.GetAxis("Mouse ScrollWheel") > 0);
            }
            if(Input.GetMouseButtonDown(0))
            {
                _lastPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                Rotation(Input.mousePosition);
            }
        }
        #endregion MouseControl

        #region TouchControl
        #region Rotation
        if(Input.touchCount != 1)
        {
            _isRotating = false;
        }
        if(Input.touchCount == 1)
        {
            if(_isRotating)
            {
                Rotation(Input.GetTouch(0).position);
            }
            _lastPosition = Input.GetTouch(0).position;
            _isRotating = true;
        }
        #endregion Rotation
        
        #region Zoom
        //zoom
        if(Input.touchCount != 2)
        {
            _isZooming = false;
        } 
        if(Input.touchCount == 2)
        {
            Vector2 firstTouch = Input.GetTouch(0).position;
            Vector2 secondTouch = Input.GetTouch(1).position;
            float touchDelta = Vector2.Distance(firstTouch, secondTouch);

            if(!_isZooming)
            {
                _isZooming = true;
                _lastTouchDelta = touchDelta;
            }
            bool zoomIn = touchDelta - _lastTouchDelta > _minDistanceDelta;
            Zoom(zoomIn);
            _lastTouchDelta = touchDelta;
        }
        #endregion Zoom
        #endregion TouchControl
    }
    private void Rotation (Vector2 position)
    {
        if (_isFirstManipulation)
        {
            FirstManipulated?.Invoke();
            _isFirstManipulation = false;
        }

        float horizontalRotationValue = (position.x - _lastPosition.x > 0 ? _rotationSpeed : position.x - _lastPosition.x != 0 ? -_rotationSpeed : 0);
        float verticalRotationValue = (position.y - _lastPosition.y > 0 ? -_rotationSpeed : position.y - _lastPosition.y < 0 ? _rotationSpeed : 0);
        float distance = Vector3.Distance(_focusObject.transform.position, _cameraTransform.position);
        _pivot.RotateAround(_pivot.position, Vector3.up, horizontalRotationValue * distance/_initialGODistance);
        _verticalPivot.Rotate(verticalRotationValue * distance/_initialGODistance, 0f, 0f);

        //Clamp vertical rotation between -_verticalAngle and _verticalAngle
        float verticalPivotRotationX = Utils.MathUtils.ClampAngle(_verticalPivot.eulerAngles.x, -_verticalAngle, _verticalAngle);
        _verticalPivot.localEulerAngles = new Vector3(verticalPivotRotationX, 0f, 0f);
        _lastPosition = position;
    }

    private void Zoom (bool zoomIn)
    {
        if (_isFirstManipulation)
        {
            FirstManipulated?.Invoke();
            _isFirstManipulation = false;
        }

        _cameraTransform.Translate(0, 0, zoomIn ? _zoomSpeed : -_zoomSpeed);
        _cameraTransform.localPosition = new Vector3(_cameraTransform.localPosition.x,
                                                     _cameraTransform.localPosition.y,
                                                     Mathf.Clamp(_cameraTransform.localPosition.z, _zoomMin, _zoomMax));
        return;
    }
    #endregion

    #endregion
}
