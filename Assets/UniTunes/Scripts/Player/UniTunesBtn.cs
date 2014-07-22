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
			Vector3 touchPos = _cam.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(touchPos, transform.position);

			if(hit.collider != null && hit.collider == collider2D) {
				OnBtnPressedEvt(name);
			}
		}
	}
#endif

#if !UNITY_EDITOR
	void Update()
	{
		if (Input.touchCount == 1)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Ended) {
				Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
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
