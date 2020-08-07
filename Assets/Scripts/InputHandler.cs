using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Instance;

	CubeController[] _cubeControllers = { null, null, null, null };
	public CubeController[] cubeControllers
	{
		private get { return _cubeControllers; }
		set
		{
			_cubeControllers = value;
			foreach(CubeController cubeController in _cubeControllers)
				cubeController.OnMovementEnded += CubeMovementEnded;
		}
	}

	InputActionAsset inputActions;
	UIPanel.MovementDirection lastMovementDirection;

	private void OnDestroy()
	{
		foreach (CubeController cubeController in _cubeControllers)
			if (cubeController != null)
				cubeController.OnMovementEnded -= CubeMovementEnded;

		InputActionMap actionMap = inputActions.actionMaps[0];
		Assert.IsTrue(actionMap.actions.Count == 5);
		actionMap.actions[0].performed -= OnUp;
		actionMap.actions[1].performed -= OnDown;
		actionMap.actions[2].performed -= OnLeft;
		actionMap.actions[3].performed -= OnRight;
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

		PlayerInput input = GetComponent<PlayerInput>();
		Assert.IsNotNull(input);
		if (input != null)
			inputActions = input.actions;
		Assert.IsNotNull(inputActions);
	}

	void Start()
	{
		InputActionMap actionMap = inputActions.actionMaps[0];
		Assert.IsTrue(actionMap.actions.Count == 5);
		actionMap.actions[0].performed += OnUp;
		actionMap.actions[1].performed += OnDown;
		actionMap.actions[2].performed += OnLeft;
		actionMap.actions[3].performed += OnRight;
	}

	public void SetCubeController(int index, CubeController controller)
	{
		if (cubeControllers[index] != null)
			cubeControllers[index].OnMovementEnded -= CubeMovementEnded;
		cubeControllers[index] = controller;
		if (cubeControllers[index] != null)
			cubeControllers[index].OnMovementEnded += CubeMovementEnded;
	}

	public CubeController GetCubeController(int index)
	{
		Assert.IsTrue(index < cubeControllers.Length);
		return cubeControllers[index];
	}

	bool IsAnyCubeMoving()
	{
		foreach (CubeController cubeController in _cubeControllers)
			if (cubeController != null && cubeController.MoveState != CubeController.MovementState.Stationary)
				return true;
		return false;
	}

	CubeController GetCubeForCameraPanelPosition(CameraPanel.DisplayPosition position)
	{
		Assert.IsTrue(cubeControllers.Length == (int)CameraPanel.DisplayPosition.NumPositions);
		return cubeControllers[(int)position];
	}

	void Update()
	{
		if (!IsAnyCubeMoving() && LevelController.GetMazeState() == LevelController.MazeState.InProgress)
		{
			InputSystem.Update();
		}
	}

	void CubeMovementEnded()
	{
		if (UDLRCameraController.Instance != null)
		{
			CubeController cubeController = GetCubeForCameraPanelPosition(UDLRCameraController.Instance.selectedPosition);
			if (cubeController != null && !cubeController.stopPanelMovement)
				UDLRCameraController.Instance.SwitchCamera(lastMovementDirection);
		}
	}

	public void OnUp(InputAction.CallbackContext context)
	{
		if (UDLRCameraController.Instance != null)
		{
			CubeController cubeController = GetCubeForCameraPanelPosition(UDLRCameraController.Instance.selectedPosition);
			if (cubeController != null)
			{
				//move cube according to selected camera
				CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
				Vector3 movementVec = selectedCam != null ? selectedCam.transform.up : Vector3.zero;
				movementVec.Normalize();
				cubeController?.MoveInDirection(movementVec);
				lastMovementDirection = UIPanel.MovementDirection.Up;
			}
		}
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		if (UDLRCameraController.Instance != null)
		{
			CubeController cubeController = GetCubeForCameraPanelPosition(UDLRCameraController.Instance.selectedPosition);
			if (cubeController != null)
			{
				//move cube according to selected camera
				CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
				Vector3 movementVec = selectedCam != null ? -selectedCam.transform.up : Vector3.zero;
				movementVec.Normalize();
				cubeController?.MoveInDirection(movementVec);
				lastMovementDirection = UIPanel.MovementDirection.Down;
			}
		}
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		if (UDLRCameraController.Instance != null)
		{
			CubeController cubeController = GetCubeForCameraPanelPosition(UDLRCameraController.Instance.selectedPosition);
			if (cubeController != null)
			{
				//move cube according to selected camera
				CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
				Vector3 movementVec = selectedCam != null ? -selectedCam.transform.right : Vector3.zero;
				movementVec.Normalize();
				cubeController?.MoveInDirection(movementVec);
				lastMovementDirection = UIPanel.MovementDirection.Left;
			}
		}
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		if (UDLRCameraController.Instance != null)
		{
			CubeController cubeController = GetCubeForCameraPanelPosition(UDLRCameraController.Instance.selectedPosition);
			if (cubeController != null)
			{
				//move cube according to selected camera
				CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
				Vector3 movementVec = selectedCam != null ? selectedCam.transform.right : Vector3.zero;
				movementVec.Normalize();
				cubeController?.MoveInDirection(movementVec);
				lastMovementDirection = UIPanel.MovementDirection.Right;
			}
		}
	}
}
