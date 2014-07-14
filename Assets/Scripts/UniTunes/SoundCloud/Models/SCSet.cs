using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SCSet : ScriptableObject
{
//	[HideInInspector]
	public List<SCTrack> tracks;

	public bool AddTrack(SCTrack track)
	{
		if(tracks == null) {
			tracks = new List<SCTrack>();
		}

		//check if track already exist in set
		foreach(SCTrack t in tracks) {
			if(t.id == track.id) {
				return false;
			}
		}

		tracks.Add(track);

		return true;
	}

	public void RemoveTrack(SCTrack track)
	{
		if(tracks == null) { return; }

		tracks.Remove(track);
	}
}
