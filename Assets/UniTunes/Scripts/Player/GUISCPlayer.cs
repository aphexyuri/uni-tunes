using UnityEngine;
using System.Collections;

public class GUISCPlayer : MonoBehaviour, ISCPlayer
{
	private int playerWidth = 320;
	private int playerHeight = 52;

	private static int titleLineLength = 60;
	private static int ownerLineLength = 40;

	public delegate void OnPlayNextBtnPressed();
	public static event OnPlayNextBtnPressed OnPlayNextBtnPressedEvt;
	
	public delegate void OnPauseBtnPressed();
	public static event OnPauseBtnPressed OnPauseBtnPressedEvt;

	private bool playerMaximised = true;

	private string trackTitle;
	private string trackOwner;
	private string url;

	private Rect playerArea;

	public Texture pauseBtnTexture;
	public Texture playBtnTexture;
	public Texture soundCloudLogoTexture;
	public Texture2D backgroundTexture;

	public GUIStyle backgroundGUIStyle;
	public GUIStyle titleTxtGUIStyle;
	public GUIStyle ownerTxtGUIStyle;

	public GUISkin skin;


	#region Unity Lifecycle
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

		if(!playerMaximised) {
			backgroundGUIStyle.normal.background = null;
		}
		else {
			backgroundGUIStyle.normal.background = backgroundTexture;
		}

		string titleLine = FormatLine(trackTitle, titleLineLength);
		string ownerLine = FormatLine(trackOwner, ownerLineLength);

		GUILayout.BeginArea(playerArea, backgroundGUIStyle); {
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.MaxHeight(2048)); {
				if(playerMaximised) {
					GUILayout.BeginHorizontal(GUILayout.MaxWidth(69)); {
						if(GUILayout.Button(pauseBtnTexture, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(42))) {
							uiAction = new SCUIAction(SCUIAction.ControlAction.Stop, null);
						}

						GUILayout.FlexibleSpace();

						if(GUILayout.Button(playBtnTexture, GUIStyle.none, GUILayout.Width(32), GUILayout.Height(42))) {
							uiAction = new SCUIAction(SCUIAction.ControlAction.Play, null);
						}
					}
					GUILayout.EndHorizontal();

					GUILayout.BeginVertical(); {
						GUILayout.FlexibleSpace();
						GUILayout.Label(titleLine, titleTxtGUIStyle);
						GUILayout.Label(ownerLine, ownerTxtGUIStyle);
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndVertical();
				}
//				else {
					GUILayout.FlexibleSpace();
//				}

				GUILayout.BeginVertical(); {
					GUILayout.FlexibleSpace();
					if(GUILayout.Button(soundCloudLogoTexture, GUIStyle.none, GUILayout.Width(41), GUILayout.Height(28))) {
						uiAction = new SCUIAction(SCUIAction.ControlAction.MinimizeMaximise, null);
					}
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndVertical();
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
	#endregion

	private string FormatLine(string line, int length)
	{
		if(line.Length > length) {
			return line.Substring(0, length - 3) + "...";
		}
		else {
			return line;
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
