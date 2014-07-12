using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundCloudService : MonoSingleton<SoundCloudService>
{
		public static string CLIENT_ID = "344dc9bb8589e6c8b19ec142ea6a43af";
		
		public static string SOUNDCLOUD_API = "https://api.soundcloud.com";
		
		private static string SC_METHOD_RESOLVE = "/resolve.json";
		
		private WWW streamWWW;
		private AudioSource audioSource;
		
		private Action<string> callBack;
		private Action<SCServiceResponse> serviceResponse;
		
		void Update()
		{
			if(audioSource != null && !audioSource.isPlaying) {
				DisposeAudioSource();
			}
		}
		
		private void CallbackLog(string msg)
		{
			//Debug.Log(msg);
			
			if(callBack != null) {
				callBack(msg);
			}
		}
		
		private void DisposeAudioSource()
		{
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
		}
		
		private IEnumerator Resolve(string urlToResolve, bool playOnSuccess)
		{
				WWW request = new WWW(
						SoundCloudServiceUtils.BuildEndpoint(SC_METHOD_RESOLVE,
						new Dictionary<string, string>() {{"url", urlToResolve}}, true)
				);
				
				//wait for request to complete
				while(!request.isDone) {
						yield return new WaitForEndOfFrame();
				}
				
				//set error info to response
				if(request.error != null) {
						string errorString = string.Format("Error: {0} ({1})", request.error, urlToResolve);
						
						CallbackLog(errorString);
						serviceResponse(new SCServiceResponse(false, errorString));
						
						yield break;
				}
				else {
						SoundCloudTrack trackInfo = JsonFx.Json.JsonReader.Deserialize<SoundCloudTrack>(System.Text.Encoding.UTF8.GetString(request.bytes));
						
						if(playOnSuccess) {
							if(trackInfo.streamable && !string.IsNullOrEmpty(trackInfo.stream_url)) {
								StartCoroutine(StreamTrack(trackInfo.stream_url));
							}
						}
						else {
							serviceResponse(new SCServiceResponse(true, string.Empty, trackInfo));
						}
				}
		}
		
		private IEnumerator StreamTrack(string streamUrl)
		{
				streamUrl = SoundCloudServiceUtils.AppendClientId(streamUrl);
				
				CallbackLog("StreamTrack: " + streamUrl);
				
				streamWWW = new WWW(streamUrl);
				
				if(streamWWW.error != null) {
					CallbackLog("Error streaming track:" + streamWWW.error);
					yield break;
				}
				
				while(streamWWW.progress < 0.2) {
						//Log("...buffering " + (int) (request.progress*100) + "%");
						yield return new WaitForSeconds(0.1f);
				}
				
				//Log("Buffering Complete!");
				
				AudioClip audioClip = streamWWW.GetAudioClip(true, true, AudioType.MPEG);
				
				while(!audioClip.isReadyToPlay) {
						//Log("...clip not ready to play yet " + (int) (streamWWW.progress*100) + "%");
						yield return new WaitForSeconds(0.1f);
				}
				
				CallbackLog("audioClip.isReadyToPlay:" + audioClip.isReadyToPlay);
				CallbackLog("audioClip.length:" + audioClip.length);
				//Log("audioClip.samples:" + audioClip.samples);
				//Log("audioClip name: " + audioClip.name);
				
				//audioSource = gameObject.AddComponent<AudioSource>(); //auto component via annotation
				audioSource = audio;
				audioSource.clip = audioClip;
				audioSource.Play();
		}
		
		#region public API
		/// <summary>
		/// Resolve the specified public url and fire callback.
		/// </summary>
		/// <param name="url">URL to resolve to api-url</param>
		/// <param name="callback">Callback for completion, can be null</param>
		public void Resolve(string url, Action<SCServiceResponse> resolveCallback, Action<string> logCallback)
		{
			//set log callback if provided
			if(logCallback != null) { callBack = logCallback; }
			
			if(callBack != null) {
				serviceResponse = resolveCallback;
			}
			
			StartCoroutine(Resolve(url, false));
		}
		
		public void ResolveAndPlay(string url, Action<string> logCallback)
		{
			//set log callback if provided
			if(logCallback != null) { callBack = logCallback; }
			
			StartCoroutine(Resolve(url, true));
		}
		
		public void StopCurrentStreamPlayback()
		{
			DisposeAudioSource();
		}
		#endregion
}
