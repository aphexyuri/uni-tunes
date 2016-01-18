using UnityEngine;
using System.IO;

public static class UniTunesConsts
{
	//soundcloud editor window
	public static int MIN_WINDOW_WIDTH = 600;
	public static int MIN_WINDOW_HEIGHT = 400;

	//misc soundcloud values
	public static string SC_CLIENT_ID = "YOUR_SOUNDCLOUD_CLIENT_ID";
	public static string SC_API = "https://api.soundcloud.com";
	public static string SC_TEST_TRACK = "https://soundcloud.com/markus-wormstorm/feat-spoek-mathambo";
	public static string SC_METHOD_RESOLVE = "/resolve.json";
	public static string EDITOR_TEXTURE_PATH = "Assets/UniTunes/Textures/Editor";
	public static string TEXTURES_SCGUIPLAYER = "SCGUIPlayer";
	public static string SC_CONFIG_FILE = "Assets/UniTunes/SCConfig.asset";

	public static string EN_NOT_AVAILABLE_PLAYBACK = "Editing not available during playback";
	public static string EN_ADD_TRACK_TO_SET = "Add Track to Set (Provide URL)";

	//alerts
	public static string EN_TRACK_ALREADY_ADDED_TITLE = "Track not added";
	public static string EN_TRACK_ALREADY_ADDED_MSG = "The track was already added to your Set";
	public static string EN_RESOLVE_FAIL_TITLE = "Track not found";
	public static string EN_RESOLVE_FAIL_MSG = "The provided URL was not found on SoundCloud or is not set as 'public'.\n\nPlease verify the provided URL, and try again.";
	public static string EN_JSON_SAVED_TITLE = "Exporting Config";
	public static string EN_JSON_SAVED_MSG = "The set .json config file will be exported to:\n\n{0}";
	public static string EN_JSON_LOAD_TITLE = "Import from exising Json Config";
	public static string EN_JSON_LOAD_MSG = "This will overwrite your existing set, and is permanent.\n\nAre you sure you want to continue?";
	public static string EN_JSON_INVALID_TITLE = "Imported config is empty";
	public static string EN_JSON_INVALID_MSG = "The Json config you imported does not contain any tracks.\n\nImport action ignored";

	//player user feedback
	public static string EN_WAITING_FOR = "WAITING FOR";
	public static string EN_AUDIO_STREAM = "AUDIO STREAM";
	public static string EN_PLAYLIST_CONFIG = "PLAYLIST CONFIG";
	public static string EN_PRESS_BUTTON = "PRESS A BUTTON";
	public static string EN_TO_START_PLAYBACK = "TO START PLAYBACK";

	//editor ui
	public static string EN_BTN_VALIDATE_ADD = "Validate & Add";
	public static string EN_BTN_SAVE_JSON = "Export Json Config";
	public static string EN_LOOP_CHECKBOX = "Loop Playlist";
	public static string EN_LOAD_EXISTING_JSON = "Import from .json Config";
	public static string EN_JSON_FILE_SELECT_TITLE = "Select .json file to import";

	public static string ConfigPath()
	{
		if(!Directory.Exists(Application.streamingAssetsPath)) {
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}

		return Path.Combine(Application.streamingAssetsPath, SC_CONFIG_FILE);
	}
}
