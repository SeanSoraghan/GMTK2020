using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHighlighterOverlay : MonoBehaviour
{
	public enum BorderSection
	{
		Left,
		Right,
		Top,
		Bottom,
		NumSections
	}	

	public enum AnimationState
	{
		Idle,
		Animating
	}

	AnimationState animState = AnimationState.Idle;
	public AnimationState AnimState
	{
		get { return animState; }
		set
		{
			animState = value;
			if (animState == AnimationState.Animating)
			{
				borderSectionLengths.Reset();
			}
		}
	}

	public delegate void OnHighlightCompleteDelegate();
	public event OnHighlightCompleteDelegate HighlightComplete;

	public Vector2 highlightSectionCentre;
	public Vector2 highlightSectionDimensions;

	public Color32 borderColour;

	public float HighlightAnimationTimeSeconds = 2.0f;
	public AnimCurve.MotionType AnimMotionType = AnimCurve.MotionType.Exponential;
	AnimatedValueList borderSectionLengths = new AnimatedValueList();

	GUIStyle guiStyle = new GUIStyle();

	public void HighlightSquareFromScreenEdges(Vector2 centre, Vector2 widthHeight)
	{
		highlightSectionCentre = centre;
		highlightSectionDimensions = widthHeight;
		borderSectionLengths.SetStartAndTarget((int)BorderSection.Left, 0.0f, highlightSectionCentre.x - highlightSectionDimensions.x * 0.5f);
		borderSectionLengths.SetStartAndTarget((int)BorderSection.Bottom, Screen.height, highlightSectionCentre.y + highlightSectionDimensions.y * 0.5f);
		borderSectionLengths.SetStartAndTarget((int)BorderSection.Right, Screen.width, highlightSectionCentre.x + highlightSectionDimensions.x * 0.5f);
		borderSectionLengths.SetStartAndTarget((int)BorderSection.Top, 0.0f, highlightSectionCentre.y - highlightSectionDimensions.y * 0.5f);

		AnimState = AnimationState.Animating;
	}

	public void HighlightSquare(Vector2 centre, Vector2 widthHeight)
	{
		highlightSectionCentre = centre;
		highlightSectionDimensions = widthHeight;
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Left, highlightSectionCentre.x - highlightSectionDimensions.x * 0.5f);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Bottom, highlightSectionCentre.y + highlightSectionDimensions.y * 0.5f);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Right, highlightSectionCentre.x + highlightSectionDimensions.x * 0.5f);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Top, highlightSectionCentre.y - highlightSectionDimensions.y * 0.5f);

		AnimState = AnimationState.Animating;
	}

	public void Retract()
	{
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Left, 0.0f);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Right, Screen.width);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Bottom, Screen.height);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Top, 0.0f);

		AnimState = AnimationState.Animating;
	}
	private void Awake()
	{
		for (int section = 0; section < (int)BorderSection.NumSections; ++section)
		{
			borderSectionLengths.AddAnimatedValue();
		}
		borderSectionLengths.animTimeSeconds = HighlightAnimationTimeSeconds;
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Right, Screen.width);
		borderSectionLengths.SetTargetFromCurrent((int)BorderSection.Bottom, Screen.height);

		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		texture.SetPixel(0, 0, borderColour);
		texture.Apply();
		guiStyle.normal.background = texture;
	}

	float sectionLeft() { return borderSectionLengths.interpedValues[(int)BorderSection.Left].value; }
	float sectionRight() { return borderSectionLengths.interpedValues[(int)BorderSection.Right].value; }
	float sectionTop() { return borderSectionLengths.interpedValues[(int)BorderSection.Top].value; }
	float sectionBottom() { return borderSectionLengths.interpedValues[(int)BorderSection.Bottom].value; }

	private void OnGUI()
	{
		float insetLeftWidth = Mathf.Max(sectionLeft(), 0.0f);
		float insetRightWidth = Screen.width - sectionRight();
		float insetTopHeight = sectionTop();

		
		// left
		GUI.Label(new Rect(new Vector2(0.0f, 0.0f), new Vector2(insetLeftWidth, Screen.height)), "", guiStyle);
		// top (minus left)
		GUI.Label(new Rect(new Vector2(sectionLeft(), 0.0f), new Vector2(Screen.width - sectionLeft(), insetTopHeight)), "", guiStyle);
		// right (minus bottom)
		GUI.Label(new Rect(new Vector2(sectionRight(), sectionTop()), new Vector2(insetRightWidth, Screen.height - insetTopHeight)), "", guiStyle);
		// bottom (minus left and right)
		float w = Screen.width - sectionLeft() - (Screen.width - sectionRight());
		GUI.Label(new Rect(new Vector2(sectionLeft(), sectionBottom()), new Vector2(w, Screen.height - sectionBottom())), "", guiStyle);
	}

	private void FixedUpdate()
	{
		if (AnimState == AnimationState.Animating)
		{
			bool animComplete = borderSectionLengths.Update(Time.deltaTime);
			if (animComplete)
			{
				AnimState = AnimationState.Idle;
				HighlightComplete();
			}
		}
	}
}
