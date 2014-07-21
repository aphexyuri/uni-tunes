using UnityEngine;
using System.IO;

public static class UniTunesConsts
{
	public static int MIN_WINDOW_WIDTH = 600;
	public static int MIN_WINDOW_HEIGHT = 400;

	public static string SC_CLIENT_ID = "344dc9bb8589e6c8b19ec142ea6a43af";
	public static string SC_API = "https://api.soundcloud.com";
	public static string SC_TEST_TRACK = "https://soundcloud.com/markus-wormstorm/feat-spoek-mathambo";
	public static string SC_METHOD_RESOLVE = "/resolve.json";

	public static string EDITOR_TEXTURE_PATH = "Assets/UniTunes/Textures/Editor";
	public static string EDITOR_TEXTURE_SCPLAYER = "Assets/UniTunes/Textures/SCPlayer";
	public static string SC_CONFIG_FILE = "Assets/UniTunes/SCConfig.asset";

	public static string EN_NOT_AVAILABLE_PLAYBACK = "Editing not available during playback";
	public static string EN_ADD_TRACK_TO_SET = "Add Track to Set (Provide URL)";
	public static string EN_TRACK_ALREADY_ADDED_TITLE = "Track not added";
	public static string EN_TRACK_ALREADY_ADDED_MSG = "The track was already added to your Set";
	public static string EN_RESOLVE_FAIL_TITLE = "Track not found";
	public static string EN_RESOLVE_FAIL_MSG = "The provided URL was not found on SoundCloud or is not set as 'public'.\n\nPlease verify the provided URL, and try again.";
	public static string EN_WAITING_FOR_STREAM = "WAITING FOR\nAUDIO STREAM...";

	public static string EN_BTN_PLAY = "Play";
	public static string EN_BTN_STOP = "Stop";
	public static string EN_BTN_REMOVE = "Remove";
	public static string EN_BTN_VALIDATE_ADD = "Validate & Add";

	public static string ConfigPath()
	{
		if(!Directory.Exists(Application.streamingAssetsPath)) {
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}

		return Path.Combine(Application.streamingAssetsPath, SC_CONFIG_FILE);
	}
}
