using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UDLRCameraController : MonoBehaviour
{
	public CameraPanel[] cameraPanels;
	UIPanel panelController;
	CameraPanel.DisplayPosition _selectedPosition = CameraPanel.DisplayPosition.TopLeft;
	public CameraPanel.DisplayPosition selectedPosition
	{
		get { return _selectedPosition; }
		private set
		{
			foreach (CameraPanel cam in cameraPanels)
			{
				cam.IsSelected = false;
				if (cam.camPosition == value)
					cam.IsSelected = true;
			}
			_selectedPosition = value;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		panelController = GetComponent<UIPanel>();
		Assert.IsNotNull(panelController);
		Assert.IsTrue(cameraPanels.Length == 4);
		SelectCameraImmediate(CameraPanel.DisplayPosition.TopLeft);
    }

	public CameraPanel GetSelectedCamera()
	{
		foreach (CameraPanel cam in cameraPanels)
		{
			if (cam.camPosition == selectedPosition)
				return cam;
		}
		Assert.IsTrue(false /* Found no selected camera! */);
		return cameraPanels[0];
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
