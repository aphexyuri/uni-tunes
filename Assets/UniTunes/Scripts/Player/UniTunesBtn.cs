using UnityEngine;
using System.Collections;

public class UniTunesBtn : MonoBehaviour
{
	public delegate void OnBtnPressed(string btnName);
	public static event OnBtnPressed OnBtnPressedEvt;

#if UNITY_EDITOR
	void OnMouseUpAsButton()
	{
		OnBtnPressedEvt(name);
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
