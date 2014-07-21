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

	private string trackTitle; // "Boom boom \n boom track title here";
	private string trackOwner; // "Markus Workstorm and The Wailers";
	private string url; // "http://www.google.com";

	public TextMesh titleTextMesh;
	public TextMesh ownerTextMesh;

	private float[] titlePositionsY = new float[2] {-0.07f, -0.15f};
	private float[] ownerPositionsY = new float[2] {-0.52f, -0.45f};


	#region Unity Lifecycle
	void OnEnable()
	{
		UniTunesBtn.OnBtnPressedEvt += OnBtnPressed;
	}

	void OnDisable()
	{
		UniTunesBtn.OnBtnPressedEvt -= OnBtnPressed;
	}
	#endregion


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
	public void SetTrackInfo(SCTrack track)
	{
		if(track == null ) {
			trackTitle = UniTunesConsts.EN_WAITING_FOR_STREAM;
			trackOwner = string.Empty;
			url = string.Empty;
		}
		else {
			trackTitle = track.title;
			trackOwner = track.user.username;
			url = track.permalink_url;
		}

		SetTrackInfo();
	}
	#endregion
}
