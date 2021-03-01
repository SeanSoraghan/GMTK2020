using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class LabelPopOutController : MonoBehaviour
{
    public enum AnimationState
	{
        Expanding,
        Idle,
        Retracting
	}

    public enum PopoutHorizontal
	{
        Left,
        Right
	}

    public enum PopoutVertical
	{
        Up,
        Down
	}

    public Vector2 popOutFrom = Vector2.zero;
    public PopoutHorizontal horizontalDirection = PopoutHorizontal.Left;
    public PopoutVertical verticalDirection = PopoutVertical.Up;
    public float popoutTimeSeconds = 0.5f;
    public Vector2 limitsHorizontal;
    public Vector2 limitsVertical;
    // Prefer public defined directions but check against position
    PopoutHorizontal directionH;
    PopoutVertical directionV;
    InfoLabelController label;
    // Use animcurve for dimensions.
    AnimatedValueList animatedRect = new AnimatedValueList();

    AnimationState animState = AnimationState.Idle;
    AnimationState AnimState
	{
        get { return animState; }
		set
		{
            animState = value;
            if (animState == AnimationState.Idle)
			{
                label.showLabel = true;
			}
			else
			{
                label.showLabel = false;
			}
		}
	}
    void Start()
    {
        limitsHorizontal = new Vector2(0, Screen.width);
        limitsVertical = new Vector2(0, Screen.height);
        label = GetComponent<InfoLabelController>();
        Assert.IsNotNull(label);
        label.showLabel = false;
        directionH = horizontalDirection;
        directionV = verticalDirection;
        animatedRect.animTimeSeconds = popoutTimeSeconds;
        switch (directionH)
        {
            case PopoutHorizontal.Right:
                if (popOutFrom.x + label.GetLabelSize().x > limitsHorizontal.y)
                {
                    directionH = PopoutHorizontal.Left;
                }
                break;
            case PopoutHorizontal.Left:
                if (popOutFrom.x - label.GetLabelSize().x < limitsHorizontal.x)
                {
                    directionH = PopoutHorizontal.Right;
                }
                break;
        }
        switch (directionV)
        {
            case PopoutVertical.Up:
                if (popOutFrom.y + label.GetLabelSize().y > limitsVertical.y)
                {
                    directionV = PopoutVertical.Down;
                }
                break;
            case PopoutVertical.Down:
                if (popOutFrom.y - label.GetLabelSize().y < limitsVertical.x)
                {
                    directionV = PopoutVertical.Up;
                }
                break;
        }
        SetupAnimatedRect();
        AnimState = AnimationState.Expanding;
    }

    void SetupAnimatedRect()
    {
        for (int i = 0; i < 4; ++i)
            animatedRect.AddAnimatedValue();

        float labelLeft = 0.0f;
        float labelTop = 0.0f;
        if (directionH == PopoutHorizontal.Left)
		{
            labelLeft = popOutFrom.x - label.GetLabelSize().x;
            animatedRect.SetStartAndTarget(0, popOutFrom.x, labelLeft);
        }
		else
		{
            labelLeft = popOutFrom.x;
            animatedRect.SetStartAndTarget(0, popOutFrom.x, popOutFrom.x);
        }
        animatedRect.SetStartAndTarget(2, 0.0f, label.GetLabelSize().x);
        if (directionV == PopoutVertical.Up)
        {
            labelTop = popOutFrom.y - label.GetLabelSize().y;
            animatedRect.SetStartAndTarget(1, popOutFrom.y, labelTop);
        }
        else
        {
            labelTop = popOutFrom.y;
            animatedRect.SetStartAndTarget(1, popOutFrom.y, popOutFrom.y);
        }
        animatedRect.SetStartAndTarget(3, 0.0f, label.GetLabelSize().y);
        label.screenPosTopLeft = new Vector2Int((int)labelLeft, (int)labelTop);
    }

    void Update()
    {
        if (AnimState != AnimationState.Idle)
		{
            bool complete = animatedRect.Update(Time.deltaTime);
            if (complete)
			{
                AnimState = AnimationState.Idle;
			}
		}
    }

	private void OnGUI()
	{
        if (AnimState != AnimationState.Idle)
        {
            Vector2 xy = new Vector2(animatedRect.interpedValues[0].value, animatedRect.interpedValues[1].value);
            Vector2 wh = new Vector2(animatedRect.interpedValues[2].value, animatedRect.interpedValues[3].value);
            GUI.Label(new Rect(xy, wh), "", label.guiStyle);
        }
    }
}
