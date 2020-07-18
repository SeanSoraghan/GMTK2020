﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CubeController : MonoBehaviour
{
    public enum ControlScheme : int
	{
        TopDown = 0,
        BottomUp,
        SideScrollLR,
        SideScrollRL
	}

    public enum MovementState : int
	{
        MovingForwards = 0,
        MovingBackwards,
        MovingRight,
        MovingLeft,
        MovingUp,
        MovingDown,
        Stationary
	}

    public enum PanelSwitch : int
	{
        Vertical,
        Horizontal,
        None
	}

    static float SMALL_DISTANCE = 0.2f;
    public static int WORLD_CUBE_LIMIT = 2;

    public GUISkin guiSkin;

    public UIController2 uiController;
    public float unitMovementTimeSeconds = 0.2f;

    bool shouldReset = false;
    bool goalReached = false;

    InputActionAsset inputActions;

    CharacterController controller;
    Vector3 moveTargetPos;
    Vector3 moveVec;
    ControlScheme _controlScheme = ControlScheme.TopDown;
    ControlScheme controlScheme
	{
        get { return _controlScheme; }
        set
		{
            inputActions.actionMaps[(int)_controlScheme].Disable();
            _controlScheme = value;
            inputActions.actionMaps[(int)_controlScheme].Enable();
            uiController.PositionPanelUI((CameraPanel.DisplayPosition)_controlScheme, lastPanelMovementDirection);
        }
    }

    MovementState moveState = MovementState.Stationary;
    MovementState MoveState
    {
        get { return moveState; }
        set
        {
            if (moveState == MovementState.Stationary)
            {
                Vector3 pos = transform.position;
                float xR = Mathf.Abs(Mathf.Repeat(pos.x, 1.0f));
                float yR = Mathf.Abs(Mathf.Repeat(pos.y, 1.0f));
                float zR = Mathf.Abs(Mathf.Repeat(pos.z, 1.0f));
                if (xR != 0.0f || yR != 0.0f || zR != 0.0f)
				{
                    Debug.Log("Caught");
                    Assert.IsTrue(false);
				}
                moveState = value;
                float limit = WORLD_CUBE_LIMIT - 0.1f;
                switch (moveState)
				{
                    case MovementState.MovingForwards:
                        if (transform.position.z < limit)
                            moveTargetPos = pos + Vector3.forward;
                        break;
                    case MovementState.MovingBackwards:
                        if (transform.position.z > -limit)
                            moveTargetPos = pos - Vector3.forward;
                        break;
                    case MovementState.MovingRight:
                        if (transform.position.x < limit)
                            moveTargetPos = pos + Vector3.right;
                        break;
                    case MovementState.MovingLeft:
                        if (transform.position.x > -limit)
                            moveTargetPos = pos - Vector3.right;
                        break;
                    case MovementState.MovingUp:
                        if (transform.position.y < limit)
                            moveTargetPos = pos + Vector3.up;
                        break;
                    case MovementState.MovingDown:
                        if (transform.position.y > -limit)
                            moveTargetPos = pos - Vector3.up;
                        break;
                    default: break;
                }
                if (moveState != MovementState.Stationary)
				{
                    moveVec = (moveTargetPos - transform.position).normalized;
                }
            }
            else if (value == MovementState.Stationary)
			{
                moveState = value;
                moveVec = Vector3.zero;
                if (goalReached)
                {
                    shouldReset = true;
                }
			}
        }
    }

	MovementState lastPanelMovementDirection = MovementState.MovingUp;

    public bool MoveInDirection(MovementState direction)
	{
        MoveState = direction;
        return MoveState == direction;
	}

	private void OnDestroy()
	{
        for (int map = 0; map < inputActions.actionMaps.Count; ++map)
        {
            InputActionMap actionMap = inputActions.actionMaps[map];
            Assert.IsTrue(actionMap.actions.Count == (int)MovementState.Stationary + 4);
            actionMap.actions[0].performed -= OnMovePanelUp;
            actionMap.actions[1].performed -= OnMovePanelRight;
			actionMap.actions[2].performed -= OnMovePanelDown;
			actionMap.actions[3].performed -= OnMovePanelLeft;
			for (int i = 2; i < actionMap.actions.Count; ++i)
                actionMap.actions[i].performed -= GetResponder((MovementState)(i - 4));
            actionMap.Disable();
        }
    }
	// Start is called before the first frame update
	void Start()
    {
        transform.position = new Vector3(-WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT);
        moveTargetPos = transform.position;
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);
        PlayerInput input = GetComponent<PlayerInput>();
        Assert.IsNotNull(input);
        if (input != null)
            inputActions = input.actions;
        Assert.IsNotNull(uiController);
        Assert.IsNotNull(inputActions);

        moveTargetPos = transform.position;
        moveVec = Vector3.zero;

        for (int map = 0; map < inputActions.actionMaps.Count; ++map)
        {
            InputActionMap actionMap = inputActions.actionMaps[map];
            Assert.IsTrue(actionMap.actions.Count == (int)MovementState.Stationary + 4);
            actionMap.actions[0].performed += OnMovePanelUp;
            actionMap.actions[1].performed += OnMovePanelRight;
			actionMap.actions[2].performed += OnMovePanelDown;
			actionMap.actions[3].performed += OnMovePanelLeft;
			for (int i = 4; i < actionMap.actions.Count; ++i)
                actionMap.actions[i].performed += GetResponder((MovementState)(i - 4));
            actionMap.Disable();
        }
        controlScheme = ControlScheme.SideScrollLR;
        uiController.TeleportPanel();
    }

    System.Action<InputAction.CallbackContext> GetResponder(MovementState movementState)
	{
        switch (movementState)
		{
            case MovementState.MovingForwards: return OnMoveForwards;
            case MovementState.MovingBackwards: return OnMoveBackwards;
            case MovementState.MovingRight: return OnMoveRight;
            case MovementState.MovingLeft: return OnMoveLeft;
            case MovementState.MovingUp: return OnMoveUp;
            case MovementState.MovingDown: return OnMoveDown;
        }
        return OnMoveForwards;
	}
    // Input responders
    public void OnMoveForwards(InputAction.CallbackContext context) { MoveState = MovementState.MovingForwards; }

    public void OnMoveBackwards(InputAction.CallbackContext context) { MoveState = MovementState.MovingBackwards; }

    public void OnMoveRight(InputAction.CallbackContext context) { MoveState = MovementState.MovingRight; }

    public void OnMoveLeft(InputAction.CallbackContext context) { MoveState = MovementState.MovingLeft; }

    public void OnMoveUp(InputAction.CallbackContext context) { MoveState = MovementState.MovingUp; }

    public void OnMoveDown(InputAction.CallbackContext context) { MoveState = MovementState.MovingDown; }

    public void OnMovePanelUp(InputAction.CallbackContext context)
    {
		lastPanelMovementDirection = MovementState.MovingUp;
        SwitchPanelVertical();
    }

    public void OnMovePanelRight(InputAction.CallbackContext context)
    {
		lastPanelMovementDirection = MovementState.MovingRight;
		SwitchPanelHorizontal();
    }

	public void OnMovePanelDown(InputAction.CallbackContext context)
	{
		lastPanelMovementDirection = MovementState.MovingDown;
		SwitchPanelVertical();
	}

	public void OnMovePanelLeft(InputAction.CallbackContext context)
	{
		lastPanelMovementDirection = MovementState.MovingLeft;
		SwitchPanelHorizontal();
	}

	void SwitchPanelVertical()
    {
		controlScheme = (ControlScheme)CameraPanel.SwitchPositionVertical((CameraPanel.DisplayPosition)controlScheme);
    }

    void SwitchPanelHorizontal()
    {
		controlScheme = (ControlScheme)CameraPanel.SwitchPositionHorizontal((CameraPanel.DisplayPosition)controlScheme);
    }

    void Update()
	{
        if (MoveState == MovementState.Stationary)
        {
            InputSystem.Update();
        }
	}

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Goal")
            goalReached = true;
    }
	void FixedUpdate()
    {
        if (MoveState != MovementState.Stationary)
		{
            Vector3 currentFrameTarget = transform.position + moveVec * (Time.deltaTime / unitMovementTimeSeconds);

			if (Vector3.Distance(transform.position, moveTargetPos) < SMALL_DISTANCE || Vector3.Distance(currentFrameTarget, moveTargetPos) < SMALL_DISTANCE)
			{
				transform.position = moveTargetPos;
				MoveState = MovementState.Stationary;
			}
			else 
            { 
                controller.Move(moveVec * (Time.deltaTime / unitMovementTimeSeconds));
            }
		}
        if (shouldReset) // Ideally the player would be animated back to starting position.
		{
            transform.position = new Vector3(-WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT);
            goalReached = false;
            shouldReset = false;
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }
}
