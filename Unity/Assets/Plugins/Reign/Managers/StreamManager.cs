// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

//#define TEST_ASYNC
#if ((UNITY_METRO || UNITY_WP8) && !UNITY_EDITOR) || TEST_ASYNC
#define ASYNC
#endif

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Reign.Plugin;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Reign
{
    public static class StreamManager
    {
		private static IStreamPlugin plugin;
		private static bool savingStream, loadingStream, checkingIfFileExists, deletingFile;
		private static StreamExistsCallbackMethod streamExistsCallback;
		private static StreamDeleteCallbackMethod streamDeleteCallback;
		private static StreamSavedCallbackMethod streamSavedCallback;
		private static StreamLoadedCallbackMethod streamLoadedCallback;

		#if ASYNC
		private static bool asyncExistsDone, asyncDeleteDone, asyncExistsSucceeded, asyncDeleteSucceeded;
		private static bool asyncSaveDone, asyncLoadDone, asyncSaveSucceeded, asyncLoadSucceeded;
		private static Stream asyncLoadStream;
		#endif

		static StreamManager()
		{
			#if !DISABLE_REIGN
				#if UNITY_EDITOR || UNITY_METRO || UNITY_WP8
				plugin = new StreamPlugin();
				#elif UNITY_IOS
				plugin = new StreamPlugin_iOS();
				#elif UNITY_ANDROID
				plugin = new StreamPlugin_Android();
				#elif UNITY_BB10
				plugin = new StreamPlugin_BB10();
				#endif
				
				ReignServices.AddService(update, null);
			#endif
		}

		#if ASYNC
		private static void update()
		{
			plugin.Update();

			if (asyncExistsDone)
			{
				checkingIfFileExists = false;
				asyncExistsDone = false;
				if (streamExistsCallback != null) streamExistsCallback(asyncExistsSucceeded);
			}

			if (asyncDeleteDone)
			{
				deletingFile = false;
				asyncDeleteDone = false;
				if (streamDeleteCallback != null) streamDeleteCallback(asyncDeleteSucceeded);
			}
		
			if (asyncSaveDone)
			{
				savingStream = false;
				asyncSaveDone = false;
				if (streamSavedCallback != null) streamSavedCallback(asyncSaveSucceeded);
			}

			if (asyncLoadDone)
			{
				loadingStream = false;
				asyncLoadDone = false;
				var stream = asyncLoadStream;
				asyncLoadStream = null;
				if (streamLoadedCallback != null) streamLoadedCallback(stream, asyncLoadSucceeded);
			}
		}

		private static void async_streamExistsCallback(bool exists)
		{
			asyncExistsSucceeded = exists;
			asyncExistsDone = true;
		}

		private static void async_streamDeleteCallback(bool succeeded)
		{
			asyncDeleteSucceeded = succeeded;
			asyncDeleteDone = true;
		}

		private static void async_streamSavedCallback(bool succeeded)
		{
			asyncSaveSucceeded = succeeded;
			asyncSaveDone = true;
		}

		private static void async_streamLoadedCallback(Stream stream, bool succeeded)
		{
			asyncLoadStream = stream;
			asyncLoadSucceeded = succeeded;
			asyncLoadDone = true;
		}
		#else
		private static void update()
		{
			plugin.Update();
		}

		private static void noAsync_streamExistsCallback(bool exists)
		{
			checkingIfFileExists = false;
			if (streamExistsCallback != null) streamExistsCallback(exists);
		}

		private static void noAsync_streamDeleteCallback(bool succeeded)
		{
			deletingFile = false;
			if (streamDeleteCallback != null) streamDeleteCallback(succeeded);
		}
		
		private static void noAsync_streamSavedCallback(bool succeeded)
		{
			savingStream = false;
			if (streamSavedCallback != null) streamSavedCallback(succeeded);
		}
		
		private static void noAsync_streamLoadedCallback(Stream stream, bool succeeded)
		{
			loadingStream = false;
			if (streamLoadedCallback != null) streamLoadedCallback(stream, succeeded);
		}
		#endif
		
		private static string getCorrectUnityPath(string fileName, FolderLocations folderLocation)
		{
			if (folderLocation == FolderLocations.Storage) return ConvertToPlatformSlash(Application.persistentDataPath + "/" + fileName);
			else return ConvertToPlatformSlash(fileName);
		}

		public static void SaveScreenShotToPictures(StreamSavedCallbackMethod streamSavedCallback)
		{
			StreamManager.streamSavedCallback = streamSavedCallback;
			ReignServices.Singleton.frameDoneCallback = saveScreenShotFrameDone;
			ReignServices.Singleton.requestFrameDone = true;
		}

		private static void saveScreenShotFrameDone()
		{
			var width = Screen.width;
			var height = Screen.height;
			var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
			texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			texture.Apply();
			var data = texture.EncodeToPNG();
			GameObject.Destroy(texture);
			SaveFile("ScreenShot.png", data, FolderLocations.Pictures, streamSavedCallback);
		}

		public static void FileExists(string fileName, FolderLocations folderLocation, StreamExistsCallbackMethod streamExistsCallback)
		{
			if (checkingIfFileExists)
			{
				Debug.LogError("You must wait for the last file check to finish!");
				if (streamExistsCallback != null) streamExistsCallback(false);
				return;
			}
			checkingIfFileExists = true;
			StreamManager.streamExistsCallback = streamExistsCallback;
		
			#if ASYNC
			asyncExistsDone = false;
			plugin.FileExists(fileName, folderLocation, async_streamExistsCallback);
			#else
			plugin.FileExists(getCorrectUnityPath(fileName, folderLocation), folderLocation, noAsync_streamExistsCallback);
			#endif
		}

		public static void DeleteFile(string fileName, FolderLocations folderLocation, StreamDeleteCallbackMethod streamDeleteCallback)
		{
			if (checkingIfFileExists)
			{
				Debug.LogError("You must wait for the last file delete to finish!");
				if (streamDeleteCallback != null) streamDeleteCallback(false);
				return;
			}
			deletingFile = true;
			StreamManager.streamDeleteCallback = streamDeleteCallback;
		
			#if ASYNC
			asyncDeleteDone = false;
			plugin.DeleteFile(fileName, folderLocation, async_streamDeleteCallback);
			#else
			plugin.DeleteFile(getCorrectUnityPath(fileName, folderLocation), folderLocation, noAsync_streamDeleteCallback);
			#endif
		}

		public static void SaveFile(string fileName, Stream stream, FolderLocations folderLocation, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				Debug.LogError("You must wait for the last saved file to finish!");
				if (streamSavedCallback != null) streamSavedCallback(false);
				return;
			}
			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFile(fileName, stream, folderLocation, async_streamSavedCallback);
			#else
			plugin.SaveFile(getCorrectUnityPath(fileName, folderLocation), stream, folderLocation, noAsync_streamSavedCallback);
			#endif
		}

		public static void SaveFile(string fileName, byte[] data, FolderLocations folderLocation, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				Debug.LogError("You must wait for the last saved file to finish!");
				if (streamSavedCallback != null) streamSavedCallback(false);
				return;
			}
			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFile(fileName, data, folderLocation, async_streamSavedCallback);
			#else
			plugin.SaveFile(getCorrectUnityPath(fileName, folderLocation), data, folderLocation, noAsync_streamSavedCallback);
			#endif
		}

		public static void LoadFile(string fileName, FolderLocations folderLocation, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (loadingStream)
			{
				Debug.LogError("You must wait for the last loaded file to finish!");
				if (streamLoadedCallback != null) streamLoadedCallback(null, false);
				return;
			}
			loadingStream = true;
			StreamManager.streamLoadedCallback = streamLoadedCallback;
		
			#if ASYNC
			asyncLoadDone = false;
			plugin.LoadFile(fileName, folderLocation, async_streamLoadedCallback);
			#else
			plugin.LoadFile(getCorrectUnityPath(fileName, folderLocation), folderLocation, noAsync_streamLoadedCallback);
			#endif
		}

		public static void SaveFileDialog(Stream stream, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				Debug.LogError("You must wait for the last saved file to finish!");
				if (streamSavedCallback != null) streamSavedCallback(false);
				return;
			}
			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFileDialog(stream, folderLocation, fileTypes, async_streamSavedCallback);
			#elif UNITY_EDITOR
			savingStream = false;
			string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", "png");
			if (!string.IsNullOrEmpty(fileName))
			{
				var data = new byte[stream.Length];
				stream.Position = 0;
				stream.Read(data, 0, data.Length);
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					file.Write(data, 0, data.Length);
				}
				if (streamSavedCallback != null) streamSavedCallback(true);
			}
			else
			{
				if (streamSavedCallback != null) streamSavedCallback(false);
			}
			#else
			plugin.SaveFileDialog(stream, folderLocation, fileTypes, noAsync_streamSavedCallback);
			#endif
		}

		public static void SaveFileDialog(byte[] data, FolderLocations folderLocation, string[] fileTypes, StreamSavedCallbackMethod streamSavedCallback)
		{
			if (savingStream)
			{
				Debug.LogError("You must wait for the last saved file to finish!");
				if (streamSavedCallback != null) streamSavedCallback(false);
				return;
			}
			savingStream = true;
			StreamManager.streamSavedCallback = streamSavedCallback;
		
			#if ASYNC
			asyncSaveDone = false;
			plugin.SaveFileDialog(data, folderLocation, fileTypes, async_streamSavedCallback);
			#elif UNITY_EDITOR
			savingStream = false;
			string fileName = EditorUtility.SaveFilePanel("Save file", "", "FileName", "png");
			if (!string.IsNullOrEmpty(fileName))
			{
				using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					file.Write(data, 0, data.Length);
				}
				if (streamSavedCallback != null) streamSavedCallback(true);
			}
			else
			{
				if (streamSavedCallback != null) streamSavedCallback(false);
			}
			#else
			plugin.SaveFileDialog(data, folderLocation, fileTypes, noAsync_streamSavedCallback);
			#endif
		}

		public static void LoadFileDialog(FolderLocations folderLocation, string[] fileTypes, StreamLoadedCallbackMethod streamLoadedCallback)
		{
			if (loadingStream)
			{
				Debug.LogError("You must wait for the last loaded file to finish!");
				if (streamLoadedCallback != null) streamLoadedCallback(null, false);
				return;
			}
			loadingStream = true;
			StreamManager.streamLoadedCallback = streamLoadedCallback;
		
			#if ASYNC
			asyncLoadDone = false;
			plugin.LoadFileDialog(folderLocation, fileTypes, async_streamLoadedCallback);
			#elif UNITY_EDITOR
			loadingStream = false;
			if (streamLoadedCallback == null) return;
			string fileName = EditorUtility.OpenFilePanel("Load file", "", "png");
			if (!string.IsNullOrEmpty(fileName)) streamLoadedCallback(new FileStream(fileName, FileMode.Open, FileAccess.Read), true);
			else streamLoadedCallback(null, false);
			#else
			plugin.LoadFileDialog(folderLocation, fileTypes, noAsync_streamLoadedCallback);
			#endif
		}

		public static MemoryStream CopyToMemoryStream(Stream stream)
		{
			var memoryStream = new MemoryStream();
			var buffer = new byte[1024];
			while (true)
			{
				int readLength = stream.Read(buffer, 0, buffer.Length);
				memoryStream.Write(buffer, 0, readLength);
				if (readLength != buffer.Length) break;
			}
		
			memoryStream.Position = 0;
			return memoryStream;
		}
		
		public static string ConvertToPlatformSlash(string path)
		{
			#if UNITY_METRO || UNITY_WP8 || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			return path.Replace('/', '\\');
			#else
			return path.Replace('\\', '/');
			#endif
		}
		
		public static string GetFileDirectory(string fileName)
		{
			bool pass = false;
			foreach (var c in fileName)
			{
				if (c == '/' || c == '\\')
				{
					pass = true;
					break;
				}
			}
			if (!pass) return "";

			var match = Regex.Match(fileName, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			#if UNITY_METRO || UNITY_WP8
			return fileName + '\\';
			#else
			return fileName + '/';
			#endif
		}
		
		public static string GetFileNameWithExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*[/\\]");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = fileName.Substring(match.Value.Length, fileName.Length - match.Value.Length);
			}

			return fileName;
		}
		
		public static string GetFileNameWithoutExt(string fileName)
		{
			fileName = GetFileNameWithExt(fileName);
			string ext = GetFileExt(fileName);
			return fileName.Substring(0, fileName.Length - ext.Length);
		}

		public static string GetFileExt(string fileName)
		{
			var names = fileName.Split('.');
			if (names.Length < 2) return null;
			return '.' + names[names.Length-1];
		}

		public static string TrimFileExt(string fileName)
		{
			var match = Regex.Match(fileName, @".*\.");
			if (match.Success && !string.IsNullOrEmpty(match.Value))
			{
				fileName = match.Value.Substring(0, match.Value.Length-1);
			}

			return fileName;
		}

		public static bool IsAbsolutePath(string fileName)
		{
			#if UNITY_STANDALONE_WIN
			var match = Regex.Match(fileName, @"A|C|D|E|F|G|H|I:/|\\");
			return match.Success;
			#else
			throw new NotImplementedException();
			#endif
		}

		public static int MakeFourCC(char ch0, char ch1, char ch2, char ch3)
		{
			return (((int)(byte)(ch0)) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
		}
    }

	public static class StreamExtensions
	{
		#region Vectors
		public static void WriteVector(this BinaryWriter writer, Vector2 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
		}

		public static void WriteVector(this BinaryWriter writer, Vector3 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
			writer.Write(value.z);
		}

		public static void WriteVector(this BinaryWriter writer, Vector4 value)
		{
			writer.Write(value.x);
			writer.Write(value.y);
			writer.Write(value.z);
			writer.Write(value.w);
		}

		public static Vector2 ReadVector2(this BinaryReader reader)
		{
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector4 ReadVector4(this BinaryReader reader)
		{
			return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
		#endregion

		#region Matrices
		public static void WriteMatrix(this BinaryWriter writer, Matrix4x4 value)
		{
			writer.WriteVector(new Vector4(value.m00, value.m01, value.m02, value.m03));
			writer.WriteVector(new Vector4(value.m10, value.m11, value.m12, value.m13));
			writer.WriteVector(new Vector4(value.m20, value.m21, value.m22, value.m23));
			writer.WriteVector(new Vector4(value.m30, value.m31, value.m32, value.m33));
		}

		public static Matrix4x4 ReadMatrix4(this BinaryReader reader)
		{
			var x = reader.ReadVector4();
			var y = reader.ReadVector4();
			var z = reader.ReadVector4();
			var w = reader.ReadVector4();
			return new Matrix4x4()
			{
				m00 = x.x, m01 = x.y, m02 = x.z, m03 = x.w,
				m10 = y.x, m11 = y.y, m12 = y.z, m13 = y.w,
				m20 = z.x, m21 = z.y, m22 = z.z, m23 = z.w,
				m30 = w.x, m31 = w.y, m32 = w.z, m33 = w.w,
			};
		}
		#endregion
	}
}
