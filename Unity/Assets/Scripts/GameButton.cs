using UnityEngine;
using System.Collections;

public class GameButton : MonoBehaviour
{
    public Sprite Sprite;
    public Sprite SpriteDown;
    public Sprite SpriteOver; 
    
    public string Message = "";
    public GameObject Target;

    static public bool InUse = false;
    bool touching = false;

    //=======================================================================================================================================================/
    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool mouse_over = collider2D.OverlapPoint(mouse);
        bool mouse_down = Input.GetMouseButton(0);
        bool mouse_touched = Input.GetMouseButtonDown(0);
        bool mouse_released = Input.GetMouseButtonUp(0);

        SpriteRenderer render = renderer as SpriteRenderer;

        if (mouse_over)
            InUse = true;

        if (mouse_over && mouse_touched)
            touching = true;

        // State //
        if (touching && mouse_down && mouse_over)
        {
            if (SpriteDown != null) 
                render.sprite = SpriteDown;        
        }
        else if(mouse_over)
        {
            if(SpriteOver != null)
                render.sprite = SpriteOver;

            // Event //
            if (mouse_released && touching)
            {
                if (Message != "")
                {
                    print("Button: " + Message);
                    if (Message != "" && Target != null)
                        Target.SendMessage(Message, SendMessageOptions.DontRequireReceiver);
                    else
                        SendMessage(Message, SendMessageOptions.DontRequireReceiver);

                    Audio.PlaySound("Button");
                }
            }
        }
        else if (mouse_released)
        {
            touching = false;
        }
        else
        {
            if (Sprite != null)
                render.sprite = Sprite;
        }
    }

    //=======================================================================================================================================================/
    void LateUpdate()
    {
        InUse = false;
    }
}
