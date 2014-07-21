using UnityEngine;
using System.Collections;

public class SCPlayerDocking : MonoBehaviour
{
	public Camera useCamera;

	private ScreenOrientation orientation;
	
	private Camera renderingCamera;
	private float zPosition;

	private Vector3 maxSize = Vector3.zero;

	public enum Docking
	{
		None,
		TopCentre,
		TopLeft,
		TopRight,
		BottomCentre,
		BottomLeft,
		BottomRight
	}
	
	void Awake ()
	{
		orientation = Screen.orientation;

		SetCamera();
		SetMaxSize();
		DockPlayer();
	}

	void Update()
	{
		if(Screen.orientation != orientation) {
			//orentation changed
			DockPlayer();
		}
	}

	private void SetCamera()
	{
		if(useCamera != null) {
			renderingCamera = useCamera;
			zPosition = -renderingCamera.transform.position.z + transform.position.z;
		}
		else {
			renderingCamera = Camera.main;
			zPosition = -renderingCamera.transform.position.z + transform.position.z;
		}
	}

	private void SetMaxSize()
	{
		foreach(Transform child in gameObject.transform) {
			Vector3 childBounds = child.renderer.bounds.size;

			if(childBounds.x > maxSize.x) {
				maxSize.x = childBounds.x;
			}
			
			if(childBounds.y > maxSize.y) {
				maxSize.y = childBounds.y;
			}
		}

		Vector3 zeroScreenPoint = renderingCamera.WorldToScreenPoint(Vector3.zero);
		Vector3 screenBounds = renderingCamera.WorldToScreenPoint(maxSize);

		maxSize.x = screenBounds.x - zeroScreenPoint.x;
		maxSize.y = screenBounds.y - zeroScreenPoint.y;
	}

	private void DockPlayer()
	{
		Vector3 screenPosition = Vector3.zero;

		switch (SoundCloudPlayer.Instance.widgetDocking) {
		case Docking.TopLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, zPosition));
			break;

		case Docking.TopCentre:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3((Screen.width/2) - (maxSize.x/2), Screen.height, zPosition));
			break;

		case Docking.TopRight:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(Screen.width - maxSize.x, Screen.height, zPosition));
			break;

		case Docking.BottomLeft:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(0, maxSize.y, zPosition));
			break;

		case Docking.BottomCentre:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3((Screen.width/2) - (maxSize.x/2), maxSize.y, zPosition));
			break;

		case Docking.BottomRight:
			screenPosition = renderingCamera.ScreenToWorldPoint(new Vector3(Screen.width - maxSize.x, maxSize.y, zPosition));
			break;

		default:
			break;
		}

		transform.position = screenPosition;
	}
}
