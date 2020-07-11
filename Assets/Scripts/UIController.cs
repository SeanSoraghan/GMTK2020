using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIController : MonoBehaviour
{
    RectTransform imageRect;
    CameraPanel.DisplayPosition displayPos = CameraPanel.DisplayPosition.TopLeft;

    // Start is called before the first frame update
    void Awake()
    {
        imageRect = GetComponent<RectTransform>();
        Assert.IsNotNull(imageRect);
        float w = Screen.width;
        float h = Screen.height;
        imageRect.sizeDelta = new Vector2(0.5f * w, 0.5f * h);
        UpdateDisplay();
    }

    public void PositionPanelUI(CameraPanel.DisplayPosition displayPosition)
	{
        displayPos = displayPosition;
        UpdateDisplay();
	}

    void UpdateDisplay()
	{
        if (imageRect != null)
        {
            float w = Screen.width;
            float h = Screen.height;
            switch (displayPos)
            {
                case CameraPanel.DisplayPosition.TopLeft:
                    imageRect.anchoredPosition = new Vector2(-0.25f * w, 0.25f * h);
                    break;
                case CameraPanel.DisplayPosition.TopRight:
                    imageRect.anchoredPosition = new Vector2(0.25f * w, 0.25f * h);
                    break;
                case CameraPanel.DisplayPosition.BottomLeft:
                    imageRect.anchoredPosition = new Vector2(-0.25f * w, -0.25f * h);
                    break;
                case CameraPanel.DisplayPosition.BottomRight:
                    imageRect.anchoredPosition = new Vector2(0.25f * w, -0.25f * h);
                    break;
                default: break;
            }
        }
    }
}
