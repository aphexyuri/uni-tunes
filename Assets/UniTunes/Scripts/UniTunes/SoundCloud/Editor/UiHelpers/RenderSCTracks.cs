﻿using UnityEngine;
using UnityEditor;
using System;

public static class RenderSCTracks
{
	public static SCUIAction RenderUI(SCSet scSet)
	{
		SCUIAction returnAction = new SCUIAction(SCUIAction.ControlAction.None, null);

		if(scSet == null || scSet.tracks == null) { return returnAction; }

		int index = 0;
		foreach(SCTrack track in scSet.tracks) {
			EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2048)); {

				//up/down buttons
				EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(55)); {
					GUILayout.FlexibleSpace();
					if(scSet.tracks.Count == 1) {
						//draw blank
						if(GUILayout.Button(UniTunesGUITexFactory.GetUpDownBlankTexture(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
							//do nothing
						}
					}
					else {
						if(index == 0) {
							//no up btn
							if(GUILayout.Button(UniTunesGUITexFactory.GetDownFullBtnTexture(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
								returnAction.SetProps(SCUIAction.ControlAction.MoveDown, track);
							}
						}
						else if(index == scSet.tracks.Count - 1) {
							//no down btn
							if(GUILayout.Button(UniTunesGUITexFactory.GetUpFullBtnTexture(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
								returnAction.SetProps(SCUIAction.ControlAction.MoveUp, track);
							}
						}
						else {
							//draw both
							if(GUILayout.Button(UniTunesGUITexFactory.GetUpBtnTexture(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
								returnAction.SetProps(SCUIAction.ControlAction.MoveUp, track);
							}

							if(GUILayout.Button(UniTunesGUITexFactory.GetDownBtnTexture(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
								returnAction.SetProps(SCUIAction.ControlAction.MoveDown, track);
							}
						}
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndVertical();

				//stop/play btn
				if(SCService.Instance.PlaybackTrack != null && track.id == SCService.Instance.PlaybackTrack.id) {
					if(GUILayout.Button(UniTunesGUITexFactory.GetStopBtnTexture(), GUIStyle.none)) {
						returnAction.SetProps(SCUIAction.ControlAction.Stop, track);
					}
				}
				else {
					if(GUILayout.Button(UniTunesGUITexFactory.GetPlayBtnTexture(), GUIStyle.none)) {
						returnAction.SetProps(SCUIAction.ControlAction.Play, track);
					}
				}

				//track info
				EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(2048)); {
					EditorGUILayout.LabelField(track.title, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));

					if(track.user != null && !string.IsNullOrEmpty(track.user.username)) {
						EditorGUILayout.LabelField("By: " + track.user.username, GUILayout.ExpandWidth(true));
					}

					EditorGUILayout.BeginHorizontal(); {
						EditorGUILayout.LabelField(track.FormattedTime(track.duration), GUILayout.Width(100));
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();

				//remove btn
				if(GUILayout.Button(UniTunesGUITexFactory.GetRemoveBtnTexture(), GUIStyle.none, GUILayout.ExpandHeight(true))) {
					returnAction.SetProps(SCUIAction.ControlAction.Remove, track);
				}
			}
			EditorGUILayout.EndHorizontal();
			index ++;
		}
		return returnAction;
	}
}
