UniTunes - v1.0
=========

UnitTunes enabled you to effortlessly build and playback of soundcloud sets/playlist in your games.

---

Features planned for future releases
===
- Specify auto-play from editor
- Improve player UI feedback (button press effects)
- SoundCloud track search from editor window
- Support for other audio streaming services (Bandcamp, Last.fm, etc)
- Ability to upload audio to hosted service, for inclusion in set/playlist

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