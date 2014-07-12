using UnityEngine;

public class SCServiceResponse : ISCResponse
{
	private bool _success = false;
	private string _errorMsg = string.Empty;
	
	private SoundCloudTrack _trackInfo;
	
	public SCServiceResponse(bool success, string eMsg, SoundCloudTrack track)
	{
		_success = success;
		_errorMsg = eMsg;
		
		if(track != null) {
			_trackInfo = track;
		}
	}
	
	public SCServiceResponse(bool success, string eMsg)
	{
		_success = success;
		_errorMsg = eMsg;
	}
	
	public bool isSuccess {
		get { return _success; }
	}
	
	public string errorMsg {
		get { return _errorMsg; }
	}
	
	public SoundCloudTrack trackInfo {
		get { return _trackInfo; }
	}
}

//public class SCServiceResponse<T> : SCServiceResponse
//{
//		public SCServiceResponse(bool success, string eMsg)
//	{
//		base.isSuccess = success;
//		base.errorMsg = eMsg;
//	}
//}

//public class SCResolveResponse : ISCResponse
//{
		
//}

//public class SoundCloudServiceResponse<T> : SoundCloudServiceResponse
//{
//	public SoundCloudServiceResponse(bool success, string eMsg)
//	{
//		base.isSuccess = success;
//		base.errorMsg = eMsg;
//	}
//}
