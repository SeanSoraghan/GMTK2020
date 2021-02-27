using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHighlighterOverlay : MonoBehaviour
{
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
				sectionLeft.Reset();
				sectionRight.Reset();
				sectionTop.Reset();
				sectionBottom.Reset();
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
	AnimatedValue sectionLeft = new AnimatedValue();
	AnimatedValue sectionBottom = new AnimatedValue();
	AnimatedValue sectionRight = new AnimatedValue();
	AnimatedValue sectionTop = new AnimatedValue();

	GUIStyle guiStyle = new GUIStyle();

	public void HighlightSquareFromScreenEdges(Vector2 centre, Vector2 widthHeight)
	{
		highlightSectionCentre = centre;
		highlightSectionDimensions = widthHeight;
		sectionLeft.SetStartAndTarget(0.0f, highlightSectionCentre.x - highlightSectionDimensions.x * 0.5f);
		sectionBottom.SetStartAndTarget(Screen.height, highlightSectionCentre.y + highlightSectionDimensions.y * 0.5f);
		sectionRight.SetStartAndTarget(Screen.width, highlightSectionCentre.x + highlightSectionDimensions.x * 0.5f);
		sectionTop.SetStartAndTarget(0.0f, highlightSectionCentre.y - highlightSectionDimensions.y * 0.5f);

		AnimState = AnimationState.Animating;
	}

	public void HighlightSquareFromCurrent(Vector2 centre, Vector2 widthHeight)
	{
		highlightSectionCentre = centre;
		highlightSectionDimensions = widthHeight;
		sectionLeft.SetTargetFromCurrent(highlightSectionCentre.x - highlightSectionDimensions.x * 0.5f);
		sectionBottom.SetTargetFromCurrent(highlightSectionCentre.y + highlightSectionDimensions.y * 0.5f);
		sectionRight.SetTargetFromCurrent(highlightSectionCentre.x + highlightSectionDimensions.x * 0.5f);
		sectionTop.SetTargetFromCurrent(highlightSectionCentre.y - highlightSectionDimensions.y * 0.5f);

		AnimState = AnimationState.Animating;
	}

	public void Retract()
	{
		sectionLeft.SetTargetFromCurrent(0.0f);
		sectionBottom.SetTargetFromCurrent(Screen.height);
		sectionRight.SetTargetFromCurrent(Screen.width);
		sectionTop.SetTargetFromCurrent(0.0f);

		AnimState = AnimationState.Animating;
	}

	private void Start()
	{
		sectionLeft.animTimeSeconds = HighlightAnimationTimeSeconds;
		sectionBottom.animTimeSeconds = HighlightAnimationTimeSeconds;
		sectionRight.animTimeSeconds = HighlightAnimationTimeSeconds;
		sectionTop.animTimeSeconds = HighlightAnimationTimeSeconds;

		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		texture.SetPixel(0, 0, borderColour);
		texture.Apply();
		guiStyle.normal.background = texture;
	}

	private void OnGUI()
	{
		float insetLeftWidth = Mathf.Max(sectionLeft.value, 0.0f);
		float insetBottomHeight = Mathf.Max(sectionBottom.value, 0.0f);
		float insetRightWidth = Screen.width - sectionRight.value;
		float insetTopHeight = sectionTop.value;

		
		// left
		GUI.Label(new Rect(new Vector2(0.0f, 0.0f), new Vector2(insetLeftWidth, Screen.height)), "", guiStyle);
		// top (minus left)
		GUI.Label(new Rect(new Vector2(sectionLeft.value, 0.0f), new Vector2(Screen.width - sectionLeft.value, insetTopHeight)), "", guiStyle);
		// right (minus bottom)
		GUI.Label(new Rect(new Vector2(sectionRight.value, sectionTop.value), new Vector2(insetRightWidth, Screen.height - insetTopHeight)), "", guiStyle);
		// bottom (minus left and right)
		float w = Screen.width - sectionLeft.value - (Screen.width - sectionRight.value);
		GUI.Label(new Rect(new Vector2(sectionLeft.value, sectionBottom.value), new Vector2(w, Screen.height - sectionBottom.value)), "", guiStyle);
	}

	private void FixedUpdate()
	{
		if (AnimState == AnimationState.Animating)
		{
			bool animComplete = sectionLeft.Update(Time.deltaTime);
			animComplete &= sectionTop.Update(Time.deltaTime);
			animComplete &= sectionBottom.Update(Time.deltaTime);
			animComplete &= sectionRight.Update(Time.deltaTime);
			if (animComplete)
			{
				AnimState = AnimationState.Idle;
				HighlightComplete();
			}
		}
	}
}
