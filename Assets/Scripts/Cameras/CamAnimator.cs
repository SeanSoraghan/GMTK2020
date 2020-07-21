using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CamAnimator : MonoBehaviour
{
	public enum AnimationState
	{
		Moving = 0,
		Shaking,
		Stationary
	}

	public WallSetup.WallPosition initialWallAttach = WallSetup.WallPosition.Back;

	float lerpIter = 0.0f;
	Vector3 rotationAxis = Vector3.zero;

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
	public CameraPanel CameraPanel { get; private set; }

	private void Start()
	{
		CameraPanel = GetComponentInChildren<CameraPanel>();
		Assert.IsNotNull(CameraPanel);
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
		rotationAxis = transform.right;
		lerpIter = 0.0f;
		animationState = AnimationState.Moving;
	}

	void FixedUpdate()
	{
		if (animationState == AnimationState.Moving)
		{
			lerpIter += Time.deltaTime;
			float rotDelta = Mathf.Lerp(0.0f, 90.0f, lerpIter);
			transform.RotateAround(Vector3.zero, rotationAxis, 90.0f * Time.deltaTime);
			if (/*newDist < CubeController.SMALL_DISTANCE*/lerpIter >= 1.0f)
			{
				animationState = AnimationState.Stationary;
			}
		}
	}
}
