using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWithSelectEvent : Button
{
    public UnityEvent<bool> ButtonSelected = new UnityEvent<bool>();

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        ButtonSelected?.Invoke(true);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        ButtonSelected?.Invoke(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        ButtonSelected?.Invoke(false);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ButtonSelected?.Invoke(false);
    }

}
