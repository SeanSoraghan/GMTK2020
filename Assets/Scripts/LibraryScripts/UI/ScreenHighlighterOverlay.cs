using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenHighlighterOverlay : MonoBehaviour
{
	public Vector2 highlightSectionCentre;
	public Vector2 highlightSectionDimensions;

	private void OnGUI()
	{
		float sectionLeft = highlightSectionCentre.x - highlightSectionDimensions.x * 0.5f;
		float sectionBottom = highlightSectionCentre.y + highlightSectionDimensions.y * 0.5f;
		float sectionRight = highlightSectionCentre.x + highlightSectionDimensions.x * 0.5f;
		float sectionTop = highlightSectionCentre.y - highlightSectionDimensions.y * 0.5f;
		float insetLeftWidth = Mathf.Max(sectionLeft, 0.0f);
		float insetBottomHeight = Mathf.Max(sectionBottom, 0.0f);
		float insetRightWidth = Screen.width - sectionRight;
		float insetTopHeight = sectionTop;
		GUI.Box(new Rect(new Vector2(0.0f, 0.0f), new Vector2(insetLeftWidth, Screen.height)), "");
		GUI.Box(new Rect(new Vector2(sectionLeft, 0.0f), new Vector2(highlightSectionDimensions.x + insetRightWidth, insetTopHeight)), "");
		GUI.Box(new Rect(new Vector2(sectionRight, sectionTop), new Vector2(insetRightWidth, Screen.height - insetTopHeight)), "");
		GUI.Box(new Rect(new Vector2(sectionLeft, sectionBottom), new Vector2(highlightSectionDimensions.x, Screen.height - sectionBottom)), "");

		GUI.Box(new Rect(highlightSectionCentre - highlightSectionDimensions * 0.5f, highlightSectionDimensions), "");
	}
}
