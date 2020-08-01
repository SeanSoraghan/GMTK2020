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

	public static float sideLengthRatio = 1.0f;

    public Camera cam;
	public DisplayPosition camPosition;

	public static float widthMargin { get; private set; }
	public static float heightMargin { get; private set; }
	public static float normedWidth { get; private set; }
	public static float normedHeight { get; private set; }
	public static float sideLength { get; private set; }
	public static float normedSideLength() { return Mathf.Min(normedHeight, normedWidth); }
	bool _isInControl = false;
	public bool IsSelected
	{
		get
		{
			return _isInControl;
		}
		set
		{
			_isInControl = value;
		}
	}

	

    // Start is called before the first frame update
    void Awake()
    {
		switch (LevelController.layout)
		{
			case LevelController.LayoutMode.CentredPanels:
				SetupCentred();
				break;
			case LevelController.LayoutMode.PerspectiveCentre:
				SetupCorners();
				break;
		}

        if (cam != null && cam.orthographic)
            cam.orthographicSize = (LevelController.WORLD_CUBE_LIMIT + 2) * (Screen.height / (float)Screen.width); // sizeToFillDisplay * aspect * 0.5. We want to fit 2 * (WORLD_CUBE_LIMIT + 1) on screen.
	}

	void SetupCorners()
	{
		float w = Screen.width;
		float h = Screen.height;

		float minDimension = w < h ? w : h;
		float maxNormedLength = 0.5f * sideLengthRatio;
		sideLength = maxNormedLength * minDimension;
		normedWidth = minDimension == w ? maxNormedLength : (sideLength / w);
		normedHeight = minDimension == h ? maxNormedLength : (sideLength / h);
		widthMargin = 0.5f - normedWidth;
		heightMargin = 0.5f - normedHeight;

		cam = gameObject.GetComponent<Camera>();
		if (cam == null)
			cam = gameObject.AddComponent<Camera>();
		switch (camPosition)
		{
			case DisplayPosition.TopLeft:
				if (cam != null)
					cam.rect = new Rect(0.0f, 0.5f + heightMargin, normedWidth, normedHeight);
				break;
			case DisplayPosition.TopRight:
				if (cam != null)
					cam.rect = new Rect(0.5f + widthMargin, 0.5f + heightMargin, normedWidth, normedHeight);
				break;
			case DisplayPosition.BottomLeft:
				if (cam != null)
					cam.rect = new Rect(0.0f, 0.0f, normedWidth, normedHeight);
				break;
			case DisplayPosition.BottomRight:
				if (cam != null)
					cam.rect = new Rect(0.5f + widthMargin, 0.0f, normedWidth, normedHeight);
				break;
		}
	}

	void SetupCentred()
	{
		float w = Screen.width;
		float h = Screen.height;

		float minDimension = w < h ? w : h;
		sideLength = 0.5f * minDimension;
		normedWidth = minDimension == w ? 0.5f : (sideLength / w);
		normedHeight = minDimension == h ? 0.5f : (sideLength / h);
		widthMargin = 0.5f - normedWidth;
		heightMargin = 0.5f - normedHeight;

		cam = gameObject.GetComponent<Camera>();
		if (cam == null)
			cam = gameObject.AddComponent<Camera>();
		switch (camPosition)
		{
			case DisplayPosition.TopLeft:
				if (cam != null)
					cam.rect = new Rect(widthMargin, 0.5f, normedWidth, normedHeight);
				break;
			case DisplayPosition.TopRight:
				if (cam != null)
					cam.rect = new Rect(0.5f, 0.5f, normedWidth, normedHeight);
				break;
			case DisplayPosition.BottomLeft:
				if (cam != null)
					cam.rect = new Rect(widthMargin, heightMargin, normedWidth, normedHeight);
				break;
			case DisplayPosition.BottomRight:
				if (cam != null)
					cam.rect = new Rect(0.5f, heightMargin, normedWidth, normedHeight);
				break;
		}
	}
}
