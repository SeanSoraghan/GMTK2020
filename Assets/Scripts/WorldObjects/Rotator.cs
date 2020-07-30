using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public enum ArcType
	{
		Up = 0,
		Down,
		Right,
		Left
	}

	public enum AnimationState
	{
		Moving = 0,
		Stationary
	}

	public enum MotionType
	{
		Linear,
		Exponential,
		Logarithmic
	}

	public float RotationTimeSeconds = 0.5f;
	float RotationSpeed = 0.0f;

	float animLinearCounter = 0.0f;
	float prevAnimCurveCounter = 0.0f;
	float rotAngle = 0.0f;
	float angleLeftToRotate = 0.0f;
	Vector3 arcPoint = Vector3.zero; // this is the point around which the object arcs.
	Vector3 rotationAxis = Vector3.zero;
	Vector3 targetEuler = Vector3.zero;
	Vector3 targetPos = Vector3.zero;
	MotionType currentMotionType = MotionType.Linear;

	AnimationState _animationState = AnimationState.Stationary;
	public AnimationState animationState
	{
		get
		{
			return _animationState;
		}

		private set
		{
			_animationState = value;
			if (_animationState == AnimationState.Stationary)
			{
				animLinearCounter = 0.0f;
				prevAnimCurveCounter = 0.0f;
				rotAngle = 0.0f;
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		RotationSpeed = 1.0f / RotationTimeSeconds;
	}

	// referenceTransform controls the 'perspective' of the rotation.
	// pass a gameObjects own transform to rotate that object up, down, left, right in the world.
	// pass a camera's transform to rotate up, down, left, right from the perspective of that camera.
	public void StartArc(Transform referenceTransform, ArcType arcDirection, Vector3 _arcPoint, MotionType motionType)
	{
		arcPoint = _arcPoint;
		currentMotionType = motionType;
		// assumes camera is always aligned with one axis and 0 on the others.
		// StartMovement() and FixedUpdate() implement an arc between one axis and the other.
		switch (arcDirection)
		{
			case ArcType.Up:
				rotationAxis = referenceTransform.right;
				rotAngle = 90.0f;
				break;
			case ArcType.Down:
				rotationAxis = referenceTransform.right;
				rotAngle = -90.0f;
				break;
			case ArcType.Left:
				rotationAxis = referenceTransform.up;
				rotAngle = 90.0f;
				break;
			case ArcType.Right:
				rotationAxis = referenceTransform.up;
				rotAngle = -90.0f;
				break;
		}
		angleLeftToRotate = rotAngle;
		Transform t = transform;
		t.RotateAround(arcPoint, rotationAxis, rotAngle);
		targetEuler = t.rotation.eulerAngles;
		targetPos = t.position;
		t.RotateAround(arcPoint, rotationAxis, -rotAngle);
		animationState = AnimationState.Moving;
	}

	void FixedUpdate()
	{
		if (animationState == AnimationState.Moving)
		{
			animLinearCounter += Time.deltaTime * RotationSpeed;
			float animCurveCounter = animLinearCounter;
			if (currentMotionType == MotionType.Exponential)
				animCurveCounter = (Mathf.Pow(2.0f, animLinearCounter * 4.0f) - 1.0f) / 15.0f;
			else if (currentMotionType == MotionType.Logarithmic)
				animCurveCounter = Mathf.Log10(9.0f * animLinearCounter + 1.0f);

			float remaining = 1.0f - prevAnimCurveCounter;
			float portionThisFrame = (animCurveCounter - prevAnimCurveCounter) / remaining;
			float angle = angleLeftToRotate * portionThisFrame;
			transform.RotateAround(arcPoint, rotationAxis, angle);
			angleLeftToRotate -= angle;
			prevAnimCurveCounter = animCurveCounter;

			if (animLinearCounter >= 1.0f)
			{
				animationState = AnimationState.Stationary;
				transform.position = targetPos;
				transform.eulerAngles = targetEuler;
			}
		}
	}
}
