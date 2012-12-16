using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour 
{
    public KeyCode Key = KeyCode.Escape;
    public string Level;

    //============================================================================================================================================//
    void Start()
    {
	
	}

    //============================================================================================================================================//
    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            if (Level.ToLower() == "exit")
            {
                Application.Quit();
            }
            else
                Application.LoadLevel(Level);
        }
	}
}
