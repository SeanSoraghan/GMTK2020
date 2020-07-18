using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIController2 : MonoBehaviour
{
    public float panelThickness = 20.0f;
    public float panelHeightRatio = 0.75f;
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
        UpdateDisplay(CubeController.MovementState.MovingUp/*<- doesnt matter, since we're teleporting*/, true);
    }

    public void PositionPanelUI(CameraPanel.DisplayPosition displayPosition, CubeController.MovementState movementDirection)
    {
        displayPos = displayPosition;
        UpdateDisplay(movementDirection, false);
    }

	public void TeleportPanel()
	{
        panelFrameCentre = targetPos;
	}

    void UpdateDisplay(CubeController.MovementState movementDirection, bool teleport)
	{
        switch (displayPos)
        {
            case CameraPanel.DisplayPosition.BottomLeft:
				targetPos = new Vector2(0.25f, 0.75f);
                break;
            case CameraPanel.DisplayPosition.BottomRight:
				targetPos = new Vector2(0.75f, 0.75f);
                break;
            case CameraPanel.DisplayPosition.TopLeft:
				targetPos = new Vector2(0.25f, 0.25f);
                break;
            case CameraPanel.DisplayPosition.TopRight:
				targetPos = new Vector2(0.75f, 0.25f);
                break;
        }
		movementTarget = targetPos;
		if (movementDirection == CubeController.MovementState.MovingUp && panelFrameCentre.y == 0.25f)
		{
			movementTarget = new Vector2(movementTarget.x, -0.25f);
		}
		if (movementDirection == CubeController.MovementState.MovingDown && panelFrameCentre.y == 0.75f)
		{
			movementTarget = new Vector2(movementTarget.x, 1.25f);
		}
		if (movementDirection == CubeController.MovementState.MovingLeft && panelFrameCentre.x == 0.25f)
		{
			movementTarget = new Vector2(-0.25f, movementTarget.y);
		}
		if (movementDirection == CubeController.MovementState.MovingRight && panelFrameCentre.x == 0.75f)
		{
			movementTarget = new Vector2(1.25f, movementTarget.y);
		}
		if (teleport)
        {
            panelFrameCentre = movementTarget;
        }
        movementStartPos = panelFrameCentre;
    }

    void OnGUI()
    {
        if (panelFrameCentre != targetPos)
        {
            lerpX = (Time.time - lerpStartTime) / panelAnimationTimeSeconds;
            panelFrameCentre = Vector2.Lerp(movementStartPos, movementTarget, lerpX);
            if (Vector2.Distance(panelFrameCentre, movementTarget) < 0.05f)
            {
				panelFrameCentre = targetPos;// movementTarget;
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

		float fullLength = h * 0.5f;
		float length = panelHeightRatio * fullLength;
		float offset = (1.0f - panelHeightRatio) * fullLength * 0.5f;

		Vector2 leftBarXY = new Vector2(centre.x - 0.25f * w, centre.y - 0.25f * h);
		GUI.Box(new Rect(leftBarXY.x, leftBarXY.y + offset, panelThickness, length), "");

		Vector2 rightBarXY = new Vector2(centre.x + 0.25f * w - panelThickness, centre.y - 0.25f * h);
		GUI.Box(new Rect(rightBarXY.x, rightBarXY.y + offset, panelThickness, length), "");

		fullLength = w * 0.5f;
		length = panelHeightRatio * fullLength;
		offset = (1.0f - panelHeightRatio) * fullLength * 0.5f;

		Vector2 topBarXY = new Vector2(centre.x - 0.25f * w, centre.y - 0.25f * h);
		GUI.Box(new Rect(topBarXY.x + offset, topBarXY.y, length, panelThickness), "");

		Vector2 bottomBarXY = new Vector2(centre.x - 0.25f * w, centre.y + 0.25f * h - panelThickness);
		GUI.Box(new Rect(bottomBarXY.x + offset, bottomBarXY.y, length, panelThickness), "");
	}
}