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

	public void Move(SCTrack track, int direction)
	{
		int oldIndex = tracks.IndexOf(track);
		int maxIndex = tracks.Count - 1;
		int newIndex = (direction > 0 ? oldIndex + 1 : oldIndex - 1);

		if(direction < 0) {
			//decreasing index
			if(oldIndex < 1) {
				//can't move before zero
				return;
			}
		}
		else if(direction > 0) {
			//increasing index
			if(oldIndex >= maxIndex) {
				//can't move past end
				return;
			}
		}

		SCTrack item = tracks[oldIndex];
		tracks.RemoveAt(oldIndex);
		tracks.Insert(newIndex, item);
	}
}
