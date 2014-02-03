using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScreen : MonoBehaviour 
{
    public string OpenAnimation = "SlideFromTop";
    public string CloseAnimation = "SlideToBottom";

    //============================================================================================================================================//
    public void Open()
    {
        if (OpenAnimation != "")
            GetComponent<MenuAnimation>().Play(OpenAnimation);
    }

    //============================================================================================================================================//
    public void Close()
    {
        if(CloseAnimation != "")
            GetComponent<MenuAnimation>().Play(CloseAnimation);
    }
}
