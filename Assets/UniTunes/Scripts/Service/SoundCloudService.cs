using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class SoundCloudService : MonoSingleton<SoundCloudService>
{
	public delegate void OnServiceStatusChange(ServiceStatus status);
	public static event OnServiceStatusChange OnServiceStatusChangeEvt;

	public delegate void OnTrackComplete();
	public static event OnTrackComplete OnTrackCompleteEvt;

	public delegate void OnTrackStreamFailure();
	public static event OnTrackStreamFailure OnTrackStreamFailureEvt;

	private WWW streamWWW;
	private AudioSource audioSource;
	
	private Action<string> _logCallback;
	private Action<SCServiceResponse> _serviceCallback;

	//track currently being played
	private SCTrack _playbackTrack;

	private ServiceStatus _status = ServiceStatus.Ready;

	public enum ServiceStatus
	{
		Ready,
		Busy
	}


	#region Getters/Setters
	public SCTrack PlaybackTrack
	{
		get { return _playbackTrack; }
	}

	public ServiceStatus Status
	{
		get { return _status; }
		private set {
			_status = value;

			//send out status change event
			if(OnServiceStatusChangeEvt != null) { OnServiceStatusChangeEvt(_status); }
		}
	}
	#endregion


	#region Unity Lifecyclwe
	//adding this hack together with [ExecuteInEditMode] to avoid reference loss on manual gameobject deletion
	void OnDestroy()
	{
		SoundCloudService.ForceNullInstance();
	}
	#endregion


	private void CallbackLog(string msg)
	{
		Debug.Log(msg);
		
		if(_logCallback != null) {
			_logCallback(msg);
		}
	}
	
	private void DisposeAudioSource()
	{
		CancelInvoke("TrackComplete");

		_playbackTrack = null;

		if(audioSource == null) { return; }
		
		if(audioSource.isPlaying) {
			audioSource.Stop();
		}
		
		audioSource.clip = null;
		audioSource = null;
		
		if(streamWWW != null) {
			streamWWW.Dispose();
			streamWWW = null;
		}
		
		//remove callbacks if any
		if(_serviceCallback != null) { _serviceCallback = null; }
		if(_logCallback != null) { _logCallback = null; }

		Status = ServiceStatus.Ready;
	}
	
	private IEnumerator ResolveRoutine(string urlToResolve, bool playOnSuccess)
	{
		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		WWW request = new WWW(
			SCServiceUtils.BuildEndpoint(SC_METHOD_RESOLVE,
			new Hashtable (<string, string>() {{"url", urlToResolve}}, true)
		);
		#else
		WWW request = new WWW(
			SCServiceUtils.BuildEndpoint(UniTunesConsts.SC_METHOD_RESOLVE,
			new Dictionary<string, string>() {{"url", urlToResolve}}, true)
		);
		#endif
		
		//wait for request to complete
		while(!request.isDone) {
			yield return new WaitForSeconds(0.1f);
		}

		//set error info to response
		if(request.error != null) {
			string errorString = string.Format("Error: {0} ({1})", request.error, urlToResolve);
			
			CallbackLog(errorString);

			if(_serviceCallback != null) {
				_serviceCallback(new SCServiceResponse(false, errorString));
			}
			
			yield break;
		}
		else {
			CallbackLog("SUCCESS ADDING TRACK: " + System.Text.Encoding.UTF8.GetString(request.bytes));

			SCTrack trackInfo = JsonFx.Json.JsonReader.Deserialize<SCTrack>(System.Text.Encoding.UTF8.GetString(request.bytes));

			if(trackInfo.streamable && !string.IsNullOrEmpty(trackInfo.stream_url)) {
				if(_serviceCallback != null) {
					_serviceCallback(new SCServiceResponse(true, string.Empty, trackInfo));
				}

				if(playOnSuccess) {
					StartCoroutine(StreamTrackRoutine(trackInfo));
				}
			}
			else {
				if(_serviceCallback != null) {
					_serviceCallback(new SCServiceResponse(false, "Can't stream this track; access denied (" + trackInfo.sharing + ")"));
					yield break;
				}
			}
		}
	}
	
	private IEnumerator StreamTrackRoutine(SCTrack track)
	{
		if(Instance == null) {
			Debug.Log("Instance is null");
		}
		
		Status = ServiceStatus.Busy;
		
		string streamUrl = SCServiceUtils.AppendClientId(track.stream_url);
		streamUrl = SCServiceUtils.AppendClientId(track.stream_url);
		
		streamWWW = new WWW(streamUrl);
		
		if(streamWWW.error != null) {
			CallbackLog("Error streaming track:" + streamWWW.error);

			if(OnTrackStreamFailureEvt != null) {
				OnTrackStreamFailureEvt();
			}
			yield break;
		}
		
		while(streamWWW.progress < 0.2) {
			yield return new WaitForSeconds(0.1f);
		}
		
		AudioClip audioClip = streamWWW.GetAudioClip(false, true, AudioType.MPEG);
		
		while(!audioClip.isReadyToPlay) {
			yield return new WaitForSeconds(0.1f);
		}

		_playbackTrack = track;
		
//		CallbackLog("audioClip.isReadyToPlay:" + audioClip.isReadyToPlay);
//		CallbackLog("audioClip.length:" + audioClip.length);
//		CallbackLog("audioClip.samples:" + audioClip.samples);
		
		//audioSource = gameObject.AddComponent<AudioSource>(); //auto component via annotation
		audioSource = audio;
		audioSource.clip = audioClip;
		audioSource.Play();
		
		Invoke("TrackComplete", track.duration/1000);

		Status = ServiceStatus.Ready;
	}

	private void TrackComplete()
	{
		DisposeAudioSource();

		if(OnTrackCompleteEvt != null) {
			OnTrackCompleteEvt();
		}
	}
	
	#region public API
	/// <summary>
	/// Editor-only: Resolve the specified public url and fire callback.
	/// </summary>
	/// <param name="url">URL to resolve to api-url</param>
	/// <param name="callback">Callback for completion, can be null</param>
	public void Resolve(string url, Action<SCServiceResponse> resolveCallback, Action<string> logCallback)
	{
		//set callback if provided
		if(resolveCallback != null) { _serviceCallback = resolveCallback; }
		
		//set log callback if provided
		if(logCallback != null) { _logCallback = logCallback; }
		
#if UNITY_EDITOR
		EditorCoroutine.start(ResolveRoutine(url, false));
#else
		StartCoroutine(ResolveRoutine(url, false));
#endif
	}

	/// <summary>
	/// Streams the privided track
	/// </summary>
	/// <param name="track">Track to stream</param>
	public void StreamTrack(SCTrack track)
	{
		CancelInvoke("TrackComplete");

		if(Status == ServiceStatus.Busy) {
			Debug.LogWarning("Service is busy - ignoring request");
			return;
		}

#if UNITY_EDITOR
		EditorCoroutine.start(StreamTrackRoutine(track));
#else
		StartCoroutine(StreamTrackRoutine(track));
#endif
	}
	
//	public void ResolveAndPlay(string url, Action<SCServiceResponse> resolveCallback, Action<string> logCallback)
//	{
//		//set callback if provided
//		if(resolveCallback != null) { _serviceCallback = resolveCallback; }
//
//		
//		//set log callback if provided
//		if(logCallback != null) { _logCallback = logCallback; }
//		
//		StartCoroutine(ResolveRoutine(url, true));
//	}
	
	/// <summary>
	/// Stops the current playback, if any
	/// </summary>
	public void StopPlayback()
	{
		DisposeAudioSource();
	}
	
//	public void IterateResolve()
//	{
//			IEnumerator e = ResolveRoutine("", false);
//			e.MoveNext();
//	}
	#endregion
}
