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
		//get the version expression
		Regex rgx = new Regex("[^0-9 . -]");
		string versionString = rgx.Replace(Application.unityVersion, "");
		Version version = new Version(versionString);
		
		//2D & sprites are not supported prior to 4.3...so we use UnityGUI in that case
//		if(version.Major < 4 || (version.Major == 4 && version.Minor < 3)) {
			_playerWidget = gameObject.GetComponentInChildren<GUISCPlayer>();
			GameObject spritePlayer = transform.Find("SpritePlayer").gameObject;
			spritePlayer.SetActive(false);
//		}
//		else {
//			_playerWidget = gameObject.GetComponentInChildren<SpriteSCPlayer>();
//			GameObject guiPlayer = transform.Find("GUIPlayer").gameObject;
//			guiPlayer.SetActive(false);
//		}

		_playerWidget.SetPlayerMessage(UniTunesConsts.EN_WAITING_FOR, UniTunesConsts.EN_PLAYLIST_CONFIG);
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
		_playerWidget.SetPlayerMessage(UniTunesConsts.EN_PRESS_BUTTON, UniTunesConsts.EN_TO_START_PLAYBACK);

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
			url = UniTunesUtils.GetSetConfigPath();
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
	#endregion
}
