using UnityEngine;
using System.Collections;

public class MenuScreen : GameScreen 
{
    public GameData Data;
    public GameText HighScoreText;
    public GameObject RateButton;

    //============================================================================================================================================================================================//
    public void SlideFromTop()
    {
        Audio.Music.Pause();
        HighScoreText.Text = string.Format("{0:n0}", Data.HighScore);

#if UNITY_WP8 || UNITY_ANDROID || UNITY_IPHONE || UNITY_METRO
        RateButton.SetActive(true);
#else
        RateButton.SetActive(false);
#endif
    }

    //============================================================================================================================================================================================//
    public void SlideToBottom()
    {
        gameObject.SetActive(false);
    }

    //============================================================================================================================================================================================//
    public void About()
    {
        print("Frontend: About");
        App.Instance.SetScreen("About");
    }

    //============================================================================================================================================================================================//
    public void Rate()
    {
        print("Frontend: Rate");

#if UNITY_WP8
        Application.OpenURL("http://www.windowsphone.com/s?appid=a7a12b7d-53e0-4043-a3f0-9655bf94c6b2");

#elif UNITY_ANDROID
        string id = "com.inhuman.absolon";
        if(SystemInfo.deviceModel.ToLower().Contains("kindle"))
		    Application.OpenURL("amzn://apps/android?p=" + id);
	    else
		    Application.OpenURL("market://details?id=" + id);	

#elif UNITY_METRO
        //Application.OpenURL("ms-windows-store:PDP?PFN=");
        Application.OpenURL("http://www.inhumanize.com");

#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.com/apps/absolon");

#endif
    }

    //============================================================================================================================================================================================//
    public void Facebook()
    {
        print("Frontend: Facebook");

        Application.OpenURL("http://www.facebook.com/inhumanentertainment");
    }

    //============================================================================================================================================================================================//
    public void Twitter()
    {
        print("Frontend: Twitter");

        Application.OpenURL("http://twitter.com/InhumanEnt");
    }

    //============================================================================================================================================================================================//
    public void NewGame()
    {
        print("Frontend: Play");
        App.Instance.SetScreen("Game");
        Game.Instance.NewGame();
    }

}
