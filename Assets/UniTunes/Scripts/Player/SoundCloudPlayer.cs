﻿using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

public class SoundCloudPlayer : UniTunesSingleton<SoundCloudPlayer>
{
	public SCPlayerDocking.Docking widgetDocking = SCPlayerDocking.Docking.None;
	public PlayerMode playerMode = PlayerMode.StartMaximized;

	public PlayerStatupMode playerStartupMode = PlayerStatupMode.Manual_ViaScript;
	public string autoStartupUrl = string.Empty;

	public enum PlayerStatupMode
	{
		Manual_ViaScript,
		Auto_StreamingAssets,
		Auto_Url
	}

	public enum PlayerMode
	{
		StartMaximized,
		StartMinimized,
		AlwaysMaximized,
		AlwaysMinimized
	}

	private SCSetJsonModel _scSet;
	private ISCPlayer _playerWidget;

	private int _currentPlayIndex = -1;

	#region Unity Lifecycle
	void Start()
	{
		GameObject guiPlayer = transform.Find("GUIPlayer").gameObject;
		guiPlayer.SetActive(true);

		_playerWidget = gameObject.GetComponentInChildren<GUISCPlayer>();

		_playerWidget.SetPlayerMessage(UniTunesConsts.EN_WAITING_FOR, UniTunesConsts.EN_PLAYLIST_CONFIG);

		//if the player startup is not manual, load config & start playback
		if(playerStartupMode == PlayerStatupMode.Auto_StreamingAssets) {
			LoadSet(true);
		}
		else if(playerStartupMode == PlayerStatupMode.Auto_Url) {
			if(!string.IsNullOrEmpty(autoStartupUrl)) {
				LoadSet(autoStartupUrl, true);
			}
			else {
				Debug.LogWarning("SoundCloudPlayer - you have selected Auto_Url as Player Startup Mode. Please provide a URL");
			}
		}
	}

	void OnEnable()
	{
		GUISCPlayer.OnPauseBtnPressedEvt += HandlePauseBtnPressed;
		GUISCPlayer.OnPlayNextBtnPressedEvt += HandlePlayNextBtnPressed;

		SoundCloudService.OnServiceStatusChangeEvt += HandleServiceStatusChange;
		SoundCloudService.OnTrackCompleteEvt += HandleTrackCompleteEvt;
		SoundCloudService.OnTrackStreamFailureEvt += HandleTrackStreamFailureEvt;
	}

	void OnDisable()
	{
		GUISCPlayer.OnPauseBtnPressedEvt -= HandlePauseBtnPressed;
		GUISCPlayer.OnPlayNextBtnPressedEvt -= HandlePlayNextBtnPressed;

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
		if(_scSet != null) {
			_playerWidget.SetPlayerMessage(UniTunesConsts.EN_PRESS_BUTTON, UniTunesConsts.EN_TO_START_PLAYBACK);
		}
		else {
			_playerWidget.SetPlayerMessage(UniTunesConsts.EN_WAITING_FOR, UniTunesConsts.EN_PLAYLIST_CONFIG);
		}

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
			_playerWidget.SetPlayerMessage(UniTunesConsts.EN_WAITING_FOR, UniTunesConsts.EN_AUDIO_STREAM);
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


	private IEnumerator LoadSetRoutine(string configUrl, bool autoPlay)
	{
		string url = string.Empty;

		if (string.IsNullOrEmpty(configUrl)) {
			url = UniTunesUtils.GetSetJsonConfigPath();
		} else {
			url = configUrl;
		}

		if(!url.StartsWith("http") && !url.StartsWith("file://") && !url.StartsWith("jar:")) {
			url = "file://" + url;
		}

		WWW www = new WWW(url);

		while(!www.isDone)
		{
			yield return null;
		}
		
		if(!string.IsNullOrEmpty(www.error)) {
			Debug.LogError("SoundCloudPlayer: Error Loading Set Data: " + www.error);
			yield break;
		}
		
		_scSet = JsonFx.Json.JsonReader.Deserialize<SCSetJsonModel>(www.text);

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

		if(autoPlay) {
			PlaySet();
		}
		else {
			_playerWidget.SetPlayerMessage(UniTunesConsts.EN_PRESS_BUTTON, UniTunesConsts.EN_TO_START_PLAYBACK);
		}
	}

	#region public API
	/// <summary>
	/// Loads the set configuration from remote json file
	/// </summary>
	/// <param name="configUrl">If the config is supplied via web, provide the full URL of the config file here</param>
	/// <param name="autoPlay">If set to <c>true</c> Automatically play the set after config is loaded</param>
	public void LoadSet(string configUrl, bool autoPlay)
	{
		StartCoroutine(LoadSetRoutine(configUrl, autoPlay));
	}

	/// <summary>
	/// Loads the set configuration from StreaminAssets/SCConfig.json (editor output location)
	/// </summary>
	/// <param name="autoPlay">If set to <c>true</c> auto play.</param>
	public void LoadSet(bool autoPlay)
	{
		LoadSet(string.Empty, autoPlay);
	}

	/// <summary>
	/// Starts playback from beginning of set (first track)
	/// </summary>
	public void PlaySet()
	{
		if(_scSet == null) {
			Debug.LogWarning("SoundCloud Set not valid or initialized");
		}
		else {
			_currentPlayIndex = 0;
			SCTrack firstTrack = _scSet.GetTrackAtIndex(_currentPlayIndex);
			if(firstTrack != null) {
				SoundCloudService.Instance.StreamTrack(firstTrack);
			}
		}
	}

	/// <summary>
	/// Plays the next track (same action as next button)
	/// </summary>
	public void PlayNext()
	{
		HandlePlayNextBtnPressed();
	}

	/// <summary>
	/// Plays the set from provided index
	/// </summary>
	/// <param name="index">Index.</param>
	public void PlayFromIndex(int index)
	{
		if(_scSet.tracks != null && index <= _scSet.tracks.Count - 1) {
			_currentPlayIndex = index;

			if(SoundCloudService.Instance.PlaybackTrack != null) {
				SoundCloudService.Instance.StopPlayback();
			}

			SCTrack nextTrack = _scSet.GetTrackAtIndex(_currentPlayIndex);

			if(nextTrack != null) {
				SoundCloudService.Instance.StreamTrack(nextTrack);
			}
		}
	}

	/// <summary>
	/// Stops the current playback.
	/// </summary>
	public void StopPlayback()
	{
		HandlePauseBtnPressed();
	}

	/// <summary>
	/// Minimizes the player.
	/// </summary>
	public void MinimizePlayer()
	{
		_playerWidget.MinimizePlayer();
	}

	/// <summary>
	/// Maximises the player.
	/// </summary>
	public void MaximisePlayer()
	{
		_playerWidget.MaximisePlayer();
	}

	/// <summary>
	/// Gets a List<SCTrack> of all tracks in set
	/// </summary>
	/// <returns>The tracks.</returns>
	public List<SCTrack> GetTracks()
	{
		return _scSet.tracks;
	}
	#endregion
}
