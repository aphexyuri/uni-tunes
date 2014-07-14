using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SCSetEditor : EditorWindow
{
	#region UI properties
	public static int MIN_WINDOW_WIDTH = 400;
	public static int MIN_WINDOW_HEIGHT = 400;
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
			win.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
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
	
	private void Update()
	{
		if(scSet == null) {
			scSet = (SCSet) Resources.LoadAssetAtPath(UniTunesUtils.GetSCConfigPath(), typeof(SCSet));

			//if the set is still null, create a new config file
			if(scSet == null) {
				scSet = ScriptableObject.CreateInstance<SCSet>();
				AssetDatabase.CreateAsset(scSet, UniTunesUtils.GetSCConfigPath());
			}
		}

//		SCService.Instance.IterateResolve();
	}

	private void OnGUI()
	{
		//render the default ui items
		SCUIAction uiAction = RenderSCAddTrack.RenderUI(scSet);

		switch(uiAction.Action) {

		case SCUIAction.ControlAction.Add:
			SCService.Instance.Resolve((string) uiAction.Data, OnResolveCallback, null);
			break;

		default:
			break;
		}
		
		EditorGUILayout.BeginVertical(); {

			windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos); {
				SCUIAction trackUiAction = RenderSCTracks.RenderUI(scSet);

				switch(trackUiAction.Action) {

				case SCUIAction.ControlAction.Play:
					SCService.Instance.StreamTrack(((SCTrack) trackUiAction.Data).stream_url);
					break;

				case SCUIAction.ControlAction.Remove:
					scSet.RemoveTrack((SCTrack) trackUiAction.Data);
					break;

				default:
					break;
				}
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
		}
		else {
			Debug.Log("DebugGUI.OnResolveCallback(): " + response.errorMsg);

			EditorUtility.DisplayDialog(UniTunesConsts.EN_RESOLVE_FAIL_TITLE, UniTunesConsts.EN_RESOLVE_FAIL_MSG + "\n\n" + response.errorMsg, "Ok");
		}
	}
}
