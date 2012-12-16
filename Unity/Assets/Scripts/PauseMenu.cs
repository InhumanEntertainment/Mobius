using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public bool Paused = false;
    public Texture Texture;
    public GUIStyle PauseButton;

    //============================================================================================================================================//
    void Start()
    {
        Time.timeScale = 1;
	}

    //============================================================================================================================================//
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Paused)
                Pause(true);
            else
                Pause(false);
        }
	}

    //============================================================================================================================================//
    void Pause(bool value)
    {
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
        
        Paused = value;
    }

    //============================================================================================================================================//
    void OnGUI()
    {
        if (Paused)
        {
            // Draw Pause Menu //
            GUI.color = new Color(1, 1, 1, 0.9f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture);

            GUI.color = Color.white;

            // Resume //
            if (GUI.Button(new Rect(Screen.width / 2 - 100, 100, 200, 50), "Resume"))
                Pause(false);

            // Restart //
            if (GUI.Button(new Rect(Screen.width / 2 - 100, 200, 200, 50), "Restart Level"))
                Application.LoadLevel(Application.loadedLevelName);

            // Switch Level //
            if (GUI.Button(new Rect(Screen.width / 2 - 100, 300, 200, 50), "Different Level"))
                Application.LoadLevel("Levels");

            // Menu //
            if (GUI.Button(new Rect(Screen.width / 2 - 100, 400, 200, 50), "Back to Menu"))
                Application.LoadLevel("MainMenu");
        }
        else
        {
            // Draw Pause Button //
            if (GUI.Button(new Rect(Screen.width - 60, 10, 50, 50), "| |"))
                Pause(true);
        }
        
    }
}
