// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using Reign.Plugin;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Reign
{
	public static class MessageBoxManager
	{
		private static IMessageBoxPlugin plugin;

		static MessageBoxManager()
		{
			#if !DISABLE_REIGN
				#if UNITY_EDITOR || UNITY_METRO || UNITY_WP8
				plugin = new MessageBoxPlugin();
				#elif UNITY_IOS
				plugin = new MessageBoxPlugin_iOS();
				#elif UNITY_ANDROID
				plugin = new MessageBoxPlugin_Android();
				#elif UNITY_BB10
				plugin = new MessageBoxPlugin_BB10();
				#endif
			#endif
		}

		public static void Show(string title, string message)
		{
			#if UNITY_EDITOR
			EditorUtility.DisplayDialog(title, message, "OK");
			#else
			plugin.Show(title, message);
			#endif
		}
	}
}