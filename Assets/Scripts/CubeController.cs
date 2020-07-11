using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CubeController : MonoBehaviour
{
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

    public GameObject camerasParent;
    public GameObject fpsFrontPanel;
    public float unitMovementTimeSeconds = 0.2f;

    PlayerInput input;
    CharacterController controller;
    Vector3 moveStartPos;
    Vector3 moveTargetPos;
    Vector3 moveVec;
    float moveStartTime = 0.0f;
    float moveLerpX = 0.0f;

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
                switch (moveState)
				{
                    case MovementState.MovingForwards:
                        moveTargetPos = transform.position + Vector3.forward;
                        break;
                    case MovementState.MovingBackwards:
                        moveTargetPos = transform.position - Vector3.forward;
                        break;
                    case MovementState.MovingRight:
                        moveTargetPos = transform.position + Vector3.right;
                        break;
                    case MovementState.MovingLeft:
                        moveTargetPos = transform.position - Vector3.right;
                        break;
                    case MovementState.MovingUp:
                        moveTargetPos = transform.position + Vector3.up;
                        break;
                    case MovementState.MovingDown:
                        moveTargetPos = transform.position - Vector3.up;
                        break;
                    default: break;
                }
                if (moveState != MovementState.Stationary)
				{
                    moveStartTime = Time.time;
                    moveStartPos = transform.position;
                    moveVec = (moveTargetPos - moveStartPos).normalized;
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
        controller = GetComponent<CharacterController>();
        Assert.IsTrue(controller != null);
        input = GetComponent<PlayerInput>();
        Assert.IsTrue(input != null);
        Assert.IsTrue(camerasParent != null);
        Assert.IsTrue(fpsFrontPanel != null);

        moveTargetPos = transform.position;
        moveVec = Vector3.zero;

        for (int i = 0; i < input.actions.actionMaps[0].actions.Count; ++i)
            input.actions.actionMaps[0].actions[i].performed += GetResponder((MovementState)i);
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

    // Update is called once per frame
    void Update()
    {
        camerasParent.transform.position = gameObject.transform.position;
        if (MoveState != MovementState.Stationary)
		{
            //         moveLerpX = (Time.time - moveStartTime) / unitMovementTimeSeconds;
            //         Vector3.Lerp(moveStartPos, moveTargetPos, moveLerpX);
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
