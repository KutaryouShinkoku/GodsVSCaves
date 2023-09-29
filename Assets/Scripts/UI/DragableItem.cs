using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentAfterDrag;
    private RectTransform rectTransform;
    private Vector3 screenPosition;
    private Vector3 mousePositionOnScreen;
    private Vector3 mousePositionInWorld;

    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.enterEventCamera, out pos);
        rectTransform.position = pos;
        Debug.Log("Pos:"+pos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log("End Drag");
        transform.SetParent(parentAfterDrag);
        Debug.Log(transform.position);
    }
}

