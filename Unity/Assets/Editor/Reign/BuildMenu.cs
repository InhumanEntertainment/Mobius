// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using UnityEditor;
using Reign;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

public class BuildMenu
{
	[MenuItem("Edit/Reign/Tools/Download WP8 Patch")]
	static void DownloadWP8Patch()
	{
		EditorUtility.DisplayDialog("WP8 Patch", "Get the WP8 patch to fix Mono.Cecil build errors.\n\nGo to \"http://www.reign-studios.net/products/win8-wp8/\"", "Ok");
	}

	private static string convertPathToPlatform(string path)
	{
		#if UNITY_EDITOR_WIN
		return path.Replace('/', '\\');
		#else
		return path.Replace('\\', '/');
		#endif
	}
	
	private static void disableReignForPlatform(BuildTargetGroup platform)
	{
		string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
		if (string.IsNullOrEmpty(valueBlock))
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, "DISABLE_REIGN");
		}
		else
		{
			string newValue = "";
			var values = valueBlock.Split(';', ' ');
			foreach (var value in values)
			{
				newValue += value + ';';
			}
			
			newValue += "DISABLE_REIGN";
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
		}
	}
	
	private static void enableReignForPlatform(BuildTargetGroup platform)
	{
		string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
		if (!string.IsNullOrEmpty(valueBlock))
		{
			string newValue = "";
			var values = valueBlock.Split(';', ' ');
			foreach (var value in values)
			{
				if (value != "DISABLE_REIGN") newValue += value + ';';
			}
			
			if (newValue.Length != 0 && newValue[newValue.Length-1] == ';') newValue = newValue.Substring(0, newValue.Length-1);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
		}
	}
	
	private static BuildTargetGroup convertBuildTarget(BuildTarget target)
	{
		switch (target)
		{
			case BuildTarget.Android: return BuildTargetGroup.Android;
			case BuildTarget.BB10: return BuildTargetGroup.BB10;
			case BuildTarget.FlashPlayer: return BuildTargetGroup.FlashPlayer;
			case BuildTarget.iPhone: return BuildTargetGroup.iPhone;
			case BuildTarget.MetroPlayer: return BuildTargetGroup.Metro;
			case BuildTarget.NaCl: return BuildTargetGroup.NaCl;
			case BuildTarget.PS3: return BuildTargetGroup.PS3;
			case BuildTarget.WebPlayer: return BuildTargetGroup.WebPlayer;
			case BuildTarget.Wii: return BuildTargetGroup.Wii;
			case BuildTarget.WP8Player: return BuildTargetGroup.WP8;
			case BuildTarget.XBOX360: return BuildTargetGroup.XBOX360;
			
			case BuildTarget.StandaloneLinux: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneLinux64: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneLinuxUniversal: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneOSXIntel: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneOSXIntel64: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneOSXUniversal: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneWindows: return BuildTargetGroup.Standalone;
			case BuildTarget.StandaloneWindows64: return BuildTargetGroup.Standalone;
			
			default: throw new System.Exception("Unknown BuildTarget: " + target);
		}
	}
	
	[MenuItem("Edit/Reign/Set Platform/Disable (Disables Reign for current platform)")]
	static void DisableReign()
	{
		disableReignForPlatform(convertBuildTarget(EditorUserBuildSettings.activeBuildTarget));
		
		setWin8(false);
		setWP8(false);
		setIOS(false);
		setAndroid(false);
		setBB10(false);
		finish("Disable");
	}
	
	[MenuItem("Edit/Reign/Set Platform/Enable (Enables Reign for current platform)")]
	static void EnableReign()
	{
		enableReignForPlatform(convertBuildTarget(EditorUserBuildSettings.activeBuildTarget));
		
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.MetroPlayer) SetPlatformWin8();
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WP8Player) SetPlatformWP8();
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone) SetPlatformIOS();
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) SetPlatformAndroid();
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.BB10) SetPlatformBB10();
		finish("Enable");
	}

	[MenuItem("Edit/Reign/Set Platform/Revert (Do before committing to version control)")]
	static void SetPlatformRevert()
	{
		setWin8(true);
		setWP8(true);
		setIOS(true);
		setAndroid(true);
		setBB10(true);
		finish("Revert");
	}

	[MenuItem("Edit/Reign/Set Platform/Win8")]
	static void SetPlatformWin8()
	{
		setWin8(true);
		setWP8(false);
		setIOS(false);
		setAndroid(false);
		setBB10(false);
		finish("Win8");
	}

	[MenuItem("Edit/Reign/Set Platform/WP8")]
	static void SetPlatformWP8()
	{
		setWin8(true);
		setWP8(true);
		setIOS(false);
		setAndroid(false);
		setBB10(false);
		finish("WP8");
	}

	[MenuItem("Edit/Reign/Set Platform/iOS")]
	static void SetPlatformIOS()
	{
		setWin8(false);
		setWP8(false);
		setIOS(true);
		setAndroid(false);
		setBB10(false);
		finish("iOS");
	}
	
	[MenuItem("Edit/Reign/Set Platform/Android")]
	static void SetPlatformAndroid()
	{
		setWin8(false);
		setWP8(false);
		setIOS(false);
		setAndroid(true);
		setBB10(false);
		finish("Android");
	}

	[MenuItem("Edit/Reign/Set Platform/BB10")]
	static void SetPlatformBB10()
	{
		setWin8(false);
		setWP8(false);
		setIOS(false);
		setAndroid(false);
		setBB10(true);
		finish("BB10");
	}

	private static void finish(string platform)
	{
		AssetDatabase.Refresh();
		EditorUtility.DisplayDialog("Set to " + platform, "NOTE: Changed some Reign libs from .dll to dllx or vise-versa.\nThis is done to remove uneeded or conflicting libs in the build process.", "OK");
	}

	private static void setWin8(bool enable)
	{
		// do nothing...
	}

	private static void setWP8(bool enable)
	{
		// do nothing...
	}

	private static void setIOS(bool enable)
	{
		setDll(enable, "/IOS/Reign.iOS");
	}
	
	private static void setAndroid(bool enable)
	{
		setDll(enable, "/Android/Reign.Android");
	}
	
	private static void setBB10(bool enable)
	{
		setDll(enable, "/BlackBerry/Reign.BB10");
	}

	private static void setDll(bool enable, string dllName)
	{
		if (enable)
		{
			string dllFile = convertPathToPlatform(Application.dataPath + "/Plugins" + dllName + ".dllx");
			if (File.Exists(dllFile))
			{
				File.Move(dllFile, Path.ChangeExtension(dllFile, ".dll"));
				if (File.Exists(dllFile + ".meta")) File.Delete(dllFile + ".meta");
			}
		}
		else
		{
			string dllFile = convertPathToPlatform(Application.dataPath + "/Plugins" + dllName + ".dll");
			if (File.Exists(dllFile))
			{
				File.Move(dllFile, Path.ChangeExtension(dllFile, ".dllx"));
				if (File.Exists(dllFile + ".meta")) File.Delete(dllFile + ".meta");
			}
		}
	}
}