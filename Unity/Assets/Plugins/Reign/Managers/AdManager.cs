// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

//#define TEST_ASYNC
#if ((UNITY_METRO || UNITY_WP8) && !UNITY_EDITOR) || TEST_ASYNC
#define ASYNC
#endif

using System;
using UnityEngine;
using System.Collections.Generic;
using Reign.Plugin;

namespace Reign
{
    public static class AdManager
    {
		private static List<IAdPlugin> plugins;
		private static bool creatingAds;
		private static AdCreatedCallbackMethod createdCallback;

		#if ASYNC
		private static bool asyncDone, asyncSucceeded;
		#endif

		static AdManager()
		{
			plugins = new List<IAdPlugin>();

			#if !DISABLE_REIGN
			ReignServices.AddService(update, null);
			#endif
		}

		#if ASYNC
		private static void update()
		{
			foreach (var plugin in plugins)
			{
				plugin.Update();
			}
		
			if (asyncDone)
			{
				creatingAds = false;
				asyncDone = false;
				if (createdCallback != null) createdCallback(asyncSucceeded);
			}
		}

		private static void async_CreatedCallback(bool succeeded)
		{
			asyncSucceeded = succeeded;
			asyncDone = true;
		}
		#else
		private static void update()
		{
			foreach (var plugin in plugins)
			{
				plugin.Update();
			}
		}
		
		private static void noAsync_CreatedCallback(bool succeeded)
		{
			creatingAds = false;
			if (createdCallback != null) createdCallback(succeeded);
		}
		#endif

		public static void CreateAd(AdDesc desc, AdCreatedCallbackMethod createdCallback)
		{
			if (creatingAds)
			{
				Debug.LogError("You must wait for the last ad to finish being created!");
				if (createdCallback != null) createdCallback(false);
				return;
			}
			creatingAds = true;
			AdManager.createdCallback = createdCallback;

			#if !DISABLE_REIGN
				#if ASYNC
				asyncDone = false;
				plugins.Add(AdPluginAPI.New(desc, async_CreatedCallback));
				#else
					#if UNITY_EDITOR
					creatingAds = false;
					plugins.Add(AdPluginAPI.New(desc, createdCallback));
					#elif UNITY_IOS
					plugins.Add(AdPluginAPI_iOS.New(desc, noAsync_CreatedCallback));
					#elif UNITY_ANDROID
					plugins.Add(AdPluginAPI_Android.New(desc, noAsync_CreatedCallback));
					#elif UNITY_BB10
					plugins.Add(AdPluginAPI_BB10.New(desc, noAsync_CreatedCallback));
					#endif
				#endif
			#endif
		}

		public static void CreateAd(AdDesc[] descs, AdCreatedCallbackMethod createdCallback)
		{
			if (creatingAds)
			{
				Debug.LogError("You must wait for the last ads to finish being created!");
				if (createdCallback != null) createdCallback(false);
				return;
			}
			creatingAds = true;
			AdManager.createdCallback = createdCallback;

			#if !DISABLE_REIGN
				#if ASYNC
				asyncDone = false;
				for (int i = 0; i != descs.Length; ++i) plugins.Add(AdPluginAPI.New(descs[i], async_CreatedCallback));
				#else
					#if UNITY_EDITOR
					creatingAds = false;
					for (int i = 0; i != descs.Length; ++i) plugins.Add(AdPluginAPI.New(descs[i], createdCallback));
					#elif UNIT_IOS
					for (int i = 0; i != descs.Length; ++i) plugins.Add(AdPluginAPI_iOS.New(descs[i], noAsync_CreatedCallback));
					#elif UNIT_ANDROID
					for (int i = 0; i != descs.Length; ++i) plugins.Add(AdPluginAPI_Android.New(descs[i], noAsync_CreatedCallback));
					#elif UNIT_BB10
					for (int i = 0; i != descs.Length; ++i) plugins.Add(AdPluginAPI_BB10.New(descs[i], noAsync_CreatedCallback));
					#endif
				#endif
			#endif
		}

		public static void DisposeAd(int adIndex)
		{
			var plugin = plugins[adIndex];
			plugin.Dispose();
			plugins.Remove(plugin);
		}
    }
}
