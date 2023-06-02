using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AreaDropHandler : MonoBehaviour, IDropHandler
{
	#region Const
	private const string TAG = "<color=green>[AreaDropHandler]</color>";
	#endregion Const

	#region Fields
	[SerializeField]
	private Wezit.Poi _wzPoi = null;
	public Wezit.Poi WzPoi { get => _wzPoi; }

	private Action<Wezit.Poi> _dropSuccessEvent = null;
	private Action<Wezit.Poi> _dropFailEvent = null;
	#endregion Fields

	#region Properties
	public event Action<Wezit.Poi> OnDropSuccess
	{
		add { _dropSuccessEvent -= value; _dropSuccessEvent += value; }
		remove { _dropSuccessEvent -= value; }
	}

	public event Action<Wezit.Poi> OnDropFail
	{
		add { _dropFailEvent -= value; _dropFailEvent += value; }
		remove { _dropFailEvent -= value; }
	}
	#endregion Properties

	#region Methods
	#region MonoBehaviour
	#endregion MonoBehaviour

	#region Public
	public void InitData(Wezit.Poi itemPoi)
	{
		ResetData();
		_wzPoi = itemPoi;
	}

	public void ResetData()
	{
		_wzPoi = null;
	}

	public void OnDrop(PointerEventData eventData)
	{
		RectTransform area = transform as RectTransform;

		ItemDragHandler itemDragHandler = eventData.pointerDrag.gameObject.GetComponent<ItemDragHandler>();
		Wezit.Poi itemPoi = itemDragHandler.WzPoi;
		if (itemPoi != null)
		{
			if (CanDropItem(itemPoi))
			{
				itemDragHandler.OnValidDrop();
				if (_dropSuccessEvent != null) _dropSuccessEvent(itemPoi);
			}
			else
			{
				itemDragHandler.OnFailedDrop();
				if (_dropFailEvent != null) _dropFailEvent(itemPoi);
			}
		}
	}
	#endregion Public

	#region Internals
	protected virtual bool CanDropItem(Wezit.Poi wzPoi)
	{
		return (wzPoi.pid == WzPoi.pid);
	}
	#endregion Internals
	#endregion
}
