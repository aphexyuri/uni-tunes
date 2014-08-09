using UnityEngine;
using System.Collections;

public class GUISCPlayer : MonoBehaviour, ISCPlayer
{
	private int[] playerWidths = new int[2] {320, 52};
	private int playerWidth = 320;
	private int playerHeight = 52;

	private static int titleLineLength = 50;
	private static int ownerLineLength = 30;

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

	private ScreenOrientation orientation;


	#region Unity Lifecycle
	private void Awake()
	{
		orientation = Screen.orientation;

		if(SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.StartMinimized || 
		   SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.AlwaysMinimized)
		{
			playerMaximised = false;
			playerWidth = playerWidths[1];
		}
		else {
			playerWidth = playerWidths[0];
		}

		SetPlayerArea();
	}

	private void OnGUI()
	{
		if(Screen.orientation != orientation) {
			orientation = Screen.orientation;

			StartCoroutine(OrientationChageDelay());
		}

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
						if(GUILayout.Button(titleLine, titleTxtGUIStyle)) {
							if(!string.IsNullOrEmpty(url)) {
								Application.OpenURL(url);
							}
						}

						if(GUILayout.Button(ownerLine, ownerTxtGUIStyle)) {
							if(!string.IsNullOrEmpty(url)) {
								Application.OpenURL(url);
							}
						}

//						GUILayout.Label(titleLine, titleTxtGUIStyle);
//						GUILayout.Label(ownerLine, ownerTxtGUIStyle);
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndVertical();
				}

				GUILayout.FlexibleSpace();

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

					if(playerMaximised) {
						playerWidth = playerWidths[0];
					}
					else {
						playerWidth = playerWidths[1];
					}

					SetPlayerArea();
				}
				else if(SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.AlwaysMinimized) {
					if(!string.IsNullOrEmpty(url)) {
						Application.OpenURL(url);
					}
				}
				break;
			}
		}
	}
	#endregion

	private IEnumerator OrientationChageDelay()
	{
		yield return new WaitForSeconds(0.3f);
		
		SetPlayerArea();
	}

	private void SetPlayerArea()
	{
		switch (SoundCloudPlayer.Instance.widgetDocking) {
		case SCPlayerDocking.Docking.TopLeft:
			playerArea = new Rect(0, 0, playerWidth, playerHeight);
			break;
			
		case SCPlayerDocking.Docking.TopCentre:
			playerArea = new Rect((Screen.width/2) - (playerWidth/2), 0, playerWidth, playerHeight);
			break;
			
		case SCPlayerDocking.Docking.TopRight:
			playerArea = new Rect(Screen.width - playerWidth, 0, playerWidth, playerHeight);
			break;
			
		case SCPlayerDocking.Docking.BottomLeft:
			playerArea = new Rect(0, Screen.height - playerHeight, playerWidth, playerHeight);
			break;
			
		case SCPlayerDocking.Docking.BottomCentre:
			playerArea = new Rect((Screen.width/2) - (playerWidth/2), Screen.height - playerHeight, playerWidth, playerHeight);
			break;
			
		case SCPlayerDocking.Docking.BottomRight:
			playerArea = new Rect(Screen.width - playerWidth, Screen.height - playerHeight, playerWidth, playerHeight);
			break;
			
		default:
			break;
		}
	}

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
	}
	
	public void SetTrackInfo(SCTrack track)
	{
		if(track != null ) {
			trackTitle = track.title;
			trackOwner = track.user.username;
			url = track.permalink_url;
		}
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
