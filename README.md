UniTunes - v1.0
=========

UnitTunes enabled you to effortlessly build and playback soundcloud sets/playlist in your games. Please view our Toutube videos for a howto guide on setup, configuration and usage.

---

Notes
===

- UniTunes needs JsonFx to work, but don't be alarmed, it is opensource and inlcuded in the unitypackage. For reference purposes, the form in use can be found at [git@github.com:scopely/jsonfx-1.x.git](git@github.com:scopely/jsonfx-1.x.git)

---

Features planned for future releases
===
- Specify auto-play from editor
- Improve player UI feedback (button press effects)
- SoundCloud track search from editor window
- Support for other audio streaming services (Bandcamp, Last.fm, etc)
- Ability to upload audio to hosted service, for inclusion in set/playlist

Please vote on, or request features at XXX

---



Usage
===

1. Import the unitypackage
2. Build your playlist by providing SoundCloud
3. Export set Json Config (distribute with game or upload to your web server)
3. Add the SoundCloudPlayer Prefab to your scene
4. Set the preferred docking location for your player (selection on SoundCloudPlayer Prefab)
	- Top-Left 
	- Top-Centre
	- Top-Right
	- Bottom-Left
	- Bottom-Centre
	- Bottom-Right
4. Use the following API to control playback
	- __LoadSet(string configUrl, bool autoPlay)__
	  Use this method if you have your config file online and wish to specify the URL of hte Json Config
	- __LoadSet(bool autoPlay)__
	- __PlaySet()__
5. Additional API methods available
	- __PlayNext()__
	- __PlayFromIndex(int index)__
	- __StopPlayback()__
	- __MinimizePlayer()__
	- __MaximisePlayer()__
	- __GetTracks()__ (returns a List of tracks)

---

Change-list
===
- v1.0

  Very first version of UnitTunes with standard SoundCloud set creation and playback support


###TODO

http://unity3d.com/asset-store/sell-assets

- GUI player - test orientation changes
- test unitypackage addition to other games
- version, change-list, copyright
- youtube video (instructions)
- AssetStore graphics & submission http://unity3d.com/asset-store/sell-assets/submission-guidelines
- cleanup debug logs
- Add feature vote/request survey link to 'Features planned for future releases' section

---

###Done

- multiple audio sources issue
- detect track end (advance to next)
- progress to next track on track end
- handle url corruption (removal etc) - proceed to next
- loop checkbox value change detection with save to json
- screen orientation changes
- improved API access
- document API methods
- persist editor data on assetdatabase refresh
- add powered by soundcloud logo to editor (with link to soundcloud)
- add undo support
- load from json
- GUI player - text touch open soundcloud url
- GUI player - docking
- API play a song at specific index