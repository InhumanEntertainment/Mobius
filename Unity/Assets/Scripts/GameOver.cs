using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour
{
    public Texture Texture;
    bool isGameOver;

    //============================================================================================================================================//
    void Start()
    {
        Time.timeScale = 1;
    }

    //============================================================================================================================================//
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Over();
        }
        if (isGameOver && Input.GetMouseButtonDown(0))
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    //============================================================================================================================================//
    public void Over()
    {
        Time.timeScale = 0;
        this.gameObject.guiTexture.enabled = true;
        isGameOver = true;
    }

    //============================================================================================================================================//
    void OnGUI()
    {
        if (isGameOver)
        {
            // Draw Pause Menu //
            GUI.color = new Color(1, 1, 1, 0.9f);
            //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture);

            GUI.color = Color.white;
            //if (GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "", GUIStyle.none))
              //  Application.LoadLevel(Application.loadedLevelName);

            // Retry //
            //if (GUI.Button(new Rect(Screen.width / 2 - 100, 100, 200, 50), "Retry"))
             //   Application.LoadLevel(Application.loadedLevelName);

            // Restart //
            //if (GUI.Button(new Rect(Screen.width / 2 - 100, 200, 200, 50), "Level Select"))
             //   Application.LoadLevel("Levels");

            // Menu //
            //if (GUI.Button(new Rect(Screen.width / 2 - 100, 400, 200, 50), "Back to Menu"))
             //   Application.LoadLevel("MainMenu");
        }
    }
}
