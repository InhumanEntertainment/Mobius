using UnityEngine;
using System.Collections;

public class Scores : MonoBehaviour 
{
    public int Score = 0;
    public float FPS = 0;

    //============================================================================================================================================//
    void Awake()
    {
        Application.targetFrameRate = 2000;
        Time.maximumDeltaTime = 0.1f;
	}

    //============================================================================================================================================//
    void Update()
    {
        Score += (int)(Time.deltaTime * 1000);

        this.guiText.text = "Score: " + AddCommas(Score);
        FPS = FPS * 0.99f + (1 / Time.deltaTime) * 0.01f;
        //this.guiText.text = "Smooth: " + FPS.ToString("N1") + "    FPS: " + (1 / Time.deltaTime).ToString("N1");

        //print("Max: " + Time.maximumDeltaTime);
	}

    //============================================================================================================================================//
    string AddCommas(int number)
    {
        string text = number.ToString("N0");

        return text;
    }
}
