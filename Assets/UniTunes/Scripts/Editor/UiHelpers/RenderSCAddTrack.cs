﻿using UnityEngine;
using UnityEditor;

public static class RenderSCAddTrack
{
	private static string publicUrl = string.Empty;

	private static bool loop;

	public static SCUIAction RenderUI(SCSet scSet)
	{
		SCUIAction returnAction = new SCUIAction(SCUIAction.ControlAction.None, null);

		if(scSet == null) { return returnAction; }

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
					returnAction.SetProps(SCUIAction.ControlAction.Add, publicUrl);
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			loop = scSet.loopPlaylist;

			scSet.loopPlaylist = EditorGUILayout.Toggle("Loop Playlist", scSet.loopPlaylist);

			if(loop != scSet.loopPlaylist) {
				returnAction.SetProps(SCUIAction.ControlAction.LoopChange, scSet.loopPlaylist);
			}
		}

		return returnAction;
	}
}
