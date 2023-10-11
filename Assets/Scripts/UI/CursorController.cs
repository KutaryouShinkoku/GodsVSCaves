using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public Texture2D curNormal;
    public Texture2D curInteract;
    public Texture2D curSelect;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotspot = Vector2.zero;
    private bool isSelected;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(curInteract, hotspot, cursorMode);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Cursor.SetCursor(curSelect, hotspot, cursorMode);
        isSelected = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSelected)
        {
            Cursor.SetCursor(curInteract, hotspot, cursorMode);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(curNormal, hotspot, cursorMode);
    }
}
