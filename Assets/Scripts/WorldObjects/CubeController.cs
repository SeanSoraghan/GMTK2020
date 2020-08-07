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

	public float unitMovementTimeSeconds = 0.2f;

	public bool isInTrigger { get; set; }

    public bool shouldReset = false;
	bool _goalReached = false;
    public bool goalReached
	{
		get { return _goalReached; }
		set
		{
			_goalReached = value;
			if (MoveState == MovementState.Stationary && _goalReached)
			{
				shouldReset = true;
			}
		}
	}

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
			MovementState prev = _moveState;
            _moveState = value;
            if (prev == MovementState.Moving && _moveState == MovementState.Stationary)
			{
                moveVec = Vector3.zero;
                if (goalReached)
                {
                    shouldReset = true;
                }
				OnMovementEnded?.Invoke();
			}
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
        moveTargetPos = transform.position;

        moveTargetPos = transform.position;
        moveVec = Vector3.zero;

		if (UDLRCameraController.Instance != null)
			UDLRCameraController.Instance.OnSelectedCameraChanged += SelectedCameraChanged;
		SelectedCameraChanged(UDLRCameraController.GetSelectedCameraAnimator());
	}

	private void OnDestroy()
	{
		if (UDLRCameraController.Instance != null)
			UDLRCameraController.Instance.OnSelectedCameraChanged -= SelectedCameraChanged;
	}

	public void SelectedCameraChanged(CamAnimator camAnimator)
	{
		//if (camAnimator != null)
		//{
		//	MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		//	if (meshRenderer != null)
		//		meshRenderer.material.SetColor("_EmissionColor", camAnimator.GetCamObjColour());
		//}
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
		// quick fix to get around seeming inaccuracies in float comparison
		float eps = 0.05f;
		float limit = LevelController.WORLD_CUBE_LIMIT + eps;
		Vector3 target = transform.position + direction;
		return !(Mathf.Abs(target.x) >= limit || Mathf.Abs(target.y) >= limit || Mathf.Abs(target.z) >= limit);
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
            goalReached = false;
            shouldReset = false;
        }
    }
}
