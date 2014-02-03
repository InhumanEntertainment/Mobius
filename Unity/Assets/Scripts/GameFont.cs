using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameFont : ScriptableObject 
{
    public Material Material;
    public string Characters;
    public float Spacing;
    public Color TopColor = new Color(1, 1, 1, 1f);
    public Color BottomColor = new Color(1, 1, 1, 1f);

    //============================================================================================================================================//
    void OnEnable()
    {

	}

    //============================================================================================================================================//
    void OnDisable()
    {

    }

    //============================================================================================================================================//
    void OnDestroy()
    {

    }
}
