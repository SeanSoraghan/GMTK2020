using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallSetup : MonoBehaviour
{
    public CubeController.MovementState direction;
    public float offset = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
        Vector3 pos = transform.position;
        float worldExtent = CubeController.WORLD_CUBE_LIMIT + 2 + offset;
        switch (direction)
		{
            case CubeController.MovementState.MovingForwards:
                transform.position = new Vector3(0.0f, 0.0f, worldExtent);
                break;
            case CubeController.MovementState.MovingBackwards:
                transform.position = new Vector3(0.0f, 0.0f, -worldExtent);
                break;
            case CubeController.MovementState.MovingRight:
                transform.position = new Vector3(worldExtent, 0.0f, 0.0f);
                break;
            case CubeController.MovementState.MovingLeft:
                transform.position = new Vector3(-worldExtent, 0.0f, 0.0f);
                break;
            case CubeController.MovementState.MovingUp:
                transform.position = new Vector3(0.0f, worldExtent, 0.0f);
                break;
            case CubeController.MovementState.MovingDown:
                transform.position = new Vector3(0.0f, -worldExtent, 0.0f);
                break;
        }
    }
}
