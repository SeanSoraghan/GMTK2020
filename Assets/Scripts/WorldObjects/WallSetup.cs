using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WallSetup : MonoBehaviour
{
	public enum WallPosition
	{
		Up = 0,
		Down,
		Left,
		Right,
		Front,
		Back
	};

    public WallPosition direction;
    public float offset = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
        Vector3 pos = transform.position;
        float worldExtent = CubeController.WORLD_CUBE_LIMIT + 2 + offset;
        switch (direction)
		{
            case WallPosition.Up:
                transform.position = new Vector3(0.0f, worldExtent, 0.0f);
                break;
            case WallPosition.Down:
                transform.position = new Vector3(0.0f, -worldExtent, 0.0f);
                break;
            case WallPosition.Left:
                transform.position = new Vector3(worldExtent, 0.0f, 0.0f);
                break;
            case WallPosition.Right:
                transform.position = new Vector3(-worldExtent, 0.0f, 0.0f);
                break;
            case WallPosition.Front:
                transform.position = new Vector3(0.0f, 0.0f, worldExtent);
                break;
            case WallPosition.Back:
                transform.position = new Vector3(0.0f, 0.0f, -worldExtent);
                break;
        }
    }
}
