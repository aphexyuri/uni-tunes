using UnityEngine;
using System.IO;
using System;

public static class UniTunesUtils
{
	private static string ReadFileToString(string path)
	{
		try {
			StreamReader sr = new StreamReader( path );
			string content = sr.ReadToEnd();
			
			sr.Close();
			return content;
		}
		catch(Exception e) {
			Debug.LogError("UniTunesUtils - Error reading file: " + path + ": " + e.Message);
			return null;
		}
	}

	public static bool WriteStringToFile(string path, string contents)
	{
		FileInfo fi = new FileInfo(path);
		Directory.CreateDirectory(fi.Directory.ToString());
		
		StreamWriter sw = File.CreateText(path);
		try {
			sw.Write(contents);
			sw.Close();
			
			Debug.Log("UniTunesUtils - File Saved: " + path + "\n" + contents);
			
			return true;
		}
		catch(Exception e) {
			sw.Close();
			Debug.LogError("UniTunesUtils - Error writing file: " + e.Message);
			return false;
		}
	}

	public static string GetSetJsonConfigPath()
	{
		return Path.Combine(Application.streamingAssetsPath, "SCConfig.json");
	}

	public static T GetSetConfigFromJsonFile<T>(string filePath)
	{
		string content = ReadFileToString(filePath);

		if(!string.IsNullOrEmpty(content)) {
			try {
				T config = (T) JsonFx.Json.JsonReader.Deserialize<T>(content);
				return config;
			}
			catch(Exception e) {
				Debug.LogError( "UniTunesUtils - Could not deserialize the Build Manifest: " + e.Message );
			}
		}

		return default(T);
	}
}
