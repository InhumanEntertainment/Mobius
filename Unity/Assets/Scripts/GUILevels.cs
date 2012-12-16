using UnityEngine;
using System.Collections;

public class GUILevels : MonoBehaviour
{
    public Texture Background;
    public GUIStyle Level1;

    //============================================================================================================================================//
    void OnGUI()
    {
        // Background //
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background, ScaleMode.StretchToFill);

        // Levels //
        if (GUI.Button(new Rect(10, 10, Screen.width - 20, 90), "Prototype"))
            StartLevel("Game");

        // Back //
        if (GUI.Button(new Rect(10, Screen.height - 60, 200, 50), "Back"))
            Application.LoadLevel("MainMenu");

        if (async != null)
        {
            GUI.Button(new Rect(10, 310, (Screen.width - 20) * async.progress, 10), "");
            print(async.isDone + " : " + async.progress);
        }
    }

    //============================================================================================================================================//
    bool DrawGUI(GUIStyle style, float top, float div)
    {
        float w = Screen.width / div;
        if (w > style.normal.background.width)
            w = style.normal.background.width;
        float h = w * ((float)style.normal.background.height / style.normal.background.width);

        return GUI.Button(new Rect(Screen.width / 2 - w / 2, top, w, h), "", style);
    }

    //============================================================================================================================================//
    AsyncOperation async;

    void StartLevel(string level)
    {
        async = Application.LoadLevelAsync(level);
    }


}
