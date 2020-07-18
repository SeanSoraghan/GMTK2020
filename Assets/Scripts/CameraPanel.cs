using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPanel : MonoBehaviour
{
    public enum DisplayPosition : int
	{
        TopLeft = 0,
        TopRight,
        BottomLeft,
        BottomRight
	};

	public static DisplayPosition SwitchPositionHorizontal(DisplayPosition position)
	{
		int p = (int)position;
		int newPos = p + (p % 2 == 0 ? 1 : -1);
		if (newPos < 0)
			newPos += 4;
		else
			newPos = newPos % 4;
		return (DisplayPosition)newPos;
	}

	public static DisplayPosition SwitchPositionVertical(DisplayPosition position)
	{
		int p = (int)position;
		int newPos = p + (p % 2 == 0 ? 2 : -2);
		if (newPos < 0)
			newPos += 4;
		else
			newPos = newPos % 4;
		return (DisplayPosition)newPos;
	}

	public CubeController.ControlScheme controlScheme;
    public Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        cam = gameObject.GetComponent<Camera>();
        if (cam == null)
            cam = gameObject.AddComponent<Camera>();
        DisplayPosition camPosition = (CameraPanel.DisplayPosition)controlScheme;
        switch (camPosition)
        {
            case DisplayPosition.TopLeft:
                if (cam != null)
                    cam.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                break;
            case DisplayPosition.TopRight:
                if (cam != null)
                    cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                break;
            case DisplayPosition.BottomLeft:
                if (cam != null)
                    cam.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                break;
            case DisplayPosition.BottomRight:
                if (cam != null)
                    cam.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                break;
        }

        if (cam != null && cam.orthographic)
            cam.orthographicSize = (CubeController.WORLD_CUBE_LIMIT + 2) * (Screen.height / (float)Screen.width); // sizeToFillDisplay * aspect * 0.5. We want to fit 2 * (WORLD_CUBE_LIMIT + 1) on screen.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
