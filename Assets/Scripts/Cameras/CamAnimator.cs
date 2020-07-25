using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CamAnimator : MonoBehaviour
{
	public enum CameraArcType
	{
		Up = 0,
		Down,
		Right,
		Left
	}

	public enum AnimationState
	{
		Moving = 0,
		Shaking,
		Stationary
	}

	public static float RotationTimeSeconds = 0.5f;
	public static float RotationIncrement = 1.0f / RotationTimeSeconds;

	public WallSetup.WallPosition initialWallAttach = WallSetup.WallPosition.Back;

	float lerpIter = 0.0f;
	float rotAngle = 0.0f;
	Vector3 rotationAxis = Vector3.zero;
	Vector3 targetEuler = Vector3.zero;
	Vector3 targetPos = Vector3.zero;

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
			if (_animationState == AnimationState.Stationary)
			{
				lerpIter = 0.0f;
				rotAngle = 0.0f;
			}
		}
	}
	public CameraPanel CameraPanel { get; private set; }

	private void Awake()
	{
		CameraPanel = GetComponentInChildren<CameraPanel>();
		Assert.IsNotNull(CameraPanel);
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

	public void ArcCamera(CameraArcType arcDirection)
	{
		// assumes camera is always aligned with one axis and 0 on the others.
		// StartMovement() and FixedUpdate() implement an arc between one axis and the other.
		switch (arcDirection)
		{
			case CameraArcType.Up:
				rotationAxis = transform.right;
				rotAngle = 90.0f;
				break;
			case CameraArcType.Down:
				rotationAxis = transform.right;
				rotAngle = -90.0f;
				break;
			case CameraArcType.Left:
				rotationAxis = transform.up;
				rotAngle = -90.0f;
				break;
			case CameraArcType.Right:
				rotationAxis = transform.up;
				rotAngle = 90.0f;
				break;
		}
		Transform t = transform;
		t.RotateAround(Vector3.zero, rotationAxis, rotAngle);
		targetEuler = t.rotation.eulerAngles;
		targetPos = t.position;
		t.RotateAround(Vector3.zero, rotationAxis, -rotAngle);
		animationState = AnimationState.Moving;
	}

	void FixedUpdate()
	{
		if (animationState == AnimationState.Moving)
		{
			lerpIter += Time.deltaTime;
			transform.RotateAround(Vector3.zero, rotationAxis, rotAngle * RotationIncrement * Time.deltaTime);
			if (lerpIter >= RotationTimeSeconds)
			{
				animationState = AnimationState.Stationary;
				transform.position = targetPos;
				transform.eulerAngles = targetEuler;
			}
		}
	}
}
