using UnityEngine;
using System.Collections;

public class GUISCPlayer : MonoBehaviour, ISCPlayer
{
//	public delegate void OnPlayNextBtnPressed();
//	public static event OnPlayNextBtnPressed OnPlayNextBtnPressedEvt;
	
//	public delegate void OnPauseBtnPressed();
//	public static event OnPauseBtnPressed OnPauseBtnPressedEvt;

//	private string trackTitle; // "Boom boom \n boom track title here";
//	private string trackOwner; // "Markus Workstorm and The Wailers";
//	private string url; // "http://www.google.com";

	#region public API
	public void SetPlayerMessage(string message, string additionalMsg)
	{
//		trackTitle = message;
//		trackOwner = string.Empty;
//		url = string.Empty;
	}

	public void SetTrackInfo(SCTrack track)
	{
//		trackTitle = track.title;
//		trackOwner = track.user.username;
//		url = track.uri;
	}
	#endregion
}
