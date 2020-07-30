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

	private void Start()
	{
		rotator = GetComponent<Rotator>();
		Assert.IsNotNull(rotator);

		StartRotation();
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

	void StartRotation()
	{
		rotator.StartArc(GetRotationDirection(), transform.position, Rotator.MotionType.Exponential);
		timeSinceLastRotationStart = 0.0f;
	}

	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			player.camController.GetSelectedCameraAnimator().rotator.StartArc(arcType, Vector3.zero, Rotator.MotionType.Linear);
	}

	private void Update()
	{
		timeSinceLastRotationStart += Time.deltaTime;
		if (timeSinceLastRotationStart >= rotator.RotationTimeSeconds + secondsBetweenRotations)
		{
			StartRotation();
		}
	}
}
