using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		if (LevelController.Instance != null)
		{
			GetComponent<Camera>().fieldOfView = LevelController.Instance.worldSettings.BackgroundCamFOV;
		}
		switch (LevelController.layout)
		{
			case LayoutMode.CentredPanels:
				transform.position = new Vector3(0.0f, 0.0f, 3.0f) * LevelController.Instance.worldSettings.worldExtent;
				break;
			case LayoutMode.PerspectiveCentre:
				transform.position = new Vector3(5.0f, 8.5f, -5.0f) * LevelController.Instance.worldSettings.worldExtent;
				transform.rotation = Quaternion.Euler(50.0f, -45.0f, 0.0f);
				break;
		}
    }
}
