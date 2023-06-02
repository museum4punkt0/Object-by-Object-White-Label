using System.Linq;
/**
 * Created by Willy
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventMapPoi : UnityEvent<Wezit.Poi> { };

public class MapPoiScript : MonoBehaviour
{
	#region Fields
	public static string TAG = "[MapPoiScript]";

	[SerializeField] private List<Image> _imagesToColor = null;

	[SerializeField]
	private Wezit.Poi _wzPoi = null;
	public Wezit.Poi WzPoi { get => _wzPoi; }

	private string _poiId;
	public string PoiId { get => _poiId; }

	private Vector2 _poiPosition = Vector2.positiveInfinity;
	public Vector2 PoiPosition { get => _poiPosition; }

	private Button _btComponent = null;
	private TextMeshProUGUI _textComponent = null;

	public UnityEventMapPoi onPoiClick;
	#endregion Fields

	#region Methods
	#region MonoBehavior
	private void Awake()
	{
		_btComponent = transform.GetComponent<Button>();
		_textComponent = transform.GetComponentInChildren<TextMeshProUGUI>();
	}
	#endregion MonoBehavior

	#region Public
	public virtual void InitWezitData(Wezit.Poi value, int positionIndex, string[] poiCoordinates = null)
	{
		_poiPosition = Vector2.positiveInfinity;

		_wzPoi = value;
		if (_wzPoi != null)
		{
			if (poiCoordinates == null) poiCoordinates = new string[] { "" };
			if (positionIndex < poiCoordinates.Length)
			{
				// int poiIdInteger = 0;
				// if (!String.IsNullOrEmpty(_wzPoi.identifier) && int.TryParse(_wzPoi.identifier, out poiIdInteger))
				// {
				// 	_poiId = _wzPoi.identifier;
				// }
				// else if (!String.IsNullOrEmpty(_wzPoi.subject) && int.TryParse(_wzPoi.subject, out poiIdInteger))
				// {
				// 	_poiId = _wzPoi.subject;
				// }
				// else
				// {
				_poiId = _wzPoi.pid;
				// }

				Wezit.PoiLocation location = PoiLocationStore.GetPoiLocationById(_wzPoi.pid);
				_poiPosition = new Vector2(location.x, location.y);
				if (location != null && !float.IsNaN(_poiPosition.y)) SetZOrder(_poiPosition.y);
				// Canvas thisCanvas = GetComponent<Canvas>();
				// if (thisCanvas != null)
				// {
				// 	thisCanvas.overrideSorting = false;
				// }
				// If position Index is 0 we only get the rotation from type.
				if (positionIndex == 0)
				{
                    int position = 0;
					if (!String.IsNullOrEmpty(poiCoordinates[positionIndex]))
					{
                        if (int.TryParse(poiCoordinates[positionIndex], out position))
                        {
                            SetRotation(position);
                        }
                        else
                        {
                            Debug.LogWarning("Couldn't parse poi coordinate : " + poiCoordinates[positionIndex]);
                        }
					}
				}
				// If not we get the position and the rotation from type.
				else
				{
					string[] splitPoiCoordinates = poiCoordinates[positionIndex].Split(';');
					string[] position = splitPoiCoordinates[0].Split(',');
					string rotation = splitPoiCoordinates[1];

					position[0] = position[0].Replace(" ", "").Replace(".", ",");
					position[1] = position[1].Replace(" ", "").Replace(".", ",");
					if (float.TryParse(position[0], out _poiPosition.x) == false)
					{
						Debug.LogError(TAG + "Could not parse " + position[0] + " to float");
					}

					if (float.TryParse(position[1], out _poiPosition.y) == false)
					{
						Debug.LogError(TAG + "Could not parse " + position[1] + " to float");
					}

					SetRotation(int.Parse(rotation));
				}

				// SetText(_poiId);
			}
			else
			{
				Debug.LogError(TAG + "Could not find the wanted poi position from infos given in type entry. PositionIndex : " + positionIndex + " infos length : " + poiCoordinates.Length);
			}

		}
	}

	public virtual void OnPoiClick()
	{
		if (_wzPoi != null)
        {
            StoreAccessor.Dispatch(Store.SelectedPoi.ActionCreator.Set(_wzPoi));
        } 
	}

	public void SetZOrder(float yLocation)
	{
		GetComponent<Canvas>().overrideSorting = true;
		GetComponent<Canvas>().sortingOrder = Mathf.RoundToInt(Utils.MathUtils.Map(yLocation, 0, 1, 200, 20));
	}

	public void SetRotation(float rotationAngle)
	{
		if (_textComponent) _textComponent.gameObject.transform.rotation = Quaternion.Euler(0, 0, -rotationAngle);
		gameObject.transform.localRotation = Quaternion.Euler(0, 0, rotationAngle);
	}

	public void SetPinColor(Color color)
	{
		if (_imagesToColor != null)
		{
			foreach (Image image in _imagesToColor)
			{
				image.color = color;
			}
		}
	}

	public void SetText(string textContent)
	{
		if (_textComponent && string.IsNullOrEmpty(textContent) == false)
		{
			_textComponent.text = StringUtils.CleanFromWezit(textContent);
		}
	}

	public void SetTextStyle(TextStyle textStyle)
	{
		if (_textComponent)
		{
			_textComponent.font = textStyle.fontLabelTitle;
			_textComponent.fontSize = textStyle.fontSizeLabelTitle;
			_textComponent.color = textStyle.fontColorLabelTitle;
		}
	}

	public virtual void SetSelected(bool value)
	{ }
	#endregion Public

	#region Private
	private void Update()
	{
		ScaleOnMapManager();
	}

	private void OnEnable()
	{
		ScaleOnMapManager();

		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		RemoveListeners();

		if (_btComponent != null)
		{
			_btComponent.onClick.AddListener(OnPoiClick);
		}
	}

	private void RemoveListeners()
	{
		if (_btComponent != null)
		{
			_btComponent.onClick.RemoveAllListeners();
		}
	}
	#endregion Private

	#region Internals
	protected virtual void ScaleOnMapManager()
	{ }
	#endregion Internals
	#endregion Methods
}
