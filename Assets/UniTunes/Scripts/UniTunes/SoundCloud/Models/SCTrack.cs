using UnityEngine;
using System;

[Serializable]
public class SCTrack
{
	public SCTrack() {}
	
	public string id;
	public long duration;
	public bool streamable;
	public bool downloadable;
	public string sharing;
	public string genre;
	public string title;
	public string description;
	public string original_format;
	public string license;
	public SCUser user;
	public string uri;
	public string permalink_url;
	public string artwork_url;
	public string stream_url;
	public string waveform_url;
	public string download_url;

	public string FormattedTime(long seconds)
	{
		TimeSpan ts = System.TimeSpan.FromMilliseconds(seconds);
		string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
		return timeString;
	}
}