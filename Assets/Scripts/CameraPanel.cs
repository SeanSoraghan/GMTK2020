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

    public DisplayPosition camPosition;

    public Camera cam;

    // Start is called before the first frame update
    void Awake()
    {
        if (cam == null)
            cam = gameObject.AddComponent<Camera>();

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
