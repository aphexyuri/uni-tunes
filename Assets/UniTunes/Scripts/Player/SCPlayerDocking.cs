using UnityEngine;
using System.Collections;

public class SCPlayerDocking : MonoBehaviour
{
	private ScreenOrientation orientation;
	
	private Camera renderingCamera;
	private float zPosition;

	private Vector2 playerSize = Vector2.zero;
	private Vector3 maxSize = Vector3.zero;

	public enum Docking
	{
		None,
		TopLeft,
		TopCentre,
		TopRight,
		BottomLeft,
		BottomCentre,
		BottomRight
	}
	
	void Awake ()
	{
		orientation = Screen.orientation;

		SetCamera();
		SetPlayerSize();

		DockPlayer();
	}

	void Update()
	{
		if(Screen.orientation != orientation) {
			orientation = Screen.orientation;

			DockPlayer();
			StartCoroutine(OrientationChageDelay());
		}
	}

	//hack to as docking calculations happened too fast, before orientation config commit
	private IEnumerator OrientationChageDelay()
	{
		yield return new WaitForSeconds(0.3f);

		DockPlayer();
	}

	private void SetCamera()
	{
		foreach(Camera cam in Camera.allCameras) {
			if(cam.name == "SoundCloudPlayer") {
				renderingCamera = cam;
				zPosition = -renderingCamera.transform.position.z + transform.position.z;
				return;
			}
		}

		renderingCamera = Camera.main;
	}

	private void SetPlayerSize()
	{
		foreach(Transform child in gameObject.transform) {
			Vector3 childBounds = child.renderer.bounds.size;
			
			if(childBounds.x > playerSize.x) {
				playerSize.x = childBounds.x;
			}
			
			if(childBounds.y > playerSize.y) {
				playerSize.y = childBounds.y;
			}
		}
	}

	private float GetViewWidth()
	{
//		return renderingCamera.pixelWidth;
		return Screen.width;
	}

	private float GetViewHeight()
	{
//		return renderingCamera.pixelHeight;
		return Screen.height;
	}

	private void DockPlayer()
	{
		Vector3 zeroScreenPoint = renderingCamera.WorldToScreenPoint(Vector3.zero);
		Vector3 screenBounds = renderingCamera.WorldToScreenPoint(playerSize);
		
		maxSize.x = screenBounds.x - zeroScreenPoint.x;
		maxSize.y = screenBounds.y - zeroScreenPoint.y;

		Vector3 screenPosition = Vector3.zero;

		switch (SoundCloudPlayer.Instance.widgetDocking) {
		case Docking.TopLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(0, GetViewHeight(), zPosition));
			break;

		case Docking.TopCentre:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3((GetViewWidth()/2) - (maxSize.x/2), GetViewHeight(), zPosition));
			break;

		case Docking.TopRight:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(GetViewWidth() - maxSize.x, GetViewHeight(), zPosition));
			break;

		case Docking.BottomLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(0, maxSize.y, zPosition));
			break;

		case Docking.BottomCentre:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3((GetViewWidth()/2) - (maxSize.x/2), maxSize.y, zPosition));
			break;

		case Docking.BottomRight:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(GetViewWidth() - maxSize.x, maxSize.y, zPosition));
			break;

		default:
			break;
		}

		transform.localPosition = screenPosition;
	}
}
