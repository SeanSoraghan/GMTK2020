using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter_Tester : MonoBehaviour
{
    public List<Vector2> centres;
    public List<Vector2> dims;
    public ScreenHighlighterOverlay highlighter;
    public float postHighlightPauseSeconds = 0.5f;
    float secondsSincePause = 0.0f;
    int highlightIndex = 0;
    void Start()
    {
        secondsSincePause = postHighlightPauseSeconds;
        highlighter.HighlightComplete += highlightComplete;
        highlighter.HighlightSquareFromScreenEdges(centres[0], dims[0]);
    }

	private void OnDestroy()
	{
        highlighter.HighlightComplete -= highlightComplete;
	}

	void highlightComplete()
    {
        secondsSincePause = 0.0f;
    }

	void onPauseComplete()
	{
        highlightIndex = (highlightIndex + 1) % (centres.Count + 1);
        if (highlightIndex == centres.Count)
            highlighter.Retract();
        else
            highlighter.HighlightSquareFromCurrent(centres[highlightIndex], dims[highlightIndex]);
    }

	private void Update()
	{
		if (secondsSincePause < postHighlightPauseSeconds)
		{
            secondsSincePause += Time.deltaTime;
            if (secondsSincePause >= postHighlightPauseSeconds)
			{
                onPauseComplete();
			}
		}
	}
}
