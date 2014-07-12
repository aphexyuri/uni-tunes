using System.Collections.Generic;

public class SoundCloudServiceUtils
{
	public static string AppendClientId(string baseUrl)
	{
		if(baseUrl.EndsWith("?")) {
			return string.Concat(baseUrl, "client_id=", SoundCloudService.CLIENT_ID);
		}
		else {
			if(baseUrl.Contains("?")) {
				return string.Concat(baseUrl, "&client_id=", SoundCloudService.CLIENT_ID);
			}
			return string.Concat(baseUrl, "?client_id=", SoundCloudService.CLIENT_ID);
		}
	}
	
	public static string BuildEndpoint(string scApiMethod, Dictionary<string, string> urlVars, bool appendClientId)
	{
		string endpoint = SoundCloudService.SOUNDCLOUD_API;
		
		if(!string.IsNullOrEmpty(scApiMethod)) {
			endpoint = endpoint + scApiMethod;
		}
		
		//return if not url vars provided
		if(urlVars == null) { return endpoint; }
		
		endpoint = endpoint + "?";
		
		//append url vars
		foreach(KeyValuePair<string, string> kvp in urlVars) {
			if(!endpoint.EndsWith("?")) {
				endpoint = string.Concat(endpoint, "&");
			} 
			endpoint  = string.Concat(endpoint, kvp.Key, "=", kvp.Value);
		}
		
		//append client_id
		if(appendClientId) {
			endpoint = AppendClientId(endpoint);
		}
		
		return endpoint;
	}
}
