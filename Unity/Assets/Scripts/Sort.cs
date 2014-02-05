using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Sort : MonoBehaviour
{
    public string SortingLayer;
    public int SortingOrder;

    //============================================================================================================================================//
    void Update()
    {
        if (SortingOrder != renderer.sortingOrder)
            renderer.sortingOrder = SortingOrder;

        if (SortingLayer != renderer.sortingLayerName)
            renderer.sortingLayerName = SortingLayer;
    }
}

