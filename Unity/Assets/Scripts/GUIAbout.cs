using UnityEngine;
using System.Collections;

public class GUIAbout : MonoBehaviour
{
    public Texture Background;
    public GUIStyle Level1;

    //============================================================================================================================================//
    void OnGUI()
    {
        // Background //
        //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Background, ScaleMode.StretchToFill);

        // Website //
        if (GUI.Button(new Rect(10, 10, Screen.width - 20, 90), "Website"))
            Application.OpenURL("http://www.larssontech.com");

        // Email //
        if (GUI.Button(new Rect(10, 110, Screen.width - 20, 90), "Email"))     
            Application.OpenURL("mailto:support@larssontech.com");
        
        // Back //
        if (GUI.Button(new Rect(10, Screen.height - 60, 200, 50), "Back"))
            Application.LoadLevel("MainMenu");
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
}
