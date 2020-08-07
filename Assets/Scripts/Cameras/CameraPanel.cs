using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanel : MonoBehaviour
{
    public enum DisplayPosition : int
	{
        TopLeft = 0,
        TopRight,
        BottomLeft,
        BottomRight,
		NumPositions
	};

	public static DisplayPosition SwitchPositionHorizontal(DisplayPosition position)
	{
		int p = (int)position;
		int newPos = p + (p % 2 == 0 ? 1 : -1);
		if (newPos < 0)
			newPos += (int)DisplayPosition.NumPositions;
		else
			newPos = newPos % (int)DisplayPosition.NumPositions;
		return (DisplayPosition)newPos;
	}

	public static DisplayPosition SwitchPositionVertical(DisplayPosition position)
	{
		int p = (int)position;
		int newPos = p + (p % 2 == 0 ? 2 : -2);
		if (newPos < 0)
			newPos += (int)DisplayPosition.NumPositions;
		else
			newPos = newPos % (int)DisplayPosition.NumPositions;
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
	static bool dimensionsSetup = false;

	bool layoutInitialized = false;
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
    void Start()
    {
		switch (LevelController.layout)
		{
			case LayoutMode.CentredPanels:
				SetupCentred();
				break;
			case LayoutMode.PerspectiveCentre:
				SetupCorners();
				break;
		}
	}

	public void PostActivate()
	{
		switch (LevelController.layout)
		{
			case LayoutMode.CentredPanels:
				SetupCentred();
				break;
			case LayoutMode.PerspectiveCentre:
				SetupCorners();
				break;
		}
	}

	static void SetupDimensionsCorners()
	{
		if (!dimensionsSetup)
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

			dimensionsSetup = true;
		}
	}

	void SetupCorners()
	{
		SetupDimensionsCorners();
		if (!layoutInitialized)
		{
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
			layoutInitialized = true;
		}
	}

	void SetupDimensionsCentred()
	{
		if (!dimensionsSetup)
		{
			float w = Screen.width;
			float h = Screen.height;

			float minDimension = w < h ? w : h;
			sideLength = 0.5f * minDimension;
			normedWidth = minDimension == w ? 0.5f : (sideLength / w);
			normedHeight = minDimension == h ? 0.5f : (sideLength / h);
			widthMargin = 0.5f - normedWidth;
			heightMargin = 0.5f - normedHeight;

			dimensionsSetup = true;
		}
	}

	void SetupCentred()
	{
		SetupDimensionsCentred();
		if (!layoutInitialized)
		{
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
			layoutInitialized = true;
		}
	}
}
