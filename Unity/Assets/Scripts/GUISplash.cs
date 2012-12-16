using UnityEngine;
using System.Collections;

public class GUISplash : MonoBehaviour 
{
    public Texture MenuTex;
    public GUIStyle GUIStyle;

    //============================================================================================================================================//
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "", GUIStyle.none))
        {
            Application.LoadLevel("MainMenu");
        }

        float w = Screen.width / 1.2f;
        float h = w * ((float)MenuTex.height / MenuTex.width);
        print(w + " : " + h);

        if (GUI.Button(new Rect(Screen.width / 2 - w / 2, Screen.height / 2 - h / 2, w, h), MenuTex, GUIStyle))
        {
            Application.LoadLevel("MainMenu");
        }
    }

    //============================================================================================================================================//
    /*void OnDrawGizmos()
    {
        Camera camera = GameObject.Find("GameCamera").camera;
        
        float w = Screen.width / 2;
        float h = w / 4;

        Vector3 topleft = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2 - w / 2, Screen.height / 2 - h / 2, camera.nearClipPlane));
        Vector3 wh = camera.ScreenToViewportPoint(new Vector3(w, h, camera.nearClipPlane)) - topleft;
        //print(wh + " : " + topleft);

        Gizmos.DrawLine(new Vector3(wh.x, -10, 0), new Vector3(wh.x, 10, 0));
        Gizmos.DrawLine(new Vector3(-10, wh.y, 0), new Vector3(10, wh.y, 0));
        
        Gizmos.DrawGUITexture(new Rect(topleft.x, topleft.y, wh.x, wh.y), MenuTex);
        Gizmos.DrawGUITexture(new Rect(0, 0, wh.x, wh.y), MenuTex);
        //Gizmos.DrawGUITexture(new Rect(0, 0, 200, 50), MenuTex);
        
    }*/
}
