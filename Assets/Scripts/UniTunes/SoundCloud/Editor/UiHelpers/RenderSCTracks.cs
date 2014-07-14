using UnityEngine;
using UnityEditor;

public static class RenderSCTracks
{
	public static SCUIAction RenderUI(SCSet scSet)
	{
		SCUIAction returnAction = new SCUIAction(SCUIAction.ControlAction.None, null);

		if(scSet == null) { return returnAction; }

		foreach(SCTrack track in scSet.tracks) {
			EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2048)); {

				GUI.color = Color.cyan;
				if(GUILayout.Button(UniTunesConsts.EN_BTN_PLAY, GUILayout.Width(50), GUILayout.ExpandHeight(true))) {
					returnAction.Action = SCUIAction.ControlAction.Play;
					returnAction.Data = track;
				}
				GUI.color = Color.white;

				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2048)); {
					EditorGUILayout.LabelField(track.title, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));

					EditorGUILayout.BeginHorizontal(); {
						EditorGUILayout.LabelField(UniTunesConsts.EN_DURATION, GUILayout.Width(60));
						EditorGUILayout.LabelField(track.duration.ToString("#,#") + "s", GUILayout.Width(100));
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();

				GUI.color = Color.yellow;
				if(GUILayout.Button(UniTunesConsts.EN_BTN_REMOVE, GUILayout.Width(55), GUILayout.ExpandHeight(true))) {
					returnAction.Action = SCUIAction.ControlAction.Remove;
					returnAction.Data = track;
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();
		}
		return returnAction;
	}
}
