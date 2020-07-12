using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIController2 : MonoBehaviour
{
    public float panelThickness = 20.0f;
    public float panelAnimationTimeSeconds = 0.2f;
    public GUISkin guiSkin;

    CameraPanel.DisplayPosition displayPos = CameraPanel.DisplayPosition.TopLeft;
    Vector2 panelFrameCentre;
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
        UpdateDisplay(true);
    }

    public void PositionPanelUI(CameraPanel.DisplayPosition displayPosition)
    {
        displayPos = displayPosition;
        UpdateDisplay(false);
    }

    public void TeleportPanel()
	{
        panelFrameCentre = movementTarget;
	}

    void UpdateDisplay(bool teleport)
	{
        switch (displayPos)
        {
            case CameraPanel.DisplayPosition.BottomLeft:
                movementTarget = new Vector2(0.25f, 0.75f);
                break;
            case CameraPanel.DisplayPosition.BottomRight:
                movementTarget = new Vector2(0.75f, 0.75f);
                break;
            case CameraPanel.DisplayPosition.TopLeft:
                movementTarget = new Vector2(0.25f, 0.25f);
                break;
            case CameraPanel.DisplayPosition.TopRight:
                movementTarget = new Vector2(0.75f, 0.25f);
                break;
        }
        if (teleport)
        {
            panelFrameCentre = movementTarget;
        }
        movementStartPos = panelFrameCentre;
    }

    void OnGUI()
    {
        if (panelFrameCentre != movementTarget)
        {
            lerpX = (Time.time - lerpStartTime) / panelAnimationTimeSeconds;
            panelFrameCentre = Vector2.Lerp(movementStartPos, movementTarget, lerpX);
            if (Vector2.Distance(panelFrameCentre, movementTarget) < 0.05f)
            {
                panelFrameCentre = movementTarget;
            }
        }
        DrawPanel();
    }

    void DrawPanel()
    {
        if (guiSkin != null)
            GUI.skin = guiSkin;

        float w = Screen.width;
        float h = Screen.height;
        Vector2 centre = panelFrameCentre * new Vector2(w, h);

        Vector2 leftBarXY = new Vector2(centre.x - 0.25f * w, centre.y - 0.25f * h);
        GUI.Box(new Rect(leftBarXY.x, leftBarXY.y, panelThickness, h * 0.5f), "");

        Vector2 rightBarXY = new Vector2(centre.x + 0.25f * w - panelThickness, centre.y - 0.25f * h);
        GUI.Box(new Rect(rightBarXY.x, rightBarXY.y, panelThickness, h * 0.5f), "");

        Vector2 topBarXY = new Vector2(centre.x - 0.25f * w, centre.y - 0.25f * h);
        GUI.Box(new Rect(topBarXY.x, topBarXY.y, 0.5f * w, panelThickness), "");

        Vector2 bottomBarXY = new Vector2(centre.x - 0.25f * w, centre.y + 0.25f * h - panelThickness);
        GUI.Box(new Rect(bottomBarXY.x, bottomBarXY.y, 0.5f * w, panelThickness), "");
    }
}