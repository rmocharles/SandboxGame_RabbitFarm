using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragStickle : MonoBehaviour
{
    private bool isDragged = false;
    private Vector3 mouseDragStartPosition;
    private Vector3 spriteDragStartPosition;

    private void OnMouseDown()
    {
        print(1);
        isDragged = true;
        mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteDragStartPosition = transform.localPosition;
    }

    private void OnMouseDrag()
    {
        if (isDragged)
        {
            transform.localPosition = spriteDragStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
    }
    //public Canvas canvas;

    //private Vector3 previousPosition;

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    previousPosition = transform.position;
    //    Debug.Log("OnBeginDrag");
    //    GetComponent<CanvasGroup>().alpha = .6f;
    //    GetComponent<CanvasGroup>().blocksRaycasts = false;
    //    transform.parent.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    Debug.Log("Drag");
    //    GetComponent<RectTransform>().position = eventData.position;
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    Debug.Log("OnEndDrag");
    //    GetComponent<CanvasGroup>().alpha = 1f;
    //    GetComponent<CanvasGroup>().blocksRaycasts = true;
    //    transform.parent.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    //    transform.parent.gameObject.SetActive(false);
    //}

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerDown");

    //}
}
