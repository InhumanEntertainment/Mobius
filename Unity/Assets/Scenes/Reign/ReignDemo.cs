// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.IO;
using System;
using Reign;

// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/
// -----------------------------------------------

public class ReignDemo : MonoBehaviour
{
	// demo images
	public Texture2D ReignLogo, ReignLogo2;
	private Texture2D currentImage;

	// status helper fields
	private string adStatus;

	private bool buyInAppStatus, restoreInAppStatus;
	private string[] restoreInAppStatusText;

	private bool saveDataFileStatus, loadDataFileStatus;
	private string saveDataFileStatusText, loadDataFileStatusText;

	private bool saveImageStatus, loadImageStatus;
	
	void Start()
	{
		currentImage = ReignLogo;

		// Ads - NOTE: You can pass in multiple "AdDesc" objects if you want more then one ad.
		var adDesc = new AdDesc()
		{
			// global settings
			Testing = true,// NOTE: To test ads on iOS, you must enable them in iTunes Connect.
			Visible = true,
			EventCallback = adEvent,

			// Win8 settings
			Win8_MicrosoftAdvertising_ApplicationID = "",// Non Testing ApplicationID
			Win8_MicrosoftAdvertising_UnitID = "",// Non Testing UnityID
			Win8_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter,
			Win8_MicrosoftAdvertising_AdSize = Win8_MicrosoftAdvertising_AdSize.Wide_728x90,
			// WP8 settings
			WP8_MicrosoftAdvertising_ApplicationID = "",// Non Testing ApplicationID
			WP8_MicrosoftAdvertising_UnitID = "",// Non Testing UnityID
			WP8_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter,
			WP8_MicrosoftAdvertising_AdSize = WP8_MicrosoftAdvertising_AdSize.Wide_480x80,
			// BB10 settings
			BB10_BlackBerryAdvertising_ZoneID = "",// Non Testing ZoneID
			BB10_BlackBerryAdvertising_AdGravity = AdGravity.BottomCenter,
			BB10_BlackBerryAdvertising_AdSize = BB10_BlackBerryAdvertising_AdSize.Wide_320x50,
			// iOS settings
			iOS_MicrosoftAdvertising_AdGravity = AdGravity.BottomCenter,
			// Android settings
			Android_AdAPI = AdAPIs.AdMob,// Choose between AdMob or AmazonAds
			Android_AdMob_UnitID = "",// NOTE: Or legacy (PublisherID), You MUST have this even for Testing!
			Android_AdMob_AdGravity = AdGravity.BottomCenter,
		};
		AdManager.CreateAd(adDesc, adCreatedCallback);

		// InApp-Purchases - NOTE: you can set different "In App IDs" for each platform.
		var inAppIDs = new InAppPurchaseID[]
		{
			new InAppPurchaseID("com.reignstudios.test_app1", InAppPurchaseTypes.NonConsumable),
			new InAppPurchaseID("com.reignstudios.test_app2", InAppPurchaseTypes.NonConsumable),
			new InAppPurchaseID("com.reignstudios.test_app3", InAppPurchaseTypes.Consumable)
		};
		restoreInAppStatusText = new string[inAppIDs.Length];
		
		var appDesc = new InAppPurchaseDesc()
		{
			Testing = true,
			Editor_InAppIDs = inAppIDs,
			Win8_MicrosoftStore_InAppIDs = inAppIDs,
			WP8_MicrosoftStore_InAppIDs = inAppIDs,
			BB10_BlackBerryWorld_InAppIDs = inAppIDs,
			iOS_AppleStore_InAppIDs = inAppIDs,
			
			Android_InAppPurchaseAPI = InAppPurchaseAPIs.GooglePlay,// Choose for either GooglePlay or Amazon
			Android_GooglePlay_InAppIDs = inAppIDs,
			Android_GooglePlay_Base64Key = "",// NOTE: Must set Base64Key for GooglePlay in Apps to work, even for Testing!
			Android_Amazon_InAppIDs = inAppIDs
		};
		InAppPurchaseManager.Init(appDesc);

		// System Events
		SystemEvents.Win8Event += SystemEvents_Win8Event;// Handle Win8 Snapping
	}
	
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}

	void OnGUI()
	{
		GUI.matrix = Matrix4x4.identity;
		GUI.color = Color.white;

		// Is Trial Mode - NOTE: Not all platforms have this concept.
		//if (InAppPurchaseManager.MainInAppAPI.IsTrial) ;// Do Something...
		
		// ui scale
		float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;

		// draw logo
		GUI.DrawTexture(new Rect((Screen.width/2)-(64*scale), 64*scale, 128*scale, 128*scale), currentImage);
		if (GUI.Button(new Rect((Screen.width/2)-(64), 0, 128, 64*scale), "Clear PlayerPrefs")) PlayerPrefs.DeleteAll();

		// status
		GUI.Label(new Rect(0, 0, 256, 64*scale), "Ad Status: " + adStatus);

		// InApp-Purchases
		if (!buyInAppStatus && GUI.Button(new Rect(Screen.width-(128), 0, 128, 64*scale), "Buy NonConsumable"))
		{
			buyInAppStatus = true;
			// NOTE: You can pass in a "InAppID string value" but we recommend passing in an "index" value as its a cross platform approach -
			// - if you use the same in app types for each platform.
			InAppPurchaseManager.MainInAppAPI.Buy(0, buyAppCallback);
		}
		
		if (!buyInAppStatus && GUI.Button(new Rect(Screen.width-(128), 64*scale, 128, 64*scale), "Buy Consumable"))
		{
			buyInAppStatus = true;
			// NOTE: You can pass in a "InAppID string value" but we recommend passing in an "index" value as its a cross platform approach -
			// - if you use the same in app types for each platform.
			InAppPurchaseManager.MainInAppAPI.Buy(2, buyAppCallback);
		}

		if (!restoreInAppStatus && GUI.Button(new Rect(Screen.width-(128), 128*scale, 128, 64*scale), "Restore Apps"))
		{
			restoreInAppStatus = true;
			InAppPurchaseManager.MainInAppAPI.Restore(restoreAppsCallback);
		}
		else
		{
			for (int i = 0; i != restoreInAppStatusText.Length; ++i)
			{
				GUI.Label(new Rect(Screen.width-(128), (192*scale)+(64*i), 128, 64), restoreInAppStatusText[i]);
			}
		}

		// Local data files
		if (!saveDataFileStatus && !loadDataFileStatus && GUI.Button(new Rect(0, 64*scale, 128, 64*scale), "Save Data file"))
		{
			saveDataFileStatusText = "Saving Data...";
			saveDataFileStatus = true;
			var data = new byte[1] {123};
			StreamManager.SaveFile("MyFile.data", data, FolderLocations.Storage, dataFileSavedCallback);
		}
		else
		{
			GUI.Label(new Rect(132, 64*scale, 256, 64*scale), saveDataFileStatusText);
		}

		if (!saveDataFileStatus && !loadDataFileStatus && GUI.Button(new Rect(0, 128*scale, 128, 64*scale), "Load Data file"))
		{
			loadDataFileStatusText = "Loading Data...";
			loadDataFileStatus = true;
			StreamManager.LoadFile("MyFile.data", FolderLocations.Storage, dataFileLoadedCallback);
		}
		else
		{
			GUI.Label(new Rect(132, 128*scale, 256, 64*scale), loadDataFileStatusText);
		}

		// save and load images
		if (!saveImageStatus && !loadImageStatus && GUI.Button(new Rect(0, 200*scale, 128, 64*scale), "Save Reign Logo"))
		{
			saveImageStatus = true;
			var data = ReignLogo.EncodeToPNG();
			StreamManager.SaveFile("TEST.png", data, FolderLocations.Pictures, imageSavedCallback);
		}

		if (!saveImageStatus && !loadImageStatus && GUI.Button(new Rect(0, (200+64)*scale, 128, 64*scale), "Load Reign Logo"))
		{
			// NOTE: LoadFile doesn't support the Pictures folder on iOS.
			loadImageStatus = true;
			StreamManager.LoadFile("TEST.png", FolderLocations.Pictures, imageLoadedCallback);
		}

		if (!saveImageStatus && !loadImageStatus && GUI.Button(new Rect(0, 332*scale, 128, 64*scale), "Save Image Picker"))
		{
			// NOTE: SaveFileDialog not supported on iOS.
			saveImageStatus = true;
			var data = ReignLogo.EncodeToPNG();
			StreamManager.SaveFileDialog(data, FolderLocations.Pictures, new string[]{".png"}, imageSavedCallback);
		}

		if (!saveImageStatus && !loadImageStatus && GUI.Button(new Rect(0, (332+64)*scale, 128, 64*scale), "Image Picker"))
		{
			loadImageStatus = true;
			StreamManager.LoadFileDialog(FolderLocations.Pictures, new string[]{".png", ".jpg"}, imageLoadedCallback);// unity only supports loading png and jpg data
		}

		// send email
		if (GUI.Button(new Rect((Screen.width/2)-(64), (Screen.height/2)-(32*scale), 128, 64*scale), "Send Email"))
		{
			EmailManager.Send("support@reign-studios.com", "ReignDemo", "Some content...");
		}
	}

	// -----------------------------------------------
	// Ad Callbacks
	// -----------------------------------------------
	private void adCreatedCallback(bool succeeded)
	{
		adStatus = succeeded ? "Ads Succeded" : "Ads Failed";
	}

	private void adEvent(AdEvents adEvent, string eventMessage)
	{
		// NOTE: On BB10 these events never get called!
		switch (adEvent)
		{
			case AdEvents.Refreshed: adStatus = "Refreshed"; break;
			// NOTE: On Win8 there seems to be a bug with test ads not firing the click event
			// NOTE: On Android with AmazonAds there seems to be a bug with test ads not firing the click event
			case AdEvents.Clicked: adStatus = "Clicked"; break;
			case AdEvents.Error: adStatus = "Error: " + eventMessage; break;
		}
	}

	// -----------------------------------------------
	// InApp-Purchase Callbacks
	// -----------------------------------------------
	void buyAppCallback(string inAppID, bool succeeded)
	{
		buyInAppStatus = false;
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		MessageBoxManager.Show("App Buy Status", inAppID + " Success: " + succeeded + " Index: " + appIndex);
		if (appIndex != -1) restoreInAppStatusText[appIndex] = "Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
	}

	void restoreAppsCallback(string inAppID, bool succeeded)
	{
		restoreInAppStatus = false;
		int appIndex = InAppPurchaseManager.MainInAppAPI.GetAppIndexForAppID(inAppID);
		if (appIndex != -1)
		{
			restoreInAppStatusText[appIndex] = "Restore Status: " + inAppID + ": " + succeeded + " Index: " + appIndex;
			Debug.Log(restoreInAppStatusText[appIndex]);
		}
	}

	// -----------------------------------------------
	// Stream Callbacks
	// -----------------------------------------------
	private void dataFileSavedCallback(bool succeeded)
	{
		saveDataFileStatus = false;
		saveDataFileStatusText = "Data Saved Status: " + succeeded;
	}

	private void dataFileLoadedCallback(Stream stream, bool succeeded)
	{
		try
		{
			loadDataFileStatus = false;
			loadDataFileStatusText = "Data Loaded Status: " + succeeded;
			if (succeeded) loadDataFileStatusText += " Data: " + stream.ReadByte();
		}
		catch (Exception e)
		{
			MessageBoxManager.Show("Error", e.Message);
		}

		if (stream != null) stream.Dispose();// NOTE: Make sure you dispose of this stream !!!
	}

	private void imageSavedCallback(bool succeeded)
	{
		saveImageStatus = false;
		MessageBoxManager.Show("Image Status", "Image Saved: " + succeeded);
		if (succeeded) currentImage = ReignLogo2;
	}

	private void imageLoadedCallback(Stream stream, bool succeeded)
	{
		loadImageStatus = false;
		MessageBoxManager.Show("Image Status", "Image Loaded: " + succeeded);
		if (!succeeded) return;
		
		try
		{
			var data = new byte[stream.Length];
			stream.Read(data, 0, data.Length);
			currentImage = new Texture2D(4, 4);
			currentImage.LoadImage(data);
		}
		catch (Exception e)
		{
			MessageBoxManager.Show("Error", e.Message);
		}

		if (stream != null) stream.Dispose();// NOTE: Make sure you dispose of this stream !!!
	}

	// -----------------------------------------------
	// SystemEvent Callbacks
	// -----------------------------------------------
	void SystemEvents_Win8Event(Win8EventTypes type)
	{
		Debug.Log(type);
	}
}
