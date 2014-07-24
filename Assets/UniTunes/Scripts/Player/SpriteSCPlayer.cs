using UnityEngine;
using System.Collections;

public class SpriteSCPlayer : MonoBehaviour, ISCPlayer
{
	public delegate void OnPlayNextBtnPressed();
	public static event OnPlayNextBtnPressed OnPlayNextBtnPressedEvt;

	public delegate void OnPauseBtnPressed();
	public static event OnPauseBtnPressed OnPauseBtnPressedEvt;

	private static int titleLineLength = 30;
	private static int titleLineTwoLength = 28;
	private static int ownerLineLength = 26;

	private float[] titlePositionsY = new float[2] {-0.07f, -0.15f};
	private float[] ownerPositionsY = new float[2] {-0.52f, -0.45f};
	private float[] soundCloudMinMaxPositionsX = new float[3] {0.42f, 4.58f, 2.5f};

	private bool playerMaximised = true;

	private string trackTitle;
	private string trackOwner;
	private string url;

	private TextMesh titleTextMesh;
	private TextMesh ownerTextMesh;

	private GameObject btnPause;
	private GameObject btnNext;
	private GameObject background;
	private GameObject soundCloudLogo;

	#region Unity Lifecycle
	void Awake()
	{
		titleTextMesh = transform.Find("TitleTextMesh").GetComponent<TextMesh>();
		ownerTextMesh = transform.Find("OwnerTextMesh").GetComponent<TextMesh>();

		btnPause = transform.Find("BtnPause").gameObject;
		btnNext = transform.Find("BtnNext").gameObject;
		background = transform.Find("Background").gameObject;
		soundCloudLogo = transform.Find("SoundCloudLogo").gameObject;

		if(SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.StartMinimized ||
		   SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.AlwaysMinimized)
		{
			MinMaxPlayer(false);
		}
	}

	void OnEnable()
	{
		UniTunesBtn.OnBtnPressedEvt += OnBtnPressed;
	}

	void OnDisable()
	{
		UniTunesBtn.OnBtnPressedEvt -= OnBtnPressed;
	}
	#endregion


	private void MinMaxPlayer(bool maximized)
	{
		btnPause.SetActive(maximized);
		btnNext.SetActive(maximized);
		background.SetActive(maximized);
		
		titleTextMesh.renderer.enabled = maximized;
		ownerTextMesh.renderer.enabled = maximized;

		//re-layout soundcloud logo if we're not on right bounds of screen
		Vector3 soundCloudPos = soundCloudLogo.transform.localPosition;
		if(SoundCloudPlayer.Instance.widgetDocking == SCPlayerDocking.Docking.TopLeft || SoundCloudPlayer.Instance.widgetDocking == SCPlayerDocking.Docking.BottomLeft) {
			if(maximized) {
				soundCloudPos.x = soundCloudMinMaxPositionsX[1];
			}
			else {
				soundCloudPos.x = soundCloudMinMaxPositionsX[0];
			}
		}
		else if(SoundCloudPlayer.Instance.widgetDocking == SCPlayerDocking.Docking.TopCentre || SoundCloudPlayer.Instance.widgetDocking == SCPlayerDocking.Docking.BottomCentre) {
			if(maximized) {
				soundCloudPos.x = soundCloudMinMaxPositionsX[1];
			}
			else {
				soundCloudPos.x = soundCloudMinMaxPositionsX[2];
			}
		}
		soundCloudLogo.transform.localPosition = soundCloudPos;

		playerMaximised = maximized;
	}

	private void SetTrackInfo()
	{
		Vector3 titlePos = titleTextMesh.transform.localPosition;
		Vector3 ownerPos = ownerTextMesh.transform.localPosition;

		//if we're spanning accross multiple lines, we need to do some formatting of the title
		if(trackTitle.Length > titleLineLength) {
			int bestLinebreakPos = GetNearestLinebreakIndex(trackTitle, titleLineLength);

			string lineOne = string.Empty;
			string lineTwo = string.Empty;

			if(bestLinebreakPos == -1) {
				lineOne = trackTitle.Substring(0, titleLineLength) + "-";
				lineTwo = "-" + trackTitle.Substring(titleLineLength, trackTitle.Length - titleLineLength);
			}
			else {
				lineOne = trackTitle.Substring(0, bestLinebreakPos);
				lineTwo = trackTitle.Substring(bestLinebreakPos, trackTitle.Length - bestLinebreakPos);
			}

			//remove leading blank space on line two if any
			if(lineTwo.StartsWith(" ")) {
				lineTwo = lineTwo.Remove(0, 1);
			}

			//trim line two if too long
			if(lineTwo.Length > titleLineTwoLength) {
				lineTwo = lineTwo.Substring(0, titleLineTwoLength - 3) + "..."; 
			}

			//concat the two lines
			trackTitle = lineOne + "\n" + lineTwo;

			//set the field positions
			titlePos.y = titlePositionsY[0];
			ownerPos.y = ownerPositionsY[0];
			titleTextMesh.transform.localPosition = titlePos;
			ownerTextMesh.transform.localPosition = ownerPos;
		}
		else {
			//set the field positions
			titlePos.y = titlePositionsY[1];
			ownerPos.y = ownerPositionsY[1];
			titleTextMesh.transform.localPosition = titlePos;
			ownerTextMesh.transform.localPosition = ownerPos;
		}
		titleTextMesh.text = trackTitle;

		//check owner value, trim and append elipses if needed
		if(trackOwner.Length > ownerLineLength) {
			trackOwner = trackOwner.Substring(0, ownerLineLength - 3) + "...";
		}
		ownerTextMesh.text = trackOwner;
	}

	private void OnBtnPressed(string btnName)
	{
		switch(btnName) {
		case "BtnPause":
			if(OnPauseBtnPressedEvt != null) {
				OnPauseBtnPressedEvt();
			}
			break;

		case "BtnNext":
			if(OnPlayNextBtnPressedEvt != null) {
				OnPlayNextBtnPressedEvt();
			}
			break;

		case "Background":
			if(!string.IsNullOrEmpty(url)) {
				Application.OpenURL(url);
			}
			break;

		case "SoundCloudLogo":
			if(SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.AlwaysMinimized ||
			   SoundCloudPlayer.Instance.playerMode == SoundCloudPlayer.PlayerMode.AlwaysMaximized)
			{
				return;
			}

			if(playerMaximised) {
				//mazimize player
				MinMaxPlayer(false);
			}
			else {
				//minimize player
				MinMaxPlayer(true);
			}
			break;
		}
	}
	
	private int GetNearestLinebreakIndex(string sourceString, int desiredBreakIndex)
	{
		int count = 0;

		string sub = string.Empty;
		for(int i = desiredBreakIndex; i >= 0; i--) {
			if(count >= UnityEngine.Mathf.Round(desiredBreakIndex/2.0f)) {
				//we've already gone back half of the line length, so get out
				return -1;
			}

			sub = sourceString.Substring(i, 1);
			if(sub == " ") {
				//break found
				return i;
			}
			count ++;
		}

		return desiredBreakIndex;
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

		SetTrackInfo();
	}

	public void SetTrackInfo(SCTrack track)
	{
		if(track != null ) {
			trackTitle = track.title;
			trackOwner = track.user.username;
			url = track.permalink_url;
		}

		SetTrackInfo();
	}
	#endregion
}
