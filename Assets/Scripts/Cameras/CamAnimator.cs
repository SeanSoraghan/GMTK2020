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
		rotator = GetComponent<Rotator>();
		Assert.IsNotNull(rotator);
	}

	private void Start()
	{
		MoveCameraToWall(initialWallAttach);
	}

	public void MoveCameraToWall(WallSetup.WallPosition wallPos)
	{
		Vector3 newPos = transform.position;
		Quaternion newRot = transform.rotation;
		WallSetup.GetCamPositionRotationForWall(wallPos, 0.0f, CameraPanel.cam.fieldOfView, ref newPos, ref newRot);
		transform.position = newPos;
		transform.rotation = newRot;
	}

	public Color GetCamObjColour()
	{
		MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
		if (renderer != null)
			return renderer.material.GetColor("_EmissionColor");
		return Color.grey;
	}

	public MeshRenderer GetCameraObjectRenderer()
	{
		return GetComponentInChildren<MeshRenderer>();
	}

	public ScalePulser GetCameraPulser()
	{
		return GetComponentInChildren<ScalePulser>();
	}
}
