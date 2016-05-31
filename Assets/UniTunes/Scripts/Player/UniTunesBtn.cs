using UnityEngine;
using System.Collections;

public class UniTunesBtn : MonoBehaviour
{
	public delegate void OnBtnPressed(string btnName);
	public static event OnBtnPressed OnBtnPressedEvt;

	private Camera _cam;

	void Awake()
	{
		foreach(Camera cam in Camera.allCameras) {
			if(cam.name == "SoundCloudPlayer") {
				_cam = cam;
			}
		}
	}

#if UNITY_EDITOR
	void Update()
	{
		if(Input.GetMouseButtonUp(0)) {
			Vector3 wp = _cam.ScreenToWorldPoint(Input.mousePosition);
			Vector2 touchPos = new Vector2(wp.x, wp.y);
			
			Collider2D c2d = Physics2D.OverlapPoint(touchPos);
			
			if(GetComponent<Collider2D>() == c2d) {
				OnBtnPressedEvt(name);
			}
		}
	}
#else
	void Update()
	{
		if (Input.touchCount == 1)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Ended) {
				Vector3 wp = _cam.ScreenToWorldPoint(Input.GetTouch(0).position);
				Vector2 touchPos = new Vector2(wp.x, wp.y);
				
				Collider2D c2d = Physics2D.OverlapPoint(touchPos);
				
				if(collider2D == c2d) {
					OnBtnPressedEvt(name);
				}
			}
		}
	}
#endif
}
