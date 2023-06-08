using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectUIClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool quitting;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ClickManager.ClickingOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClickManager.ClickingOnUI = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ClickManager.ClickingOnUI = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ClickManager.ClickingOnUI = false;
    }

    private void OnDisable()
    {
        if(!quitting) ClickManager.ClickingOnUI = false;
    }

    private void OnApplicationQuit()
    {
        quitting = true;
    }
}
