using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour 
{
    public Texture Texture;
    public Color Color = Color.white;
    public float FadeTime = 1;

    //============================================================================================================================================//
    void OnGUI()
    {
        float a = Time.timeSinceLevelLoad / FadeTime;
        a = Mathf.Min(1, a);

        if (a == 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            GUI.color = new Color(Color.r, Color.g, Color.b, 1 - a);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture, ScaleMode.StretchToFill, true);
        }
    }
}
