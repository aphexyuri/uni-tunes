using UnityEngine;
using System.Collections;

public class DebugGUI : MonoBehaviour
{
	private int screenButtonHeight = 100;
	
	private string logLine = string.Empty;
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0, 0, 200, screenButtonHeight), "Load Set"))
		{
			SoundCloudPlayer.Instance.LoadSet(false);
		}
		
//		GUI.TextArea(new Rect(0, Screen.height - 300, Screen.width, 300), logLine);
	}
	
	private void OnResolveCallback(SCServiceResponse response)
	{
		if(response.isSuccess) {
			Debug.Log(string.Format("DebugGUI.OnResolveCallback() - SUCCESS! {0}, streamable:{1}, from url:{2}",
				response.trackInfo.title,
				response.trackInfo.streamable,
				response.trackInfo.stream_url)
			);
		}
		else {
			Debug.Log("DebugGUI.OnResolveCallback(): " + response.errorMsg);
		}
	}
	
	private void OnLogCallback(string logMsg)
	{
		logLine = logMsg + "\n" + logLine;
	}
}
