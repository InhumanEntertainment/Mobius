using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
    private bool MouseDown = false;
    private Vector2 MousePosition = Vector2.zero;

    //============================================================================================================================================//
    void Update()
    {
        if (Input.touchCount > 0)
        {
            // Set Position //
            MousePosition = Input.touches[0].position;

            if (MouseDown)
            {
                // Splash //
                if (name == "Menu_Splash")
                {
                    Application.LoadLevel("MainMenu");
                }

                // Menu //
                if (name == "Menu_Play")
                {
                    Application.LoadLevel("Game");
                }
            }
            else
            {
                MouseDown = true;
            }
        }
        else
        {
            if (MouseDown)
            {
                // Check Positions// 
            }
        }

        
    }

    //============================================================================================================================================//
    void OnMouseEnter()
    {
        renderer.material.color = Color.cyan;
    }

    //============================================================================================================================================//
    void OnMouseExit()
    {     
        renderer.material.color = Color.white;
    }

    //============================================================================================================================================//
    void OnMouseUp()
    {
        // Main Menu //
        if (name == "Menu_Play")
        {
            Application.LoadLevel("Game");
        }
        else if (name == "Menu_About")
        {
            Application.LoadLevel("About");
        }
        else if (name == "Menu_Exit")
        {
            Application.Quit();
        }

        // About Menu //
        if (name == "Menu_Back")
        {
            Application.LoadLevel("MainMenu");
        }
        else if (name == "Menu_Website")
        {
            Application.OpenURL("http://www.larssontech.com");
        }
        else if (name == "Menu_Email")
        {
            Application.OpenURL("mailto:support@larssontech.com");
        }

        // Splash //
        if (name == "Menu_Splash")
        {
            Application.LoadLevel("MainMenu");
        }

    }
}
