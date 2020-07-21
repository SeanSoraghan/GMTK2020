﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UDLRCameraController : MonoBehaviour
{
	public CamAnimator[] cameraAnimators;
	UIPanel panelController;
	CameraPanel.DisplayPosition _selectedPosition = CameraPanel.DisplayPosition.TopLeft;
	public CameraPanel.DisplayPosition selectedPosition
	{
		get { return _selectedPosition; }
		private set
		{
			foreach (CamAnimator cam in cameraAnimators)
			{
				CameraPanel panel = cam.CameraPanel;
				panel.IsSelected = false;
				if (panel.camPosition == value)
					panel.IsSelected = true;
			}
			_selectedPosition = value;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		panelController = GetComponent<UIPanel>();
		Assert.IsNotNull(panelController);
		Assert.IsTrue(cameraAnimators.Length == 4);
		SelectCameraImmediate(CameraPanel.DisplayPosition.TopLeft);
    }

	public CamAnimator GetSelectedCameraAnimator()
	{
		foreach (CamAnimator cam in cameraAnimators)
		{
			CameraPanel panel = cam.CameraPanel;
			if (panel.camPosition == selectedPosition)
				return cam;
		}
		Assert.IsTrue(false /* Found no selected camera! */);
		return cameraAnimators[0];
	}

	public void SelectCameraImmediate(CameraPanel.DisplayPosition camPosition)
	{
		selectedPosition = camPosition;
		panelController.PositionPanelImmediate(selectedPosition);
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
