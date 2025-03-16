using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDetection : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 initialPosition = eventData.pressPosition;
        Vector2 finalPosition = eventData.position;

        Vector2 dragVectorDirection = finalPosition - initialPosition;

        if (Math.Abs(dragVectorDirection.x) > Math.Abs(dragVectorDirection.y))
        {
            if (dragVectorDirection.x > Screen.width * 0.2f && initialPosition.x < Screen.width * 0.2f)
                NavigationManager.instance.previousPage();
            else if (-dragVectorDirection.x > Screen.width * 0.2f && initialPosition.x > Screen.width * 0.8f)
                NavigationManager.instance.previousPage();
        }
    }
}
