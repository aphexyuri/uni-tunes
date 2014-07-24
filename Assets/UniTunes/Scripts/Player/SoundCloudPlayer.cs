using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

public class SoundCloudPlayer : MonoSingleton<SoundCloudPlayer>
{
	public SCPlayerDocking.Docking widgetDocking = SCPlayerDocking.Docking.None;
	public PlayerMode playerMode = PlayerMode.StartMaximized;

	public enum PlayerMode {
		StartMaximized,
		StartMinimized,
		AlwaysMaximized,
		AlwaysMinimized
	}

	private SCSet _scSet;
	private ISCPlayer _playerWidget;

	private int _currentPlayIndex = -1;

	#region Unity Lifecycle
	void Start()
	{
		LoadSet(true);
	}

	void OnEnable()
	{
		SpriteSCPlayer.OnPauseBtnPressedEvt += HandlePauseBtnPressed;
		SpriteSCPlayer.OnPlayNextBtnPressedEvt += HandlePlayNextBtnPressed;

		SoundCloudService.OnServiceStatusChangeEvt += HandleServiceStatusChange;
		SoundCloudService.OnTrackCompleteEvt += HandleTrackCompleteEvt;
		SoundCloudService.OnTrackStreamFailureEvt += HandleTrackStreamFailureEvt;
	}

	void OnDisable()
	{
		SpriteSCPlayer.OnPauseBtnPressedEvt -= HandlePauseBtnPressed;
		SpriteSCPlayer.OnPlayNextBtnPressedEvt -= HandlePlayNextBtnPressed;

		SoundCloudService.OnServiceStatusChangeEvt -= HandleServiceStatusChange;
		SoundCloudService.OnTrackCompleteEvt -= HandleTrackCompleteEvt;
		SoundCloudService.OnTrackStreamFailureEvt -= HandleTrackStreamFailureEvt;
	}
	#endregion


	#region GettersSetters
	#endregion


	#region event handlers
	private void HandlePauseBtnPressed()
	{
		if(_currentPlayIndex == -1) {
			PlaySet();
			return;
		}

		if(SoundCloudService.Instance.PlaybackTrack == null) {
			SCTrack nextTrack = _scSet.GetTrackAtIndex(_currentPlayIndex);
			if(nextTrack != null && nextTrack != SoundCloudService.Instance.PlaybackTrack) {
				SoundCloudService.Instance.StreamTrack(nextTrack);
			}
		}
		else {
			SoundCloudService.Instance.StopPlayback();
		}
	}

	private void HandlePlayNextBtnPressed()
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

	void HandleServiceStatusChange(SoundCloudService.ServiceStatus status)
	{
		if(status == SoundCloudService.ServiceStatus.Ready) {
			_playerWidget.SetTrackInfo(SoundCloudService.Instance.PlaybackTrack);
		}
		else {
			_playerWidget.SetTrackInfo(null);
		}
	}

	void HandleTrackCompleteEvt()
	{
		_playerWidget.SetTrackInfo(null);
		HandlePlayNextBtnPressed();
	}

	void HandleTrackStreamFailureEvt ()
	{
		HandlePlayNextBtnPressed();
	}
	#endregion


	private IEnumerator LoadSetRoutine(bool autoPlay)
	{
		string url = UniTunesUtils.GetSetConfigPath();

		if(!url.StartsWith("http") && !url.StartsWith("file://") && !url.StartsWith("jar:")) {
			url = "file://" + url;
		}

		WWW www = new WWW(url);

		while(!www.isDone)
		{
			yield return null;
		}
		
		if(!string.IsNullOrEmpty(www.error)) {
			Debug.LogError("Error Loading Set Data: " + www.error);
			yield break;
		}
		
		_scSet = JsonFx.Json.JsonReader.Deserialize<SCSet>(www.text);

		//makde sure the laod was successful
		if(_scSet == null) {
			Debug.LogWarning("SoundCloudPlayer: failed to load SCSet");
			yield break;
		}

		//make sure we have tracks in the set
		if(_scSet.tracks.Count < 1) {
			Debug.LogWarning("SoundCloudPlayer: No tracks in Set");
			yield break;
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

		if(autoPlay) {
			PlaySet();
		}
	}

	#region public API
	public void LoadSet(bool autoPlay)
	{
		StartCoroutine(LoadSetRoutine(autoPlay));
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
		else {
			Debug.LogWarning("SoundCloud Set not valid or initialized");
		}
	}
	#endregion
}
