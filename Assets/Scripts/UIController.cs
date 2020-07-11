using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    RectTransform imageRect;

    // Start is called before the first frame update
    void Start()
    {
        imageRect = GetComponent<RectTransform>();
        Assert.IsNotNull(imageRect);
        float w = Screen.width;
        float h = Screen.height;
        imageRect.sizeDelta = new Vector2(0.5f * w, 0.5f * h);
        //PositionPanelUI();
    }

    public void PositionPanelUI(CameraPanel.DisplayPosition displayPosition)
	{
        float w = Screen.width;
        float h = Screen.height;
        switch(displayPosition)
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
