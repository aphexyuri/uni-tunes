UniTunes - v1.0
=========

UnitTunes enabled you to effortlessly build and playback soundcloud sets/playlist in your games. Please view our Youtube videos for a howto guide on setup, configuration and usage.

---

Notes
===

- UniTunes needs JsonFx to work, but don't be alarmed, it is opensource and included in the unitypackage. For reference purposes, the form in use can be found at [git@github.com:scopely/jsonfx-1.x.git](git@github.com:scopely/jsonfx-1.x.git)
- If using Unity 4.3 +, the SoundCloud Player will be constructed with 2D Sprites. IF you are using a Unity version prior to 4.3, the SoundCloud Player will use Unity OnGUI

---

Features planned for future releases
===
- Specify auto-play from editor
- Improve player UI feedback (button press effects)
- SoundCloud track search from editor window
- Support for other audio streaming services (Bandcamp, Last.fm, etc)
- Ability to upload audio to hosted service, for inclusion in set/playlist
- Support for Unity GUI

Please vote on, or request features at XXX

---

Usage
===

1. Import the unitypackage
2. Open the SoundCloud set editor from "Window" > "UniTunes" > "SoundCloud Set Editor"
3. Use the SoundCloud Set Editor to create your playlist, by providing SoundCloud URLS clicking "Validate & Add"
4. Export set Json Config (distribute with game or upload to your web server)
5. Add the SoundCloudPlayer Prefab to your scene
6. If you are using Unity 4.3 +, configure your layers
	1. Create a layer named "SoundCloudPlayer"
	2. Change the layer of the SoundCloudPlayer prefab you added to the scene, to be this new layer
	3. Change the Culling Mask of the Camera component in SoundCloudPlayer to be this new layer
7. Set the preferred docking location for your player (selection on SoundCloudPlayer Prefab)
	- Top-Left 
	- Top-Centre
	- Top-Right
	- Bottom-Left
	- Bottom-Centre
	- Bottom-Right
8. Use the following API to control playback
	- __LoadSet(string configUrl, bool autoPlay)__
	  Use this method if you have your config file online and wish to specify the URL of the Json Config
	- __LoadSet(bool autoPlay)__
	- __PlaySet()__
9. Additional API methods available
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