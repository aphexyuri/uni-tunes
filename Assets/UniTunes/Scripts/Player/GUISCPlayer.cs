using UnityEngine;
using System.Collections;

public class GUISCPlayer : MonoBehaviour, ISCPlayer
{
	private int playerWidth = 300;
	private int playerHeight = 80;

	public delegate void OnPlayNextBtnPressed();
	public static event OnPlayNextBtnPressed OnPlayNextBtnPressedEvt;
	
	public delegate void OnPauseBtnPressed();
	public static event OnPauseBtnPressed OnPauseBtnPressedEvt;

	private bool playerMaximised = true;

	private string trackTitle;
	private string trackOwner;
	private string url;

	private Rect playerArea;

	private void Awake()
	{
		playerArea = new Rect(Screen.width - playerWidth, 0, playerWidth, playerHeight);

		if(SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.StartMinimized || 
		   SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.AlwaysMinimized)
		{
			playerMaximised = false;
		}
	}

	private void OnGUI()
	{
		SCUIAction uiAction = null;
		GUILayout.BeginArea(playerArea); {
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(2048)); {
				if(playerMaximised) {
					if(GUILayout.Button("Pause")) {
						uiAction = new SCUIAction(SCUIAction.ControlAction.Stop, null);
					}

					if(GUILayout.Button("Play")) {
						uiAction = new SCUIAction(SCUIAction.ControlAction.Play, null);
					}

					GUILayout.BeginVertical(); {
						GUILayout.Label(trackTitle);
						GUILayout.Label(trackOwner);
					}
					GUILayout.EndVertical();
				}
				else {
					GUILayout.FlexibleSpace();
				}

				if(GUILayout.Button("SC")) {
					uiAction = new SCUIAction(SCUIAction.ControlAction.MinimizeMaximise, null);
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();

		//handle action taken in GUI above
		if(uiAction != null) {
			switch(uiAction.Action) {
			case SCUIAction.ControlAction.Play:
				if(OnPlayNextBtnPressedEvt != null) {
					OnPlayNextBtnPressedEvt();
				}
				break;

			case SCUIAction.ControlAction.Stop:
				if(OnPauseBtnPressedEvt != null) {
					OnPauseBtnPressedEvt();
				}
				break;

			case SCUIAction.ControlAction.MinimizeMaximise:
				if(SoundCloudPlayer.Instance.playerMode != SoundCloudPlayer.PlayerMode.AlwaysMinimized &&
				   SoundCloudPlayer.Instance.playerMode != SoundCloudPlayer.PlayerMode.AlwaysMaximized)
				{
					playerMaximised = !playerMaximised;
				}
				break;
			}
		}
	}

	#region public API
	public void SetPlayerMessage(string message, string additionalMsg)
	{
		trackTitle = message;
		
		if(string.IsNullOrEmpty(additionalMsg)) {
			trackOwner = string.Empty;
		}
		else {
			trackOwner = additionalMsg;
		}
		
		url = string.Empty;
		
//		SetTrackInfoInternal();
	}
	
	public void SetTrackInfo(SCTrack track)
	{
		if(track != null ) {
			trackTitle = track.title;
			trackOwner = track.user.username;
			url = track.permalink_url;
		}
		
//		SetTrackInfoInternal();
	}

	public void MinimizePlayer()
	{
		playerMaximised = false;
	}

	public void MaximisePlayer()
	{
		playerMaximised = true;
	}
	#endregion
}
