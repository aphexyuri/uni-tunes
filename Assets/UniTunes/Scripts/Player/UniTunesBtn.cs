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

	void Update()
	{
#if UNITY_EDITOR
		if(Input.GetMouseButtonUp(0)) {
#else
		if (Input.touchCount == 1) {
			if(Input.GetTouch(0).phase == TouchPhase.Ended) {
#endif
				Vector3 touchPos = _cam.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(touchPos, transform.position);

				if(hit.collider != null && hit.collider == collider2D) {
					OnBtnPressedEvt(name);
				}
#if !UNITY_EDITOR
			}
#endif
		}
	}
}
