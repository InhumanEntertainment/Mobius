// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using UnityEditor;
using Reign;

public class ClearPlayerPrefs
{
	[MenuItem("Edit/Reign/Clear All PlayerPrefs")]
	static void InitClearAll()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("Edit/Reign/Reset Editor InApps Prefs (While game is running)")]
	static void InitClearInApps()
	{
		if (InAppPurchaseManager.InAppAPIs == null) return;

		foreach (var api in InAppPurchaseManager.InAppAPIs)
		{
			api.ClearPlayerPrefData();
		}
	}
}