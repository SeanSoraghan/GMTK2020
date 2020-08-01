using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UDLRCameraController : MonoBehaviour
{
	public static UDLRCameraController Instance;

	CamAnimator[] cameraAnimators;
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
		cameraAnimators = GetComponentsInChildren<CamAnimator>();
	}

	// Start is called before the first frame update
	void Start()
    {
		Assert.IsTrue(cameraAnimators.Length == 4);
		SelectCameraImmediate(CameraPanel.DisplayPosition.TopLeft);
    }

	public static CamAnimator GetSelectedCameraAnimator()
	{
		if (Instance == null)
			return null;

		foreach (CamAnimator cam in Instance.cameraAnimators)
		{
			CameraPanel panel = cam.CameraPanel;
			if (panel.camPosition == Instance.selectedPosition)
				return cam;
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
