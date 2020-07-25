using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CubeController : MonoBehaviour
{
    public enum MovementState : int
	{
        Moving = 0,
        Stationary
	}

    public enum PanelSwitch : int
	{
        Vertical,
        Horizontal,
        None
	}

    public static float SMALL_DISTANCE = 0.2f;
    public static int WORLD_CUBE_LIMIT = 2;

	public UDLRCameraController camController;

	public float unitMovementTimeSeconds = 0.2f;

    public bool shouldReset = false;
    public bool goalReached = false;

	public delegate void MovementEnded();
	public event MovementEnded OnMovementEnded;

    CharacterController controller;
    Vector3 moveTargetPos;
    Vector3 moveVec;

    MovementState _moveState = MovementState.Stationary;
    public MovementState MoveState
    {
        get { return _moveState; }
        set
        {
            if (_moveState == MovementState.Moving && value == MovementState.Stationary)
			{
                moveVec = Vector3.zero;
                if (goalReached)
                {
                    shouldReset = true;
                }
				OnMovementEnded?.Invoke();
			}
            _moveState = value;
        }
    }

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		Assert.IsNotNull(controller);
	}

	// Start is called before the first frame update
	void Start()
    {
        transform.position = new Vector3(-WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT, -WORLD_CUBE_LIMIT);
        moveTargetPos = transform.position;
		Assert.IsNotNull(camController);

        moveTargetPos = transform.position;
        moveVec = Vector3.zero;
    }

	public bool MoveInDirection(Vector3 direction)
	{
		if (MoveState != MovementState.Stationary)
			return false;
		direction.Normalize();
		if (!CheckMovement(direction))
			return false;
		moveVec = direction;
		moveTargetPos = transform.position + direction;
		MoveState = MovementState.Moving;
		return true;
	}

	bool CheckMovement(Vector3 direction)
	{
		float limit = WORLD_CUBE_LIMIT;
		Vector3 target = transform.position + direction;
		return !(Mathf.Abs(target.x) > limit || Mathf.Abs(target.y) > limit || Mathf.Abs(target.z) > limit);
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
            //SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }
}
