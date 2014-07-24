using UnityEngine;
using System.Collections;
using System.IO;

public static class UniTunesGUITextFactory
{
	public static Texture2D playBtn;
	public static Texture2D stopBtn;
	public static Texture2D upBtn;
	public static Texture2D downBtn;
	public static Texture2D upFullBtn;
	public static Texture2D downFullBtn;
	public static Texture2D upDownBlank;
	public static Texture2D removeBtn;
	public static Texture2D soundcloudLogo;

	private static byte[] GetImageData(string imgPath)
	{
		FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
		byte[] imageData = new byte[fs.Length];
		fs.Read(imageData, 0, (int) fs.Length);

		return imageData;
	}

	public static void ClearTextures()
	{
		playBtn = null;
		stopBtn = null;
		upBtn = null;
		downBtn = null;
	}

	public static Texture2D GetRemoveBtnTexture()
	{
		if(removeBtn == null) {
			//do the loading
			removeBtn = new Texture2D(55, 55);
			removeBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_remove.png")));
		}
		
		return removeBtn;
	}

	public static Texture2D GetPlayBtnTexture()
	{
		if(playBtn == null) {
			//do the loading
			playBtn = new Texture2D(55, 55);
			playBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_play.png")));
		}

		return playBtn;
	}

	public static Texture2D GetStopBtnTexture()
	{
		if(stopBtn == null) {
			//do the loading
			stopBtn = new Texture2D(55, 55);
			stopBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_stop.png")));
		}
		
		return stopBtn;
	}

	public static Texture2D GetUpBtnTexture()
	{
		if(upBtn == null) {
			//do the loading
			upBtn = new Texture2D(55, 55);
			upBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_up.png")));
		}
		
		return upBtn;
	}

	public static Texture2D GetDownBtnTexture()
	{
		if(downBtn == null) {
			//do the loading
			downBtn = new Texture2D(55, 55);
			downBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_down.png")));
		}
		
		return downBtn;
	}

	public static Texture2D GetDownFullBtnTexture()
	{
		if(downFullBtn == null) {
			//do the loading
			downFullBtn = new Texture2D(55, 55);
			downFullBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_down_full.png")));
		}
		
		return downFullBtn;
	}

	public static Texture2D GetUpFullBtnTexture()
	{
		if(upFullBtn == null) {
			//do the loading
			upFullBtn = new Texture2D(55, 55);
			upFullBtn.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_up_full.png")));
		}
		
		return upFullBtn;
	}

	public static Texture2D GetUpDownBlankTexture()
	{
		if(upDownBlank == null) {
			//do the loading
			upDownBlank = new Texture2D(55, 55);
			upDownBlank.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "btn_up_down_blank.png")));
		}
		
		return upDownBlank;
	}

	public static Texture2D GetSoundCloudLogo()
	{
		if(soundcloudLogo == null) {
			//do the loading
			soundcloudLogo = new Texture2D(55, 55);
			soundcloudLogo.LoadImage(GetImageData(Path.Combine(UniTunesConsts.EDITOR_TEXTURE_PATH, "pb_soundcloud_logo.png")));
		}
		
		return soundcloudLogo;
	}
}
