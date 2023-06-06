using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	#region Const
	private const string TAG = "<color=green>[ItemDragHandler]</color>";
	#endregion Const

	#region Fields
	#region Serialized
	[SerializeField]
	private Wezit.Poi _wzPoi = null;
	public Wezit.Poi WzPoi { get => _wzPoi; }
	[Space]
	[SerializeField] private Image _poiImage = null;
	[SerializeField] private float _scaleChangeOnDrag = 1f;
	[SerializeField] private Color _imageColorOnDrag = Color.white;
	[SerializeField] private int _itemIndex;
	#endregion Serialized

	private RectTransform _rectTransform = null;
	private Canvas _rootCanvas = null;
	private Camera _uiCamera = null;
	private bool _isDragging;
	private bool _isDropped;
	private bool _isDraggable = true;
	private bool _dragFromCenter = true;
	private Vector3 _originalScale = Vector3.one;
	private Color _originalColor = Color.white;
	private int _originalCanvasSortingOrder = 0;
	private float _poiImageAlphaOnNotDraggable = 0.25f;
	private Vector3 _onBeginDragOffset = Vector3.zero;
	private Vector3 _onDragPosition = Vector3.zero;
	#endregion Fields

	#region Properties
	public Image PoiImage { get => _poiImage; }
	public bool IsDropped { get => _isDropped; set => _isDropped = value; }
	public bool IsDraggable
	{
		get => _isDraggable;
		set
		{
			_isDraggable = value;
			if (_poiImage) _poiImage.CrossFadeAlpha(_isDraggable ? 1 : _poiImageAlphaOnNotDraggable, 0, false);
		}
	}
	public bool DragFromCenter { get => _dragFromCenter; set => _dragFromCenter = value; }
	public int ItemIndex { get => _itemIndex; }
	#endregion Properties

	#region Methods
	#region MonoBehaviour
	private void Awake()
	{
		_rectTransform = transform as RectTransform;
		_rootCanvas = GetComponentInParent<Canvas>();
		if (_rootCanvas && _rootCanvas.renderMode == RenderMode.ScreenSpaceCamera) _uiCamera = _rootCanvas.worldCamera;
	}

	private void OnDestroy()
	{
		_wzPoi = null;
		_isDragging = false;
	}
	#endregion MonoBehaviour

	#region Public
	public void Init(Wezit.Poi wzPoi, int itemIndex, Boolean isDraggableOnInit)
	{
		_wzPoi = wzPoi;
		_itemIndex = itemIndex;
		IsDraggable = isDraggableOnInit;

		if (_poiImage) LoadImage(_poiImage, wzPoi, Wezit.RelationName.SHOW_PICTURE, WezitSourceTransformation.original, true, 0, IsDraggable ? 1 : _poiImageAlphaOnNotDraggable);

		_originalScale = _rectTransform.localScale;
		_originalColor = _poiImage.color;
		if (_rootCanvas) _originalCanvasSortingOrder = _rootCanvas.sortingOrder;
	}

	public void Reset()
	{
		if (_poiImage) UnloadImage(_poiImage);
		OnDestroy();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!IsDraggable)
		{
			eventData.pointerDrag = null;
			return;
		}

		if (!DragFromCenter)
		{
			_onBeginDragOffset = Vector3.zero;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, Input.mousePosition, _uiCamera, out _onBeginDragOffset);
			_onBeginDragOffset -= _rectTransform.position;
			_onBeginDragOffset.z = 0;
		}


		GetComponent<CanvasGroup>().blocksRaycasts = false;
		SetZOrder(true);

		_isDragging = true;
		_isDropped = false;

		_rectTransform.localScale *= _scaleChangeOnDrag;
		_poiImage.color = _imageColorOnDrag;
	}

	public void OnDrag(PointerEventData eventData)
	{
		_onDragPosition = Vector3.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTransform, Input.mousePosition, _uiCamera, out _onDragPosition);
		_rectTransform.position = _onDragPosition - (!DragFromCenter ? _onBeginDragOffset : Vector3.zero);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		transform.localPosition = Vector3.zero;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		SetZOrder(false);

		_isDragging = false;

		if (_isDropped == false)
		{
			OnFailedDrop();
		}

		_rectTransform.localScale = _originalScale;
		_poiImage.color = _originalColor;
	}

	public void OnValidDrop()
	{
		_isDropped = true;
		_poiImage.gameObject.SetActive(false);
	}

	public void OnFailedDrop()
	{
		_isDropped = true;
	}
	#endregion Public

	#region Private
	private void LoadImage(Image imageComponent, Wezit.Node wzData, string targetRelation = "relationForShowPicture", string targetWzSourceTransformation = "original", bool fillParent = true, float crossFadeAlphaDuration = 0.25f, float alphaOnLoaded = 1)
	{
		if (imageComponent != null)
		{
			if (wzData != null)
			{
				foreach (Wezit.Relation relation in wzData.Relations)
				{
					if (relation.relation == targetRelation)
					{
						StartCoroutine(ImageUtils.SetImage(imageComponent, 
															relation.GetAssetSourceByTransformation(targetWzSourceTransformation), 
															relation.GetAssetMimeTypeByTransformation(targetWzSourceTransformation), 
															fillParent, 
															null, 
															crossFadeAlphaDuration, 
															true, 
															alphaOnLoaded));
						break;
					}
				}
			}
			else
			{
				UnloadImage(imageComponent);
			}
		}
	}

	private void UnloadImage(Image imageComponent)
	{
		if (imageComponent) ImageUtils.ResetImage(imageComponent);
	}

	private void SetZOrder(Boolean above)
	{
		if (_rootCanvas)
		{
			_rootCanvas.overrideSorting = above;
			_rootCanvas.sortingOrder = _originalCanvasSortingOrder + (above ? 1 : 0);
		}
	}
	#endregion Private
	#endregion Methods
}