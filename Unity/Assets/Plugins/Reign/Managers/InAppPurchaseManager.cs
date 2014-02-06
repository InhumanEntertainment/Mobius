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
	public class InAppAPI
	{
		public bool IsTrial {get{return plugin.IsTrial;}}
		private IInAppPurchasePlugin plugin;
		private InAppPurchaseAPIs pluginAPI;
		private bool restoringProducts, buyingProduct;
		private InAppPurchaseRestoreCallbackMethod restoreCallback;
		private InAppPurchaseBuyCallbackMethod buyCallback;

		#if ASYNC
		private string asyncBuyAppID;
		private bool asyncRestoreDone, asyncBuyDone, asyncBuySucceeded;
		private bool[] asyncRestoreDoneItems, asyncRestoreSucceededItems;
		#endif

		public InAppAPI(InAppPurchaseDesc desc)
		{
			#if !DISABLE_REIGN
				#if UNITY_EDITOR || UNITY_METRO || UNITY_WP8
				plugin = InAppPurchaseAPI.New(desc);
				#elif UNITY_IOS
				plugin = InAppPurchaseAPI_iOS.New(desc);
				#elif UNITY_ANDROID
				plugin = InAppPurchaseAPI_Android.New(desc);
				#elif UNITY_BB10
				plugin = InAppPurchaseAPI_BB10.New(desc);
				#endif
			
				#if UNITY_METRO
				pluginAPI = desc.Win8_InAppPurchaseAPI;
				#elif UNITY_WP8
				pluginAPI = desc.WP8_InAppPurchaseAPI;
				#elif UNITY_IOS
				pluginAPI = desc.iOS_InAppPurchaseAPI;
				#elif UNITY_ANDROID
				pluginAPI = desc.Android_InAppPurchaseAPI;
				#elif UNITY_BB10
				pluginAPI = desc.BB10_InAppPurchaseAPI;
				#else
				pluginAPI = InAppPurchaseAPIs.Default;
				#endif

				#if ASYNC
				asyncRestoreDoneItems = new bool[plugin.InAppIDs.Length];
				asyncRestoreSucceededItems = new bool[plugin.InAppIDs.Length];
				#endif
			#endif
		}

		#if ASYNC
		internal void update()
		{
			plugin.Update();
		
			if (asyncRestoreDone)
			{
				restoringProducts = false;
				asyncRestoreDone = false;
				for (int i = 0; i != plugin.InAppIDs.Length; ++i)
				{
					saveBuyToPrefs(plugin.InAppIDs[i].ID, asyncRestoreSucceededItems[i]);
					if (restoreCallback != null) restoreCallback(plugin.InAppIDs[i].ID, asyncRestoreSucceededItems[i]);
				}
			}

			if (asyncBuyDone)
			{
				buyingProduct = false;
				asyncBuyDone = false;
				saveBuyToPrefs(asyncBuyAppID, asyncBuySucceeded);
				if (buyCallback != null) buyCallback(asyncBuyAppID, asyncBuySucceeded);
			}
		}

		private void async_RestoreCallback(string inAppID, bool succeeded)
		{
			// find done item
			for (int i = 0; i != plugin.InAppIDs.Length; ++i)
			{
				if (plugin.InAppIDs[i].ID == inAppID)
				{
					asyncRestoreSucceededItems[i] = succeeded;
					asyncRestoreDoneItems[i] = true;
					break;
				}
			}

			// check to see if all items are done
			foreach (var done in asyncRestoreDoneItems)
			{
				if (!done) return;
			}

			asyncRestoreDone = true;
		}

		private void async_BuyCallback(string inAppID, bool succeeded)
		{
			asyncBuyAppID = inAppID;
			asyncBuySucceeded = succeeded;
			asyncBuyDone = true;
		}
		#else
		internal void update()
		{
			plugin.Update();
		}
		
		private void noAsync_RestoreCallback(string inAppID, bool succeeded)
		{
			restoringProducts = false;
			saveBuyToPrefs(inAppID, succeeded);
			if (restoreCallback != null) restoreCallback(inAppID, succeeded);
		}
		
		private void noAsync_BuyCallback(string inAppID, bool succeeded)
		{
			buyingProduct = false;
			saveBuyToPrefs(inAppID, succeeded);
			if (buyCallback != null) buyCallback(inAppID, succeeded);
		}
		#endif

		private void saveBuyToPrefs(string inAppID, bool succeeded)
		{
			if (succeeded) PlayerPrefs.SetInt(pluginAPI + "_" + inAppID, 1);
		}

		public void ClearPlayerPrefData()
		{
			foreach (var inAppID in plugin.InAppIDs)
			{
				string key = pluginAPI + "_" + inAppID;
				if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
			}
		}

		public int GetAppIndexForAppID(string inAppID)
		{
			for (int i = 0; i != plugin.InAppIDs.Length; ++i)
			{
				if (plugin.InAppIDs[i].ID == inAppID) return i;
			}

			return -1;
		}
		
		public InAppPurchaseID GetAppID(string inAppID)
		{
			for (int i = 0; i != plugin.InAppIDs.Length; ++i)
			{
				if (plugin.InAppIDs[i].ID == inAppID) return plugin.InAppIDs[i];
			}

			return null;
		}

		public bool IsPurchased(int inAppIndex)
		{
			if (plugin.InAppIDs[inAppIndex].Type == InAppPurchaseTypes.Consumable) return false;
			return PlayerPrefs.GetInt(pluginAPI + "_" + plugin.InAppIDs[inAppIndex].ID) == 1;
		}

		public bool IsPurchased(string inAppID)
		{
			int i = GetAppIndexForAppID(inAppID);
			if (i == -1) return false;
			
			if (plugin.InAppIDs[i].Type == InAppPurchaseTypes.Consumable) return false;
			return PlayerPrefs.GetInt(pluginAPI + "_" + plugin.InAppIDs[i].ID) == 1;
		}

		public void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback)
		{
			if (restoringProducts || buyingProduct)
			{
				Debug.LogError("You must wait for the last restore or buy to finish!");
				if (restoreCallback != null) restoreCallback(null, false);
				return;
			}
			restoringProducts = true;
			this.restoreCallback = restoreCallback;
			
			#if ASYNC
			asyncRestoreDone = false;
			for (int i = 0; i != plugin.InAppIDs.Length; ++i)
			{
				asyncRestoreDoneItems[i] = false;
			}
			plugin.Restore(async_RestoreCallback);
			#elif UNITY_EDITOR
			restoringProducts = false;
			if (restoreCallback != null)
			{
				for (int i = 0; i != plugin.InAppIDs.Length; ++i)
				{
					restoreCallback(plugin.InAppIDs[i].ID, IsPurchased(i));
				}
			}
			#else
			plugin.Restore(noAsync_RestoreCallback);
			#endif
		}
		
		public void Buy(InAppPurchaseID inAppID, InAppPurchaseBuyCallbackMethod buyCallback)
		{
			Buy(inAppID.ID, buyCallback);
		}

		public void Buy(int inAppIndex, InAppPurchaseBuyCallbackMethod buyCallback)
		{
			Buy(plugin.InAppIDs[inAppIndex].ID, buyCallback);
		}

		public void Buy(string inAppID, InAppPurchaseBuyCallbackMethod buyCallback)
		{
			if (buyingProduct || restoringProducts)
			{
				Debug.LogError("You must wait for the last buy or restore to finish!");
				if (buyCallback != null) buyCallback(inAppID, false);
				return;
			}
			buyingProduct = true;
			this.buyCallback = buyCallback;

			// skip if item is already purchased locally
			if (IsPurchased(inAppID))
			{
				Debug.Log("InApp already puchased: " + inAppID);
				buyingProduct = false;
				if (buyCallback != null) buyCallback(inAppID, true);
				return;
			}
		
			#if ASYNC
			asyncBuyDone = false;
			plugin.BuyInApp(inAppID, async_BuyCallback);
			#else
			#if UNITY_EDITOR
			buyingProduct = false;
			#endif
			plugin.BuyInApp(inAppID, noAsync_BuyCallback);
			#endif
		}
	}

	public static class InAppPurchaseManager
	{
		public static InAppAPI MainInAppAPI {get{return InAppAPIs[0];}}
		public static InAppAPI[] InAppAPIs {get; private set;}

		static InAppPurchaseManager()
		{
			#if !DISABLE_REIGN
			ReignServices.AddService(update, null);
			#endif
		}

		public static InAppAPI Init(InAppPurchaseDesc desc)
		{
			InAppAPIs = new InAppAPI[1];
			InAppAPIs[0] = new InAppAPI(desc);
			return InAppAPIs[0];
		}

		public static InAppAPI[] Init(InAppPurchaseDesc[] descs)
		{
			var inAppAPIs = new InAppAPI[descs.Length];
			for (int i = 0; i != descs.Length; ++i)
			{
				inAppAPIs[i] = new InAppAPI(descs[i]);
			}

			return inAppAPIs;
		}

		private static void update()
		{
			if (InAppAPIs == null) return;

			foreach (var app in InAppAPIs)
			{
				app.update();
			}
		}
	}
}