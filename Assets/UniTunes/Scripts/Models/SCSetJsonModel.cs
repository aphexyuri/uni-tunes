using System.Collections.Generic;

public class SCSetJsonModel
{
	public SCSetJsonModel() {}

	public SCSetJsonModel(SCSet scSet)
	{
		tracks = scSet.tracks;
		loopPlaylist = scSet.loopPlaylist;
	}

	public List<SCTrack> tracks;
	
	public bool loopPlaylist;

	public SCTrack GetTrackAtIndex(int index)
	{
		if(tracks != null) {
			if(index < tracks.Count) {
				return tracks[index];
			}
		}

		return null;
	}
}
