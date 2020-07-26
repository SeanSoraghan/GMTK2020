using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LineCubeController : MonoBehaviour
{
	LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
		lineRenderer = GetComponent<LineRenderer>();
		Assert.IsNotNull(lineRenderer);
		for(int p = 0; p < lineRenderer.positionCount; ++p)
		{
			Vector3 pos = lineRenderer.GetPosition(p);
			lineRenderer.SetPosition(p, pos * (CubeController.WORLD_CUBE_LIMIT + 0.5f));
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
