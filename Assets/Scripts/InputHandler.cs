using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class InputHandler : MonoBehaviour
{
	public UDLRCameraController camController;
	public CubeController cubeController;
	InputActionAsset inputActions;

	private void OnDestroy()
	{
		InputActionMap actionMap = inputActions.actionMaps[0];
		Assert.IsTrue(actionMap.actions.Count == 5);
		actionMap.actions[0].performed -= OnUp;
		actionMap.actions[1].performed -= OnDown;
		actionMap.actions[2].performed -= OnLeft;
		actionMap.actions[3].performed -= OnRight;
	}

	void Start()
	{
		PlayerInput input = GetComponent<PlayerInput>();
		Assert.IsNotNull(input);
		if (input != null)
			inputActions = input.actions;
		Assert.IsNotNull(inputActions);
		Assert.IsNotNull(camController);
		Assert.IsNotNull(cubeController);

		InputActionMap actionMap = inputActions.actionMaps[0];
		Assert.IsTrue(actionMap.actions.Count == 5);
		actionMap.actions[0].performed += OnUp;
		actionMap.actions[1].performed += OnDown;
		actionMap.actions[2].performed += OnLeft;
		actionMap.actions[3].performed += OnRight;
	}

	void Update()
	{
		if (cubeController.MoveState == CubeController.MovementState.Stationary)
		{
			InputSystem.Update();
		}
	}

	public void OnUp(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		Vector3 movementVec = camController.GetSelectedCamera().transform.up;
		movementVec.Normalize();
		if (cubeController.MoveInDirection(movementVec))
			camController.SwitchCameraVertical(UIPanel.MovementDirection.Up);
	}

	public void OnDown(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		Vector3 movementVec = -camController.GetSelectedCamera().transform.up;
		movementVec.Normalize();
		if (cubeController.MoveInDirection(movementVec))
			camController.SwitchCameraVertical(UIPanel.MovementDirection.Down);
	}

	public void OnLeft(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		Vector3 movementVec = -camController.GetSelectedCamera().transform.right ;
		movementVec.Normalize();
		if (cubeController.MoveInDirection(movementVec))
			camController.SwitchCameraHorizontal(UIPanel.MovementDirection.Left);
	}

	public void OnRight(InputAction.CallbackContext context)
	{
		//move cube according to selected camera
		Vector3 movementVec = camController.GetSelectedCamera().transform.right;
		movementVec.Normalize();
		if (cubeController.MoveInDirection(movementVec))
			camController.SwitchCameraHorizontal(UIPanel.MovementDirection.Right);
	}
}
