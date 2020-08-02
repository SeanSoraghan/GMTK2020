using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraRotatorTrigger : PlayerTrigger
{
	public Rotator.ArcType arcType;
	public float secondsBetweenRotations = 0.1f;

	public Rotator rotator { get; private set; }

	float timeSinceLastRotationStart = 0.0f;
	bool needsStartRotation = false;
	Transform startingTransform;

	private void Awake()
	{
		rotator = GetComponent<Rotator>();
		Assert.IsNotNull(rotator);
	}

	private void Start()
	{
		startingTransform = transform;
		TryStartRotation();
	}

	// since this trigger will cause the camera to arc around in the given direction (arcType), we should rotate it
	// in the opposite direction, so it indicates what the maze cube will appear to do. (i.e. it looks like the whole
	// maze rotates, not that the camera arcs).
	Rotator.ArcType GetRotationDirection()
	{
		switch (arcType)
		{
			case Rotator.ArcType.Down: return Rotator.ArcType.Up;
			case Rotator.ArcType.Up: return Rotator.ArcType.Down;
			case Rotator.ArcType.Left: return Rotator.ArcType.Right;
			case Rotator.ArcType.Right: return Rotator.ArcType.Left;
		}
		return arcType;
	}

	void TryStartRotation()
	{
		if (LevelController.GetMazeState() == LevelController.MazeState.InProgress)
		{
			CamAnimator camAnim = UDLRCameraController.GetSelectedCameraAnimator();
			if (camAnim != null && camAnim.rotator.animationState == Rotator.AnimationState.Stationary)
			{
				rotator.StartArc(camAnim.transform, GetRotationDirection(), transform.position, AnimCurve.MotionType.Exponential);
				timeSinceLastRotationStart = 0.0f;
				needsStartRotation = false;
			}
		}
	}

	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
		{
			CamAnimator camAnim = UDLRCameraController.GetSelectedCameraAnimator();
			//transform.position = startingTransform.position;
			//transform.rotation = startingTransform.rotation;
			//transform.localScale = startingTransform.localScale;
			if (camAnim != null)
				camAnim.rotator.StartArc(camAnim.transform, arcType, Vector3.zero, AnimCurve.MotionType.Linear);
		}
	}

	private void Update()
	{
		timeSinceLastRotationStart += Time.deltaTime;
		if (timeSinceLastRotationStart >= rotator.RotationTimeSeconds + secondsBetweenRotations)
		{
			needsStartRotation = true;
		}
		if (needsStartRotation)
		{
			TryStartRotation();
		}
	}
}
