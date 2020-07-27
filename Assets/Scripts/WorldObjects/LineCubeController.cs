using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class LineCubeController : MonoBehaviour
{
	public enum AnimationState
	{
		Shrinking = 0,
		Growing,
		Stationary
	}

	public float animationLengthSeconds = 0.7f;
	float animationStartTime = 0.0f;
	float animationLerpIter = 0.0f;
	AnimationState _animationState = AnimationState.Stationary;
	AnimationState animationState
	{
		get { return _animationState; }
		set
		{
			animationLerpIter = 0.0f;
			_animationState = value;
			if (_animationState != AnimationState.Stationary)
			{
				animationStartTime = Time.time;
			}
			else if (LevelController.IsMazeCompleted())
			{
				LevelController.PostMazeCompleteWorkDone(this);
			}
		}
	}
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

	private void Start()
	{
		animationState = AnimationState.Growing;
		LevelController.RegisterPostMazeCompleteWorker(this);
		LevelController.Instance.OnMazeCompleted += MazeCompleted;
	}

	public void MazeCompleted()
	{
		animationState = AnimationState.Shrinking;
	}

	private void FixedUpdate()
	{
		if (animationState == AnimationState.Growing || animationState == AnimationState.Shrinking)
		{
			float startScale = animationState == AnimationState.Growing ? 0.0f : 1.0f;
			float endScale = animationState == AnimationState.Growing ? 1.0f : 0.0f;
			animationLerpIter = (Time.time - animationStartTime) / animationLengthSeconds;
			float scale = Mathf.Lerp(startScale, endScale, animationLerpIter);
			if ((endScale == 1.0f && scale >= endScale) || (endScale == 0.0f && scale <= endScale))
			{
				scale = endScale;
				animationState = AnimationState.Stationary;
			}
			transform.localScale = new Vector3(scale, scale, scale);
		}
	}
}
