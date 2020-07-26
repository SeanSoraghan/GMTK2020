using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class LineCubeController : MonoBehaviour
{
	LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Awake()
    {
		lineRenderer = GetComponent<LineRenderer>();
		Assert.IsNotNull(lineRenderer);
		// Since we do this in the editor as well, we don't want to set the position once, then multiply it again every Awake for each PIE.
		// So we set newExtent as target / current, and multiply by that.
		float currentExtent = Mathf.Abs(lineRenderer.GetPosition(0).x);
		float mult = (CubeController.WORLD_CUBE_LIMIT + 0.5f) / currentExtent;
		for (int p = 0; p < lineRenderer.positionCount; ++p)
		{
			Vector3 pos = lineRenderer.GetPosition(p);
			lineRenderer.SetPosition(p, pos * mult);
		}
    }
}
