using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class SCSetEditor : EditorWindow
{
	#region UI properties
	private Vector2 windowScrollPos;
	#endregion
	
	
	#region builder model
	private SCSet scSet;
	#endregion
	
	
	#region EditorWindow Lifecycle
	[MenuItem("Window/SoundCloud Set")]
	private static void ShowWindow()
	{
		SCSetEditor win = EditorWindow.GetWindow<SCSetEditor>("SCSetEditor");
		
		if(win != null) {
			win.title = "SoundCloud Set";
			win.minSize = new Vector2(UniTunesConsts.MIN_WINDOW_WIDTH, UniTunesConsts.MIN_WINDOW_HEIGHT);
		}

		win.LoadSetConfig();
	}
	
	/// <summary>
	/// Validates the Menu Item based on if the Editor is Playing.
	/// </summary>
	/// <returns><c>true</c>, if RT was validated, <c>false</c> otherwise.</returns>
	[MenuItem("Window/SoundCloud Set", true)]
	private static bool ValidateRTD ()
	{
		return !EditorApplication.isPlaying;
	}

	void OnDestroy()
	{
		UniTunesGUITextFactory.ClearTextures();
	}
	
//	private void Update()
//	{
//		if(scSet == null) {
//			scSet = (SCSet) Resources.LoadAssetAtPath(UniTunesConsts.SC_CONFIG_FILE, typeof(SCSet));
//
//			//if the set is still null, create a new config file
//			if(scSet == null) {
//				Debug.Log("creating new scSet");
//				scSet = ScriptableObject.CreateInstance<SCSet>();
//				AssetDatabase.CreateAsset(scSet, UniTunesConsts.SC_CONFIG_FILE);
//			}
//		}
//	}
	
	private void OnGUI()
	{
		//render the default ui items
		SCUIAction uiAction = RenderSCAddTrack.RenderUI(scSet);

		switch(uiAction.Action) {

		case SCUIAction.ControlAction.Add:
			SoundCloudService.Instance.Resolve((string) uiAction.Data, OnResolveCallback, null);
			break;

		default:
			break;
		}
		
		EditorGUILayout.BeginVertical(); {

			windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos); {
				SCUIAction trackUiAction = RenderSCTracks.RenderUI(scSet);

				switch(trackUiAction.Action) {

				case SCUIAction.ControlAction.Play:
					SoundCloudService.Instance.StreamTrack(((SCTrack) trackUiAction.Data));
					break;
				
				case SCUIAction.ControlAction.Stop:
					SoundCloudService.Instance.StopPlayback();
					break;

				case SCUIAction.ControlAction.Remove:
					scSet.RemoveTrack((SCTrack) trackUiAction.Data);
					SaveConfig();
					break;

				case SCUIAction.ControlAction.MoveUp:
					scSet.Move((SCTrack) trackUiAction.Data, -1);
					SaveConfig();
					break;

				case SCUIAction.ControlAction.MoveDown:
					scSet.Move((SCTrack) trackUiAction.Data, 1);
					SaveConfig();
					break;

				default:
					Repaint();
					break;
				}

//				if(scSet != null) {
//					EditorUtility.SetDirty(scSet);
//				}
			}
			EditorGUILayout.EndScrollView();
		}
	}
	#endregion

	private void OnResolveCallback(SCServiceResponse response)
	{
		if(response.isSuccess) {
			Debug.Log(string.Format("DebugGUI.OnResolveCallback() - SUCCESS! {0}, streamable:{1}, from url:{2}",
			                        response.trackInfo.title,
			                        response.trackInfo.streamable,
			                        response.trackInfo.stream_url)
			);

			if(!scSet.AddTrack(response.trackInfo)) {
				EditorUtility.DisplayDialog(UniTunesConsts.EN_TRACK_ALREADY_ADDED_TITLE, UniTunesConsts.EN_TRACK_ALREADY_ADDED_MSG, "Ok");
			}

			SaveConfig();
		}
		else {
			Debug.Log("DebugGUI.OnResolveCallback(): " + response.errorMsg);

			EditorUtility.DisplayDialog(UniTunesConsts.EN_RESOLVE_FAIL_TITLE, UniTunesConsts.EN_RESOLVE_FAIL_MSG + "\n\n" + response.errorMsg, "Ok");
		}
	}

	private void LoadSetConfig()
	{
		string configPath = UniTunesUtils.GetSetConfigPath();

		if(File.Exists(configPath)) {
			scSet = UniTunesUtils.GetSetConfig<SCSet>();
		}
		else {
			scSet = new SCSet();
			UniTunesUtils.WriteSetConfig(scSet);
		}
	}

	private void SaveConfig()
	{
		if(scSet != null) {
			UniTunesUtils.WriteSetConfig(scSet);
		}
	}
}
