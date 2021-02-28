﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UDLRCameraController : MonoBehaviour
{
	public static UDLRCameraController Instance;

	public Texture2D frameTexture;

	public delegate void SelectedCameraChanged(CamAnimator selectedCamera);
	public event SelectedCameraChanged OnSelectedCameraChanged;

	public GameObject[] CameraPrefabs = { null, null, null, null };
	CamAnimator[] cameraAnimators = { null, null, null, null };
	CameraPanel.DisplayPosition _selectedPosition = CameraPanel.DisplayPosition.TopLeft;
	public UIPanel panelController { get; private set; }
	public CameraPanel.DisplayPosition selectedPosition
	{
		get { return _selectedPosition; }
		private set
		{
			Vector3 selectedCamPosition = Vector3.zero;
			foreach (CamAnimator cam in cameraAnimators)
			{
				if (cam != null)
				{
					CameraPanel panel = cam.CameraPanel;
					if (panel.camPosition == value)
					{
						selectedCamPosition = cam.transform.position;
					}
				}
			}
			foreach (CamAnimator cam in cameraAnimators)
			{
				if (cam != null)
				{
					CameraPanel panel = cam.CameraPanel;
					ScalePulser pulser = cam.GetCameraPulser();
					if (pulser != null)
						pulser.StopLooping();
					MeshRenderer camObjRenderer = cam.GetCameraObjectRenderer();
					panel.IsSelected = false;
					if (panel.camPosition == value)
					{
						panel.IsSelected = true;
						if (pulser != null && LevelController.AreAllObjectsRevealed())
							pulser.StartLooping();
					}
					if (camObjRenderer != null)
					{
						if (panel.IsSelected)
						{
							Color fillColor = camObjRenderer.material.GetColor("_EmissionColor");
							var fillColorArray = frameTexture.GetPixels();
							for (var i = 0; i < fillColorArray.Length; ++i)
							{
								fillColorArray[i] = fillColor;
							}
							frameTexture.SetPixels(fillColorArray);
							frameTexture.Apply();
						}
					}
				}
				
			}
			_selectedPosition = value;
			OnSelectedCameraChanged?.Invoke(GetSelectedCameraAnimator());
		}
	}

	public CamAnimator[] GetCameraAnimators()
	{
		return cameraAnimators;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}

		panelController = GetComponent<UIPanel>();
		Assert.IsNotNull(panelController);
		//CamAnimator[] camAnims = GetComponentsInChildren<CamAnimator>();
		//foreach(CamAnimator camAnimator in camAnims)
		//{
		//	if (camAnimator != null)
		//	{
		//		cameraAnimators.SetValue(camAnimator, (int)camAnimator.CameraPanel.camPosition);
		//	}
		//}
		for (CameraPanel.DisplayPosition camPos = 0; camPos < CameraPanel.DisplayPosition.NumPositions; ++camPos)
		{
			if (LevelController.Instance.LevelCollection.levels[LevelController.Instance.levelIndex].cameraToggles[(int)camPos])
			{
				GameObject camObj = Instantiate(CameraPrefabs[(int)camPos]);
				camObj.transform.parent = gameObject.transform;
				cameraAnimators[(int)camPos] = camObj.GetComponent<CamAnimator>();
				GameObject camModel = camObj.GetComponentInChildren<ScalePulser>().gameObject;
				Vector3 localPos = camModel.transform.localPosition;
				camModel.transform.localPosition = new Vector3(localPos.x, localPos.y, LevelController.Instance.worldSettings.worldExtent);
			}
		}
	}

	void Start()
    {
		Assert.IsTrue(cameraAnimators.Length == 4);
		SelectCameraImmediate(LevelController.Instance.GetCurrentLevel().InitialPanelPosition);
	}

	public void ResetCameraPositions()
	{
		foreach (CamAnimator cam in cameraAnimators)
			if (cam != null)
				cam.ResetCameraPosition();
	}

	public static void StartPulsingSelectedCamera()
	{
		ScalePulser pulser = GetSelectedCameraAnimator().GetCameraPulser();
		if (pulser != null)
			pulser.StartLooping();
	}

	public static void RotateCameras(Transform relativeTransform, Rotator.ArcType arcType, AnimCurve.MotionType motionType)
	{
		if (Instance == null)
			return;
		foreach (CamAnimator camAnimator in Instance.cameraAnimators)
		{
			camAnimator?.rotator.StartArc(relativeTransform, arcType, Vector3.zero, motionType);
		}
	}

	public static CamAnimator GetSelectedCameraAnimator()
	{
		if (Instance == null)
			return null;

		foreach (CamAnimator cam in Instance.cameraAnimators)
		{
			if (cam != null)
			{
				CameraPanel panel = cam.CameraPanel;
				if (panel.camPosition == Instance.selectedPosition)
					return cam;
			}
		}
		Assert.IsTrue(false /* Found no selected camera! */);
		return Instance.cameraAnimators[0];
	}

	public void SelectCameraImmediate(CameraPanel.DisplayPosition camPosition)
	{
		selectedPosition = camPosition;
		panelController.PositionPanelImmediate(selectedPosition);
	}

	public void SwitchCamera(UIPanel.MovementDirection direction)
	{
		switch (direction)
		{
			case UIPanel.MovementDirection.Up:
			case UIPanel.MovementDirection.Down:
				SwitchCameraVertical(direction);
				break;
			case UIPanel.MovementDirection.Left:
			case UIPanel.MovementDirection.Right:
				SwitchCameraHorizontal(direction);
				break;
		}
	}

	public void SwitchCameraVertical(UIPanel.MovementDirection direction)
	{
		Assert.IsTrue(direction == UIPanel.MovementDirection.Up || direction == UIPanel.MovementDirection.Down);
		selectedPosition = CameraPanel.SwitchPositionVertical(selectedPosition);
		panelController.PositionPanel(selectedPosition, direction);
	}

	public void SwitchCameraHorizontal(UIPanel.MovementDirection direction)
	{
		Assert.IsTrue(direction == UIPanel.MovementDirection.Left || direction == UIPanel.MovementDirection.Right);
		selectedPosition = CameraPanel.SwitchPositionHorizontal(selectedPosition);
		panelController.PositionPanel(selectedPosition, direction);
	}
}
