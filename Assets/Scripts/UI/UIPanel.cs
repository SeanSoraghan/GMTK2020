using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
	public enum MovementDirection
	{
		Up = 0,
		Down,
		Left,
		Right
	};

    public float panelThickness = 20.0f;
    public float sideLengthRatio = 0.75f;
    public float panelAnimationTimeSeconds = 0.2f;
    public GUISkin guiSkin;

    CameraPanel.DisplayPosition displayPos = CameraPanel.DisplayPosition.TopLeft;
    Vector2 panelFrameCentre;
	Vector2 targetPos;
    Vector2 _movementTarget;
    Vector2 movementTarget
    {
        get
        {
            return _movementTarget;
        }
        set
        {
            _movementTarget = value;
            lerpStartTime = Time.time;
        }
    }
    Vector2 movementStartPos;
    float lerpX;
    float lerpStartTime;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(guiSkin);
    }

    public void PositionPanel(CameraPanel.DisplayPosition displayPosition, MovementDirection movementDirection)
    {
        displayPos = displayPosition;
        UpdateDisplay(movementDirection, false);
    }

	public void PositionPanelImmediate(CameraPanel.DisplayPosition displayPosition)
	{
		displayPos = displayPosition;
		UpdateDisplay(MovementDirection.Up/* <- Doesnt matter, since we're teleporting the panel */, true);
	}

	public void TeleportPanel()
	{
        panelFrameCentre = targetPos;
	}

    void UpdateDisplay(MovementDirection direction, bool teleport)
	{
		GetTargetPosForCornersLayout();
		movementTarget = targetPos;
		if (direction == MovementDirection.Up && panelFrameCentre.y == 0.25f)
		{
			movementTarget = new Vector2(movementTarget.x, -0.25f);
		}
		if (direction == MovementDirection.Down && panelFrameCentre.y == 0.75f)
		{
			movementTarget = new Vector2(movementTarget.x, 1.25f);
		}
		if (direction == MovementDirection.Left && panelFrameCentre.x == 0.25f)
		{
			movementTarget = new Vector2(-0.25f, movementTarget.y);
		}
		if (direction == MovementDirection.Right && panelFrameCentre.x == 0.75f)
		{
			movementTarget = new Vector2(1.25f, movementTarget.y);
		}
		if (teleport)
        {
            panelFrameCentre = movementTarget;
        }
        movementStartPos = panelFrameCentre;
    }

	void GetTargetPosForCornersLayout()
	{
		float marginW = CameraPanel.widthMargin * 0.5f;
		float marginH = CameraPanel.heightMargin * 0.5f;
		switch (displayPos)
		{
			case CameraPanel.DisplayPosition.BottomLeft:
				targetPos = new Vector2(CameraPanel.normedWidth * 0.5f, 1.0f - CameraPanel.normedHeight * 0.5f);
				break;
			case CameraPanel.DisplayPosition.BottomRight:
				targetPos = new Vector2(1.0f - CameraPanel.normedWidth * 0.5f, 1.0f - CameraPanel.normedHeight * 0.5f);
				break;
			case CameraPanel.DisplayPosition.TopLeft:
				targetPos = new Vector2(CameraPanel.normedWidth * 0.5f, CameraPanel.normedHeight * 0.5f);
				break;
			case CameraPanel.DisplayPosition.TopRight:
				targetPos = new Vector2(1.0f - CameraPanel.normedWidth * 0.5f, CameraPanel.normedHeight * 0.5f);
				break;
		}
	}

	void GetTargetPosForCentredLayout()
	{
		float marginW = CameraPanel.widthMargin * 0.5f;
		float marginH = CameraPanel.heightMargin * 0.5f;
		switch (displayPos)
		{
			case CameraPanel.DisplayPosition.BottomLeft:
				targetPos = new Vector2(0.25f + marginW, 0.75f - marginH);
				break;
			case CameraPanel.DisplayPosition.BottomRight:
				targetPos = new Vector2(0.75f - marginW, 0.75f - marginH);
				break;
			case CameraPanel.DisplayPosition.TopLeft:
				targetPos = new Vector2(0.25f + marginW, 0.25f + marginH);
				break;
			case CameraPanel.DisplayPosition.TopRight:
				targetPos = new Vector2(0.75f - marginW, 0.25f + marginH);
				break;
		}
	}

    void OnGUI()
    {
        if (panelFrameCentre != targetPos)
        {
            lerpX = (Time.time - lerpStartTime) / panelAnimationTimeSeconds;
            panelFrameCentre = Vector2.Lerp(movementStartPos, movementTarget, lerpX);
            if (Vector2.Distance(panelFrameCentre, movementTarget) < 0.05f)
            {
				panelFrameCentre = targetPos;
            }
        }
        DrawPanel();
    }

    void DrawPanel()
    {
        if (guiSkin != null)
            GUI.skin = guiSkin;

		DrawPanelWithFrameCenter(panelFrameCentre);
		Vector2 virtualCentre = panelFrameCentre;
		if (panelFrameCentre.x > 0.75f)
		{
			virtualCentre.x = -(1.0f - panelFrameCentre.x);
		}
		else if (panelFrameCentre.x < 0.25f)
		{
			virtualCentre.x = (1.0f + panelFrameCentre.x);
		}
		if (panelFrameCentre.y > 0.75f)
		{
			virtualCentre.y = -(1.0f - panelFrameCentre.y);
		}
		else if (panelFrameCentre.y < 0.25f)
		{
			virtualCentre.y = (1.0f + panelFrameCentre.y);
		}
		if (virtualCentre != panelFrameCentre)
			DrawPanelWithFrameCenter(virtualCentre);
    }

	void DrawPanelWithFrameCenter(Vector2 frameCenter)
	{
		float w = Screen.width;
		float h = Screen.height;
		Vector2 centre = frameCenter * new Vector2(w, h);

		float sideLength = CameraPanel.sideLength;
		float length = sideLengthRatio * sideLength;
		float halfSide = sideLength * 0.5f;
		float offset = (1.0f - sideLengthRatio) * halfSide;

		Vector2 leftBarXY = new Vector2(centre.x - halfSide, centre.y - halfSide);
		GUI.Box(new Rect(leftBarXY.x, leftBarXY.y + offset, panelThickness, length), "");

		Vector2 rightBarXY = new Vector2(centre.x + halfSide - panelThickness, centre.y - halfSide);
		GUI.Box(new Rect(rightBarXY.x, rightBarXY.y + offset, panelThickness, length), "");

		//fullLength = w * 0.5f;
		//length = panelHeightRatio * fullLength;
		//offset = (1.0f - panelHeightRatio) * fullLength * 0.5f;

		Vector2 topBarXY = new Vector2(centre.x - halfSide, centre.y - halfSide);
		GUI.Box(new Rect(topBarXY.x + offset, topBarXY.y, length, panelThickness), "");

		Vector2 bottomBarXY = new Vector2(centre.x - halfSide, centre.y + halfSide - panelThickness);
		GUI.Box(new Rect(bottomBarXY.x + offset, bottomBarXY.y, length, panelThickness), "");
	}
}