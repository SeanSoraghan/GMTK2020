using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LineThicknessAnimator : MonoBehaviour
{
	public float globalMaxThickness = 0.05f;
	float centre = 0.0f;
	LineRenderer lineRenderer;

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		Assert.IsNotNull(lineRenderer);
	}

	// Start is called before the first frame update
	void Start()
    {
		centre = globalMaxThickness * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {

		lineRenderer.startWidth = centre + Mathf.Sin(Time.time * 1.0f) * centre;
		lineRenderer.endWidth = centre - Mathf.Sin(Time.time * 1.0f) * centre;
    }
}
