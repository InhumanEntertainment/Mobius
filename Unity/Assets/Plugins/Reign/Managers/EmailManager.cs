// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using Reign.Plugin;

namespace Reign
{
	public static class EmailManager
	{
		private static IEmailPlugin plugin;

		static EmailManager()
		{
			#if !DISABLE_REIGN
				#if UNITY_EDITOR || UNITY_METRO || UNITY_WP8
				plugin = new EmailPlugin();
				#elif UNITY_IOS
				plugin = new EmailPlugin_iOS();
				#elif UNITY_ANDROID
				plugin = new EmailPlugin_Android();
				#elif UNITY_BB10
				plugin = new EmailPlugin_BB10();
				#endif
			#endif
		}

		public static void Send(string to, string subject, string body)
		{
			plugin.Send(to, subject, body);
		}
	}
}