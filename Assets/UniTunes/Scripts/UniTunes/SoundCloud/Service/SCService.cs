using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Editor;

[ExecuteInEditMode]
[RequireComponent(typeof(AudioSource))]
public class SCService : MonoSingleton<SCService>
{
	public static string CLIENT_ID = "344dc9bb8589e6c8b19ec142ea6a43af";
	
	public static string SOUNDCLOUD_API = "https://api.soundcloud.com";
	
	private static string SC_METHOD_RESOLVE = "/resolve.json";
	
	private WWW streamWWW;
	private AudioSource audioSource;
	
	private Action<string> _logCallback;
	private Action<SCServiceResponse> _serviceCallback;

	//track currently being played
	private SCTrack _playbackTrack;


	#region Getters/Setters
	public SCTrack PlaybackTrack
	{
		get { return _playbackTrack; }
	}
	#endregion


	#region Unity Lifecyclwe
	void Update()
	{
		if(audioSource != null && !audioSource.isPlaying) {
			DisposeAudioSource();
		}
	}

	//adding this hack together with [ExecuteInEditMode] to avoid reference loss on manual gameobject deletion
	void OnDestroy()
	{
		SCService.ForceNullInstance();
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
	}
	
	private IEnumerator ResolveRoutine(string urlToResolve, bool playOnSuccess)
	{
		CallbackLog("Resolve routine: " + urlToResolve);

		#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		WWW request = new WWW(
			SCServiceUtils.BuildEndpoint(SC_METHOD_RESOLVE,
			new Hashtable (<string, string>() {{"url", urlToResolve}}, true)
		);
		#else
		WWW request = new WWW(
			SCServiceUtils.BuildEndpoint(SC_METHOD_RESOLVE,
			new Dictionary<string, string>() {{"url", urlToResolve}}, true)
		);
		#endif
		
		//wait for request to complete
		while(!request.isDone) {
			//CallbackLog("Waiting for Resolve");

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
			CallbackLog(System.Text.Encoding.UTF8.GetString(request.bytes));

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

		if(gameObject == null) {
			Debug.Log("gameobject is not active");
		}
		
		string streamUrl = SCServiceUtils.AppendClientId(track.stream_url);
		streamUrl = SCServiceUtils.AppendClientId(track.stream_url);
		
		CallbackLog("StreamTrack: " + streamUrl);
		
		streamWWW = new WWW(streamUrl);
		
		if(streamWWW.error != null) {
			CallbackLog("Error streaming track:" + streamWWW.error);
			yield break;
		}
		
		while(streamWWW.progress < 0.2) {
			//CallbackLog("...buffering " + (int) (request.progress*100) + "%");
			yield return new WaitForSeconds(0.1f);
		}
		
		//Log("Buffering Complete!");
		
		AudioClip audioClip = streamWWW.GetAudioClip(true, true, AudioType.MPEG);
		
		while(!audioClip.isReadyToPlay) {
			//CallbackLog("...clip not ready to play yet " + (int) (streamWWW.progress*100) + "%");
			yield return new WaitForSeconds(0.1f);
		}

		_playbackTrack = track;
		
		CallbackLog("audioClip.isReadyToPlay:" + audioClip.isReadyToPlay);
		CallbackLog("audioClip.length:" + audioClip.length);
		CallbackLog("audioClip.samples:" + audioClip.samples);
		CallbackLog("audioClip name: " + audioClip.name);
		
		//audioSource = gameObject.AddComponent<AudioSource>(); //auto component via annotation
		audioSource = audio;
		audioSource.clip = audioClip;
		audioSource.Play();
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
		
		EditorCoroutine.start(ResolveRoutine(url, false));
	}

	public void StreamTrack(SCTrack track)
	{
//		StartCoroutine(StreamTrackRoutine(track));
		EditorCoroutine.start(StreamTrackRoutine(track));
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
