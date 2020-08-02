using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewCameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		switch (LevelController.layout)
		{
			case LevelController.LayoutMode.CentredPanels:
				transform.position = new Vector3(0.0f, 0.0f, 3.0f) * LevelController.WORLD_CUBE_LIMIT;
				break;
			case LevelController.LayoutMode.PerspectiveCentre:
				transform.position = new Vector3(5.0f, 8.5f, -5.0f) * LevelController.WORLD_CUBE_LIMIT;
				transform.rotation = Quaternion.Euler(50.0f, -45.0f, 0.0f);
				break;
		}
    }
}
