using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Fields
    [SerializeField] private float _scrollSpeed = 10f;
    [SerializeField] private bool _positiveScroll = true;
    private enum axis {x, y};
    [SerializeField] private axis _axisChoice;
    [SerializeField] private ScrollRect _scrollRect = null;
    [SerializeField] private Image _buttonImage = null;
    private RectTransform _scrollContentParent = null;
    private bool _isPressed = false;
    private Color _inactiveColor;
    private Color _activeColor;
    private float _threshold = 1e-3f;
    private bool _leftReached = false;
    private bool _rightReached = false;
    private bool _topReached = false;
    private bool _bottomReached = false;
    #endregion

    #region Methods
    #region Public
    public void CheckSize()
    {
        if(Mathf.Abs(_scrollContentParent.sizeDelta.x) > Mathf.Abs(_scrollRect.GetComponent<RectTransform>().sizeDelta.x))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void ResetButton()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _bottomReached = false;
        _topReached = false;
        _leftReached = false;
        _rightReached = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }
    #endregion Public

    #region Private
    private void Start() 
    {
        _scrollContentParent = _scrollRect.content;
        _activeColor = _buttonImage.color;
        _inactiveColor = new Color(_buttonImage.color.r, 
                                  _buttonImage.color.g, 
                                  _buttonImage.color.b,
                                  0.5f);
        if(!_positiveScroll) _buttonImage.color = _inactiveColor;
        if(_positiveScroll) _buttonImage.color = _activeColor;
    }
    private void Update()
    {
        if (!_isPressed)
        {
            CheckPosition();
            return;
        } 

        else
        {
            Scroll();
        }        
    }

    private void Scroll()
    {
        Vector3 newPos;
        int scrollDirection = _positiveScroll ? -1 : 1;
        switch(_axisChoice)
        {
            case axis.x:
                newPos = new Vector3(_scrollContentParent.localPosition.x + _scrollSpeed * scrollDirection, 
                                    _scrollContentParent.localPosition.y, 
                                    _scrollContentParent.localPosition.z);
                break;
            case axis.y:
                newPos = new Vector3(_scrollContentParent.localPosition.x, 
                                    _scrollContentParent.localPosition.y + _scrollSpeed * scrollDirection, 
                                    _scrollContentParent.localPosition.z);
                break;
            default:
                newPos = Vector3.zero;
                break;
        }
        _scrollContentParent.localPosition = newPos;
    }
    private void CheckPosition()
    {
        switch(_axisChoice)
        {
            case axis.x:
                if(_positiveScroll && _scrollRect.normalizedPosition.x >= 1f - _threshold)
                {
                    if(!_rightReached)
                    {
                        _buttonImage.color = _inactiveColor;
                        _rightReached = true;
                    } 
                }
                else if(!_positiveScroll && _scrollRect.normalizedPosition.x <= _threshold)
                {
                    if(!_leftReached) 
                    {
                        _buttonImage.color = _inactiveColor;
                        _leftReached = true;
                    }
                }
                else
                {
                    _rightReached = false;
                    _leftReached = false;
                    _buttonImage.color = _activeColor;
                }
                break;
            case axis.y:
                if(_positiveScroll && _scrollRect.normalizedPosition.y >= 1f - _threshold)
                {
                    if(!_topReached)
                    {
                        _buttonImage.color = _inactiveColor;
                        _topReached = true;
                    }
                }
                else if (!_positiveScroll && _scrollRect.normalizedPosition.y <= 0 - _threshold)
                {
                    if(!_bottomReached)
                    {
                        _buttonImage.color = _inactiveColor;
                        _bottomReached = true;
                    }
                }
                else if(_bottomReached || _topReached)
                {
                    _bottomReached = false;
                    _topReached = false;
                    _buttonImage.color = _activeColor;
                }
                break;
            default:
                break;
        }
    }
    #endregion Private
    #endregion
}
