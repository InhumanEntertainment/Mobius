using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class GameDataEditor 
{
    //============================================================================================================================================//
    [MenuItem("Assets/Create/Game Data")]
    public static void CreateData()
    {
        ScriptableObjectUtility.CreateAsset<GameData>();
    }
}
