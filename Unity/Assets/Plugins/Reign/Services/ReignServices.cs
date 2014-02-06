// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ReignServices : MonoBehaviour
{
	public static ReignServices Singleton {get; private set;}
	internal bool requestFrameDone;
	internal delegate void frameDoneCallbackMethod();
	internal frameDoneCallbackMethod frameDoneCallback;

	public delegate void ServiceMethod();
	private static event ServiceMethod updateService, destroyService, onguiService;

	public static void AddService(ServiceMethod update, ServiceMethod destroy)
	{
		if (update != null)
		{
			updateService -= update;
			updateService += update;
		}

		if (destroy != null)
		{
			destroyService -= destroy;
			destroyService += destroy;
		}
	}

	public static void RemoveService(ServiceMethod update, ServiceMethod destroy)
	{
		if (update != null) updateService -= update;
		if (destroy != null) destroyService -= destroy;
	}

	public static void AddOnGUIService(ServiceMethod onGUI)
	{
		if (onGUI != null)
		{
			onguiService -= onGUI;
			onguiService += onGUI;
		}
	}

	public static void RemoveOnGUIService(ServiceMethod onGUI)
	{
		if (onGUI != null) onguiService -= onGUI;
	}

	void Awake()
	{
		if (Singleton != null)
		{
			Destroy(gameObject);
			return;
		}

		Singleton = this;
		Reign.Logs.Debug.Log += Debug.Log;
		Reign.Logs.Debug.LogError += Debug.LogError;
		dispose();
		DontDestroyOnLoad(gameObject);
	}

	void OnDestroy()
	{
		if (destroyService != null) destroyService();
		dispose();
	}

	private void dispose()
	{
		updateService = null;
		destroyService = null;
	}

	void Update()
	{
		if (updateService != null) updateService();

		if (requestFrameDone)
		{
			requestFrameDone = false;
			StartCoroutine(frameSync());
		}
	}

	private IEnumerator frameSync()
	{
		yield return new WaitForEndOfFrame();
		if (frameDoneCallback != null) frameDoneCallback();
	}

	void OnGUI()
	{
		if (onguiService != null) onguiService(); 
	}
}
