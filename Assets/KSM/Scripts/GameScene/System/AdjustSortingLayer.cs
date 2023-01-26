using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

public class AdjustSortingLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<SpriteRenderer>())
            GetComponent<SpriteRenderer>().sortingOrder = (int)(transform.position.y * -100);
        else if(GetComponent<SortingGroup>())
            GetComponent<SortingGroup>().sortingOrder = (int)(transform.position.y * -100);
    }

}
