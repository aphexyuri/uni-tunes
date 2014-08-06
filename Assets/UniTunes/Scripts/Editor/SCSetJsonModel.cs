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
}
