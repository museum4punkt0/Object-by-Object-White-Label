using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectUIClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool quitting;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ClickManager.Instance) ClickManager.Instance.ClickingOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(ClickManager.Instance) ClickManager.Instance.ClickingOnUI = false;
    }

    private void OnDisable()
    {
        if(!quitting) ClickManager.Instance.ClickingOnUI = false;
    }

    private void OnApplicationQuit()
    {
        quitting = true;
    }
}
