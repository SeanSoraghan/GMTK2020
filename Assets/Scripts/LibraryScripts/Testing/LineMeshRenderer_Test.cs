using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LineMeshRenderer_Test : MonoBehaviour
{
    LineMeshRenderer lineRenderer;
    bool _draw = true; //false implies erase
    bool draw
    {
        get { return _draw; }
		set
		{
            _draw = value;
            secondsSincePause = 0.0f;
		}
    }

    public float postLineEventPauseSeconds = 0.5f;
    float secondsSincePause = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        secondsSincePause = postLineEventPauseSeconds;
        lineRenderer = GetComponent<LineMeshRenderer>();
        Assert.IsNotNull(lineRenderer);
        lineRenderer.LineDrawn += OnLineDrawn;
        lineRenderer.LineErased += OnLineErased;
        lineRenderer.BeginDrawingLine();
    }

	private void OnDestroy()
	{
        lineRenderer.LineErased -= OnLineErased;
        lineRenderer.LineDrawn -= OnLineDrawn;
	}
	void OnLineDrawn()
	{
        draw = false;
	}

    void OnLineErased()
	{
        draw = true;
	}

    void onPauseComplete()
	{
        if (draw)
		{
            lineRenderer.BeginDrawingLine();
		}
        else
		{
            lineRenderer.BeginErasingLine();
		}
	}

	private void Update()
	{
        if (secondsSincePause < postLineEventPauseSeconds)
        {
            secondsSincePause += Time.deltaTime;
            if (secondsSincePause >= postLineEventPauseSeconds)
            {
                onPauseComplete();
            }
        }
    }
}
