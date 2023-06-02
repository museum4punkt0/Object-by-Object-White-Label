using System;
using UnityEngine;

public class ItemDragContainer : MonoBehaviour
{
	#region Fields
	[SerializeField] private ItemDragHandler _itemDragHandler = null;

	public ItemDragHandler ItemDragHandler { get => _itemDragHandler; }
	#endregion Fields

	#region Properties
	#endregion Properties

	#region Methods
	public void Init(Wezit.Poi itemPoi, int itemIndex, Boolean isDraggableOnInit)
	{
		_itemDragHandler.Init(itemPoi, itemIndex, isDraggableOnInit);
	}
	#endregion Methods
}