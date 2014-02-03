using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class GameFontEditor 
{
    //============================================================================================================================================//
    [MenuItem("Assets/Create/Game Font")]
    public static void CreateFont()
    {
        ScriptableObjectUtility.CreateAsset<GameFont>();
    }
}
