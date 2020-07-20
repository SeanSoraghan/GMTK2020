using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAnimator : MonoBehaviour
{
	public enum AnimationState
	{
		Moving = 0,
		Shaking,
		Stationary
	}

	public WallSetup.WallPosition initialWallAttach = WallSetup.WallPosition.Back;

	// technically, this should always be CubeController.WORLD_CUBE_LIMIT + 2 + offset (see p in GetCamPositionRotationForWall).
	// we'll use a slightly more general implementation here, but we still assume the camera will be aligned on one particular axis, and 0 on the others.
	float distToCentre = 0.0f;
	float lerpIter = 0.0f;
	Vector3 forwardAtArcStart = Vector3.zero;
	Vector3 orthogonalDirectionAtArcStart = Vector3.zero;

	AnimationState _animationState = AnimationState.Stationary;
	AnimationState animationState
	{
		get
		{
			return _animationState;
		}

		set
		{
			_animationState = value;
		}
	}

	private void Start()
	{
		MoveCameraToWall(initialWallAttach);
	}

	public void MoveCameraToWall(WallSetup.WallPosition wallPos)
	{
		Vector3 newPos = transform.position;
		Quaternion newRot = transform.rotation;
		WallSetup.GetCamPositionRotationForWall(wallPos, ref newPos, ref newRot, 2.0f);
		transform.position = newPos;
		transform.rotation = newRot;
	}

	public void StartMovement()
	{
		// assumes camera is always aligned with one axis and 0 on the others.
		// StartMovement() and FixedUpdate() implement an arc between one axis and the other.
		forwardAtArcStart = transform.forward;
		orthogonalDirectionAtArcStart = transform.up;
		distToCentre = Vector3.Magnitude(transform.position);
		lerpIter = 0.0f;
		animationState = AnimationState.Moving;
	}

	void FixedUpdate()
	{
		if (animationState == AnimationState.Moving)
		{
			// lerp along one axis, from distToCentre to 0.0
			// calculate the orthogonal axis value to follow an arc, given the arc follows a circle of radius distToCentre.
			lerpIter += Time.deltaTime;
			float newDist = Mathf.Lerp(distToCentre, 0.0f, lerpIter);
			float orth = Mathf.Sqrt(Mathf.Pow(distToCentre, 2.0f) - Mathf.Pow(newDist, 2.0f));
			transform.position = newDist * -forwardAtArcStart + orth * orthogonalDirectionAtArcStart;
			if (newDist < CubeController.SMALL_DISTANCE)
			{
				transform.position = distToCentre * orthogonalDirectionAtArcStart;
				animationState = AnimationState.Stationary;
			}
			transform.LookAt(Vector3.zero);
		}
	}
}
