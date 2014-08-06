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
		EditorUtility.SetDirty(scSet);

		UniTunesTextureFactory.ClearTextures();
	}

	private void OnGUI()
	{
		if(scSet == null) {
			LoadSetConfig();
		}

		EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(2048)); {
			//render the default ui items
			SCUIAction uiAction = RenderSCAddTrack.RenderUI(scSet);

			switch(uiAction.Action) {

			case SCUIAction.ControlAction.Add:
				SoundCloudService.Instance.Resolve((string) uiAction.Data, OnResolveCallback, null);
				break;

			default:
				break;
			}

			//render the tracks
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
					Undo.RecordObject(scSet, "Remove track");
					scSet.RemoveTrack((SCTrack) trackUiAction.Data);
					break;

				case SCUIAction.ControlAction.MoveUp:
					Undo.RecordObject(scSet, "Track order up");
					scSet.Move((SCTrack) trackUiAction.Data, -1);
					break;

				case SCUIAction.ControlAction.MoveDown:
					Undo.RecordObject(scSet, "Track order down");
					scSet.Move((SCTrack) trackUiAction.Data, 1);
					break;

				default:
					Repaint();
					break;
				}
			}
			EditorGUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			//render save json, load json & loop playlist controls
			EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2048)); {
				//loop checkbox
				if(scSet != null) {
					bool loop = scSet.loopPlaylist;
					scSet.loopPlaylist = EditorGUILayout.Toggle(scSet.loopPlaylist, GUILayout.Width(20));
					if(loop != scSet.loopPlaylist) { EditorUtility.SetDirty(scSet); }

					GUILayout.Label(UniTunesConsts.EN_LOOP_CHECKBOX, GUILayout.Width(80));
				}

				//load json btn
				GUI.color = Color.yellow;
				if(GUILayout.Button(UniTunesConsts.EN_LOAD_EXISTING_JSON, GUILayout.Width(160), GUILayout.ExpandHeight(true))) {

					string selectedFile = string.Empty;
					if(scSet.tracks == null || scSet.tracks.Count == 0) {
						//skip confirmation and go right to selecting a json file
						selectedFile = EditorUtility.OpenFilePanel(UniTunesConsts.EN_JSON_FILE_SELECT_TITLE, Application.dataPath, "json");
					}
					else {
						if(EditorUtility.DisplayDialog(UniTunesConsts.EN_JSON_LOAD_TITLE, UniTunesConsts.EN_JSON_LOAD_MSG, "Continue", "Cancel")) {
							selectedFile = EditorUtility.OpenFilePanel(UniTunesConsts.EN_JSON_FILE_SELECT_TITLE, Application.dataPath, "json");
						}
					}

					if(!string.IsNullOrEmpty(selectedFile)) {
						Debug.Log("Loading .json file at: " + selectedFile);
						SCSetJsonModel loadedSet = UniTunesUtils.GetSetConfigFromJsonFile<SCSetJsonModel>(selectedFile);

						if(loadedSet.tracks == null || loadedSet.tracks.Count == 0) {
							//no track info, so we're not doing anything further
							EditorUtility.DisplayDialog(UniTunesConsts.EN_JSON_INVALID_TITLE, UniTunesConsts.EN_JSON_INVALID_MSG, "Ok");
						}
						else {
							//tracks found, copy to a new SCSet instance
							SCSet set = ScriptableObject.CreateInstance<SCSet>();
							set.tracks = loadedSet.tracks;
							set.loopPlaylist = loadedSet.loopPlaylist;

							ConfigHardSave(set);
						}
					}
				}
				GUI.color = Color.white;

				//save json btn
				GUI.color = Color.green;
				if(GUILayout.Button(UniTunesConsts.EN_BTN_SAVE_JSON, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
					EditorUtility.DisplayDialog(UniTunesConsts.EN_JSON_SAVED_TITLE, string.Format(UniTunesConsts.EN_JSON_SAVED_MSG, UniTunesUtils.GetSetJsonConfigPath()), "Ok");
					UniTunesUtils.WriteSetJsonConfig(scSet);
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();
		}
	}
	#endregion

	private void OnResolveCallback(SCServiceResponse response)
	{
		if(response.isSuccess) {
			/*
			Debug.Log(string.Format("DebugGUI.OnResolveCallback() - SUCCESS! {0}, streamable:{1}, from url:{2}",
			                        response.trackInfo.title,
			                        response.trackInfo.streamable,
			                        response.trackInfo.stream_url)
			);
			*/

			Undo.RecordObject(scSet, "Track added");
			if(!scSet.AddTrack(response.trackInfo)) {
				EditorUtility.DisplayDialog(UniTunesConsts.EN_TRACK_ALREADY_ADDED_TITLE, UniTunesConsts.EN_TRACK_ALREADY_ADDED_MSG, "Ok");
			}
		}
		else {
			Debug.Log("DebugGUI.OnResolveCallback(): " + response.errorMsg);

			EditorUtility.DisplayDialog(UniTunesConsts.EN_RESOLVE_FAIL_TITLE, UniTunesConsts.EN_RESOLVE_FAIL_MSG + "\n\n" + response.errorMsg, "Ok");
		}
	}

	private void LoadSetConfig()
	{
		if(scSet == null) {
			scSet = (SCSet) Resources.LoadAssetAtPath(UniTunesConsts.SC_CONFIG_FILE, typeof(SCSet));

			if(scSet == null) {
				Debug.Log("creating new scSet");
				scSet = ScriptableObject.CreateInstance<SCSet>();
				AssetDatabase.CreateAsset(scSet, UniTunesConsts.SC_CONFIG_FILE);
			}
		}
	}

	private void ConfigHardSave(SCSet set)
	{
		AssetDatabase.DeleteAsset(UniTunesConsts.SC_CONFIG_FILE);
		AssetDatabase.CreateAsset(set, UniTunesConsts.SC_CONFIG_FILE);
	}
}
