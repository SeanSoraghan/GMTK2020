using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Instance;

	CubeController _cubeController;
	public CubeController cubeController
	{
		private get { return _cubeController; }
		set
		{
			_cubeController = value;
			_cubeController.OnMovementEnded += CubeMovementEnded;
		}
	}

	InputActionAsset inputActions;
	UIPanel.MovementDirection lastMovementDirection;

	private void OnDestroy()
	{
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

	void Update()
	{
		if (cubeController != null && cubeController.MoveState == CubeController.MovementState.Stationary && LevelController.GetMazeState() == LevelController.MazeState.InProgress)
		{
			InputSystem.Update();
		}
	}

	void CubeMovementEnded()
	{
		if (UDLRCameraController.Instance != null && cubeController != null && !cubeController.isInTrigger)
			UDLRCameraController.Instance.SwitchCamera(lastMovementDirection);
	}

	public void OnUp(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
		Vector3 movementVec = selectedCam != null ? selectedCam.transform.up : Vector3.zero;
		movementVec.Normalize();
		cubeController?.MoveInDirection(movementVec);
		lastMovementDirection = UIPanel.MovementDirection.Up;
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
		Vector3 movementVec = selectedCam != null ? -selectedCam.transform.up : Vector3.zero;
		movementVec.Normalize();
		cubeController?.MoveInDirection(movementVec);
		lastMovementDirection = UIPanel.MovementDirection.Down;
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
		Vector3 movementVec = selectedCam != null ? -selectedCam.transform.right : Vector3.zero;
		movementVec.Normalize();
		cubeController?.MoveInDirection(movementVec);
		lastMovementDirection = UIPanel.MovementDirection.Left;
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		CamAnimator selectedCam = UDLRCameraController.GetSelectedCameraAnimator();
		Vector3 movementVec = selectedCam != null ? selectedCam.transform.right : Vector3.zero;
		movementVec.Normalize();
		cubeController?.MoveInDirection(movementVec);
		lastMovementDirection = UIPanel.MovementDirection.Right;
	}
}
