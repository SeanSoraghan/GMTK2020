using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CubeController : MonoBehaviour
{
    public enum ControlScheme : int
	{
        TopDown = 0,
        FPSFront,
        FPSBack,
        SideScroller,
        NumSchemes
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
    static float SMALL_DISTANCE = 0.2f;
    public static int WORLD_CUBE_LIMIT = 2;

    static CameraPanel.DisplayPosition ControlSchemeToPanelPosition(ControlScheme controls)
	{
        switch (controls)
		{
            case ControlScheme.TopDown:
                return CameraPanel.DisplayPosition.TopLeft;
            case ControlScheme.FPSFront:
                return CameraPanel.DisplayPosition.TopRight;
            case ControlScheme.FPSBack:
                return CameraPanel.DisplayPosition.BottomRight;
            case ControlScheme.SideScroller:
                return CameraPanel.DisplayPosition.BottomLeft;
        }
        return CameraPanel.DisplayPosition.TopLeft;
	}

    static ControlScheme PanelPositionToControlScheme(CameraPanel.DisplayPosition displayPosition)
    {
        switch (displayPosition)
        {
            case CameraPanel.DisplayPosition.TopLeft:
                return ControlScheme.TopDown;
            case CameraPanel.DisplayPosition.TopRight:
                return ControlScheme.FPSFront;
            case CameraPanel.DisplayPosition.BottomRight:
                return ControlScheme.FPSBack;
            case CameraPanel.DisplayPosition.BottomLeft:
                return ControlScheme.SideScroller;
        }
        return ControlScheme.TopDown;
    }

    public UIController uiController;
    public float unitMovementTimeSeconds = 0.2f;

    InputActionAsset inputActions;
    CharacterController controller;
    Vector3 moveTargetPos;
    Vector3 moveVec;
    ControlScheme _controlScheme = ControlScheme.TopDown;
    ControlScheme controlScheme
	{
        get
		{
            return _controlScheme;
		}
        set
		{
            inputActions.actionMaps[(int)_controlScheme].Disable();
            _controlScheme = value;
            inputActions.actionMaps[(int)_controlScheme].Enable();
            uiController.PositionPanelUI(ControlSchemeToPanelPosition(_controlScheme));
        }
    }

    MovementState moveState = MovementState.Stationary;
    MovementState MoveState
    {
        get
        {
            return moveState;
        }
        set
        {
            if (moveState == MovementState.Stationary)
            {
                moveState = value;
                float limit = WORLD_CUBE_LIMIT - 0.1f;
                switch (moveState)
				{
                    case MovementState.MovingForwards:
                        if (transform.position.z < limit)
                            moveTargetPos = transform.position + Vector3.forward;
                        break;
                    case MovementState.MovingBackwards:
                        if (transform.position.z > -limit)
                            moveTargetPos = transform.position - Vector3.forward;
                        break;
                    case MovementState.MovingRight:
                        if (transform.position.x < limit)
                            moveTargetPos = transform.position + Vector3.right;
                        break;
                    case MovementState.MovingLeft:
                        if (transform.position.x > -limit)
                            moveTargetPos = transform.position - Vector3.right;
                        break;
                    case MovementState.MovingUp:
                        if (transform.position.y < limit)
                            moveTargetPos = transform.position + Vector3.up;
                        break;
                    case MovementState.MovingDown:
                        if (transform.position.y > -limit)
                            moveTargetPos = transform.position - Vector3.up;
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
			}
        }
    }

    public bool MoveInDirection(MovementState direction)
	{
        MoveState = direction;
        return MoveState == direction;
	}

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT);
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
            Assert.IsTrue(actionMap.actions.Count == (int)MovementState.Stationary + 2);
            for (int i = 0; i < (int)MovementState.Stationary; ++i)
                actionMap.actions[i].performed += GetResponder((MovementState)i);
            actionMap.actions[(int)MovementState.Stationary].performed += OnSwitchControlsVertical;
            actionMap.actions[(int)MovementState.Stationary + 1].performed += OnSwitchControlsHorizontal;
            actionMap.Disable();
        }
        controlScheme = ControlScheme.SideScroller;
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
    public void OnMoveForwards(InputAction.CallbackContext context)
    {
        MoveState = MovementState.MovingForwards;
    }

    public void OnMoveBackwards(InputAction.CallbackContext context) { MoveState = MovementState.MovingBackwards; }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        MoveState = MovementState.MovingRight;
    }

    public void OnMoveLeft(InputAction.CallbackContext context) { MoveState = MovementState.MovingLeft; }

    public void OnMoveUp(InputAction.CallbackContext context) { MoveState = MovementState.MovingUp; }

    public void OnMoveDown(InputAction.CallbackContext context) { MoveState = MovementState.MovingDown; }

    public void OnSwitchControlsVertical(InputAction.CallbackContext context)
    {
        switch (ControlSchemeToPanelPosition(controlScheme))
        {
            case CameraPanel.DisplayPosition.TopLeft:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.BottomLeft);
                break;
            case CameraPanel.DisplayPosition.TopRight:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.BottomRight);
                break;
            case CameraPanel.DisplayPosition.BottomRight:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.TopRight);
                break;
            case CameraPanel.DisplayPosition.BottomLeft:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.TopLeft);
                break;
        }
    }

    public void OnSwitchControlsHorizontal(InputAction.CallbackContext context)
    {
        switch (ControlSchemeToPanelPosition(controlScheme))
        {
            case CameraPanel.DisplayPosition.TopLeft:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.TopRight);
                break;
            case CameraPanel.DisplayPosition.TopRight:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.TopLeft);
                break;
            case CameraPanel.DisplayPosition.BottomRight:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.BottomLeft);
                break;
            case CameraPanel.DisplayPosition.BottomLeft:
                controlScheme = PanelPositionToControlScheme(CameraPanel.DisplayPosition.BottomRight);
                break;
        }
    }

    // Update is called once per frame
    void Update()
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
    }

    
}
