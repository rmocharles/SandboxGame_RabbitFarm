using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{
    public void TEST(PointerEventData data)
    {
        data.position = transform.localPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        GetComponentInParent<GraphicRaycaster>().Raycast(data, results);
        if (results.Count > 0)
            Debug.LogError(results[0].gameObject.name);
    }
}