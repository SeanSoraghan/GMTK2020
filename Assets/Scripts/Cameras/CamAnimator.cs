using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CamAnimator : MonoBehaviour
{
	public WallSetup.WallPosition initialWallAttach = WallSetup.WallPosition.Back;

	public CameraPanel CameraPanel { get; private set; }

	public Rotator rotator { get; private set; }

	private void Awake()
	{
		CameraPanel = GetComponentInChildren<CameraPanel>();
		Assert.IsNotNull(CameraPanel);
	}

	private void Start()
	{
		rotator = GetComponent<Rotator>();
		Assert.IsNotNull(rotator);
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
}
