using UnityEngine;
using UnityEditor;

public static class RenderSCAddTrack
{
	private static string publicUrl = UniTunesConsts.TEST_TRACK;

	public static SCUIAction RenderUI(SCSet scSet)
	{
		SCUIAction returnAction = new SCUIAction(SCUIAction.ControlAction.None, null);

		if(EditorApplication.isPlaying) {
			GUILayout.Label(UniTunesConsts.EN_NOT_AVAILABLE_PLAYBACK, EditorStyles.boldLabel);
		}
		else {
			GUILayout.Label(UniTunesConsts.EN_ADD_TRACK_TO_SET, EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			{
				publicUrl = EditorGUILayout.TextField(publicUrl, GUILayout.MaxWidth(2048), GUILayout.ExpandWidth(true), GUILayout.Height(20));

				GUI.color = Color.green;
				if(GUILayout.Button(UniTunesConsts.EN_BTN_VALIDATE_ADD, GUILayout.Width(100), GUILayout.ExpandHeight(true))) {
					returnAction.Action = SCUIAction.ControlAction.Add;
					returnAction.Data = publicUrl;
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			scSet.loopPlaylist = EditorGUILayout.Toggle("Loop Playlist", scSet.loopPlaylist);
		}

		return returnAction;
	}
}
