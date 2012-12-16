using UnityEngine;
using System.Collections;

public class GUIMenu : MonoBehaviour 
{
    public Texture BackgroundTex;
    public Texture Title;

    //public Texture Play;
    public GUIStyle PlayStyle;

    //public Texture About;
    public GUIStyle AboutStyle;

    //public Texture Modes;
    public GUIStyle ModesStyle;

    public Texture Exit;    
    
    public GUIStyle GUIStyle;

    //============================================================================================================================================//
    void OnGUI()
    {
        float w = Screen.width / 1f;
        float h = w * ((float)Title.height / Title.width);

        //GUI.Box(new Rect(0, 0, Screen.width, Screen.height), BackgroundTex, GUIStyle);
        
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackgroundTex, ScaleMode.StretchToFill);
        //GUI.Box(new Rect(Screen.width / 2 - w / 2, 20, w, h), Title, GUIStyle);
        DrawGUI(GUIStyle, 20, 0.8f);

        // Play //
        if (DrawGUI(PlayStyle, 250, 1.2f))
        {
            Music.Instance.Destroy();

            Application.LoadLevel("Game");
        }

        // Modes //
        if (DrawGUI(ModesStyle, 350, 1.2f))
            Application.LoadLevel("Levels");

        // About //
        if (DrawGUI(AboutStyle, 450, 1.2f))
            Application.LoadLevel("About");

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
