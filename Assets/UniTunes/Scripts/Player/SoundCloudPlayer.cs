using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class SoundCloudPlayer : MonoSingleton<SoundCloudPlayer>
{
	private SCSet _scSet;
	private ISCPlayer _playerWdget;

	void Start()
	{
		LoadSet();
	}

	public void LoadSet()
	{
		_scSet = (SCSet) Resources.LoadAssetAtPath(UniTunesConsts.SC_CONFIG_PATH, typeof(SCSet));
		
		if(_scSet == null) {
			Debug.LogWarning("SoundCloudPlayer: failed to load SCSet");
			return;
		}

		//get the version expression
		Regex rgx = new Regex("[^0-9 . -]");
		string versionString = rgx.Replace(Application.unityVersion, "");
		Version version = new Version(versionString);

		//2D & sprites are not supported prior to 4.3...so we use UnityGUI in that case
		if(version.Major < 4 || (version.Major == 4 && version.Minor < 3)) {
//			_playerWdget = new GUISCPlayer();
			gameObject.AddComponent<GUISCPlayer>();
		}
		else {
//			_playerWdget = new SpriteSCPlayer();
			gameObject.AddComponent<SpriteSCPlayer>();
		}
	}
}
