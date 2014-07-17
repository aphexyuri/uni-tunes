using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class SoundCloudPlayer : MonoSingleton<SoundCloudPlayer>
{
	private SCSet _scSet;
	private ISCPlayer _playerWidget;

	private int _currentPlayIndex = -1;

	public Docking widgetDocking = Docking.TopRight;

	public enum Docking
	{
		TopCentre,
		TopLeft,
		TopRight,
		BottomCentre,
		BottomLeft,
		BottomRight
	}

	#region Unity Lifecycle
	void Start()
	{
		LoadSet(false);
	}

	void OnEnable()
	{
		SpriteSCPlayer.OnPauseBtnPressedEvt += OnPauseBtnPressed;
		SpriteSCPlayer.OnPlayNextBtnPressedEvt += OnPlayNextBtnPressed;
		SoundCloudService.OnServiceStatusChangeEvt += OnServiceStatusChange;
	}


	
	void OnDisable()
	{
		SpriteSCPlayer.OnPauseBtnPressedEvt -= OnPauseBtnPressed;
		SpriteSCPlayer.OnPlayNextBtnPressedEvt -= OnPlayNextBtnPressed;
		SoundCloudService.OnServiceStatusChangeEvt -= OnServiceStatusChange;
	}
	#endregion


	#region GettersSetters
	#endregion


	#region event handlers
	private void OnPauseBtnPressed()
	{
		if(_currentPlayIndex == -1) {
			PlaySet();
			return;
		}

		SoundCloudService.Instance.StopPlayback();
	}

	private void OnPlayNextBtnPressed()
	{
		if(_currentPlayIndex == -1) {
			PlaySet();
			return;
		}

		//if we're at the last track and looping is enabled, start at first track
		if(_currentPlayIndex == (_scSet.tracks.Count - 1)) {
			if(_scSet.loopPlaylist) {
				_currentPlayIndex = 0;
			}
		}
		else {
			_currentPlayIndex ++;
		}

		SCTrack nextTrack = _scSet.GetTrackAtIndex(_currentPlayIndex);

		if(nextTrack != null && nextTrack != SoundCloudService.Instance.PlaybackTrack) {
			SoundCloudService.Instance.StreamTrack(nextTrack);
		}
	}

	void OnServiceStatusChange(SoundCloudService.ServiceStatus status)
	{
		if(status == SoundCloudService.ServiceStatus.Ready) {
			_playerWidget.SetTrackInfo(SoundCloudService.Instance.PlaybackTrack);
		}
		else {
			_playerWidget.SetTrackInfo(null);
		}
	}
	#endregion


	#region public API
	public void LoadSet(bool autoPlay)
	{
		_scSet = (SCSet) Resources.LoadAssetAtPath(UniTunesConsts.SC_CONFIG_PATH, typeof(SCSet));

		//makde sure the laod was successful
		if(_scSet == null) {
			Debug.LogWarning("SoundCloudPlayer: failed to load SCSet");
			return;
		}

		//make sure we have tracks in the set
		if(_scSet.tracks.Count < 1) {
			Debug.LogWarning("SoundCloudPlayer: No tracks in Set");
			return;
		}

		//get the version expression
		Regex rgx = new Regex("[^0-9 . -]");
		string versionString = rgx.Replace(Application.unityVersion, "");
		Version version = new Version(versionString);

		//2D & sprites are not supported prior to 4.3...so we use UnityGUI in that case
		if(version.Major < 4 || (version.Major == 4 && version.Minor < 3)) {
			_playerWidget = gameObject.GetComponentInChildren<GUISCPlayer>();
		}
		else {
			_playerWidget = gameObject.GetComponentInChildren<SpriteSCPlayer>();
		}
	}

	public void PlaySet()
	{
		if(_scSet != null) {
			_currentPlayIndex = 0;
			SCTrack firstTrack = _scSet.GetTrackAtIndex(_currentPlayIndex);
			if(firstTrack != null) {
				SoundCloudService.Instance.StreamTrack(firstTrack);
			}
		}
	}
	#endregion
}
