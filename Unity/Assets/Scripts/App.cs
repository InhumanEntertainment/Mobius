using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class App : MonoBehaviour 
{
	static public App Instance;	
	public int TargetFramerate = 60;
	float FPS = 60;
	
	// Frontend //
	public GameScreen[] Screens;
	public GameScreen CurrentScreen;
	public GameScreen LastScreen;
	
	//============================================================================================================================================================================================//
	void Awake()
	{
		// Singleton //
		if (App.Instance == null)
		{
			Instance = this;
			Application.targetFrameRate = TargetFramerate;
			
			if(Screens.Length > 0)
			{
                SetScreen("Menu");
			}          
		}
	}
	
	//============================================================================================================================================================================================//
	void Update()
	{
		if (TargetFramerate != Application.targetFrameRate)
		{
			Application.targetFrameRate = TargetFramerate;
		}
		
		// Update FPS Counter //
		FPS = Mathf.Lerp(FPS, Time.deltaTime > 0 ? 1f / Time.deltaTime : 0, 0.03f);   
	}
	
	//============================================================================================================================================================================================//
	public GameScreen GetScreen(string name)
	{
		for (int i = 0; i < Screens.Length; i++)
		{
			if (name == Screens[i].name)
			{
				return Screens[i];
			}
		}
		
		return null;
	}
	
	//============================================================================================================================================================================================//
	public void SetScreen(GameScreen screen)
	{
		print("Set Screen: " + screen.name); 
		
		if (screen != CurrentScreen)
		{
            if(CurrentScreen != null)
            {
                print("Close");
                CurrentScreen.Close();
                LastScreen = CurrentScreen;
            }

            CurrentScreen = screen;
            CurrentScreen.gameObject.SetActive(true);
            screen.Open();
		}
	}

    //============================================================================================================================================================================================//
    public void SetScreen(string name)
	{
		SetScreen(GetScreen(name));
	} 
}

	
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
