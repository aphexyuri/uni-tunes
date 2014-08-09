UniTunes - v1.0
=========

UnitTunes enabled you to effortlessly build and playback soundcloud sets/playlist in your games. Please view our Youtube videos for a howto guide on setup, configuration and usage.

---

Notes
===

- UniTunes needs JsonFx to work, but don't be alarmed, it is opensource and included in the unitypackage. For reference purposes, the form in use can be found at https://github.com/scopely/jsonfx-1.x (git@github.com:scopely/jsonfx-1.x.git)
- Currently, the SoundCloudPlayer uses OnGUI for rendering the player UI - a future updated will include Unity GUI support
- Please submit any feedback or feature request at https://docs.google.com/forms/d/1y4yQQXLmAG5ZCZC7PQxCBceTxLcOdMbDNsHkYNGtcqg/viewform?usp=send_form

---

Features planned for future releases
===
- Unity GUI Player (after Unity 4.6 release)
- Specify auto-play from editor
- Improve player UI feedback (button press effects)
- SoundCloud track search from editor window
- Support for other audio streaming services (Bandcamp, Last.fm, etc)
- Various player sizes

Please vote on, or request features at XXX

---

Usage
===

1. Import the unitypackage
2. Open the SoundCloud set editor from "Window" > "UniTunes" > "SoundCloud Set Editor"
3. Use the SoundCloud Set Editor to create your playlist, by providing SoundCloud URLS clicking "Validate & Add"
4. Export set Json Config (distribute with game or upload to your web server)
5. Add the SoundCloudPlayer Prefab to your scene
6. Set the preferred docking location for your player (selection on SoundCloudPlayer Prefab)
	- Top-Left 
	- Top-Centre
	- Top-Right
	- Bottom-Left
	- Bottom-Centre
	- Bottom-Right
7. Use the following API to control playback
	- __LoadSet(string configUrl, bool autoPlay)__
	  Use this method if you have your config file online and wish to specify the URL of the Json Config
	- __LoadSet(bool autoPlay)__
	- __PlaySet()__
8. Additional API methods available
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

- move events not working on game (slotsvacation)
- youtube video (instructions)
- AssetStore graphics & submission http://unity3d.com/asset-store/sell-assets/submission-guidelines

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
- test unitypackage addition to other games
- GUI player - test orientation changes
- cleanup debug logs
- Add feature vote/request survey link to 'Features planned for future releases' section