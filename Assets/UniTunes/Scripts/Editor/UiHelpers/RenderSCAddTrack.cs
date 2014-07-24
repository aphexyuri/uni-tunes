using UnityEngine;
using UnityEditor;

public static class RenderSCAddTrack
{
	private static string publicUrl = string.Empty;

	private static bool loop;

	public static SCUIAction RenderUI(SCSet scSet)
	{
		SCUIAction returnAction = new SCUIAction(SCUIAction.ControlAction.None, null);

		if(scSet == null) { return returnAction; }

		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2048));
		{
			EditorGUILayout.BeginVertical(GUILayout.Height(30));
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label(UniTunesConsts.EN_ADD_TRACK_TO_SET, EditorStyles.boldLabel);
			}
			EditorGUILayout.EndVertical();

			GUILayout.FlexibleSpace();

			if(GUILayout.Button(UniTunesGUITextFactory.GetSoundCloudLogo(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
				Application.OpenURL("http://developers.soundcloud.com");
			}
		}
		EditorGUILayout.EndHorizontal();




		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		{
			publicUrl = EditorGUILayout.TextField(publicUrl, GUILayout.MaxWidth(2048), GUILayout.ExpandWidth(true), GUILayout.Height(25));

			GUI.color = Color.green;
			if(GUILayout.Button(UniTunesConsts.EN_BTN_VALIDATE_ADD, GUILayout.Width(100), GUILayout.ExpandHeight(true))) {
				returnAction.SetProps(SCUIAction.ControlAction.Add, publicUrl);
			}
			GUI.color = Color.white;
		}
		EditorGUILayout.EndHorizontal();

		return returnAction;
	}
}
