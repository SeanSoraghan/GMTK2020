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

	public static void GetCamPositionRotationForWall(WallPosition wallPosition, ref Vector3 position, ref Quaternion rotation, float offset)
	{
		float p = CubeController.WORLD_CUBE_LIMIT + 2 + offset;
		switch (wallPosition)
		{
			case WallPosition.Front:
				position = new Vector3(0.0f, 0.0f, p);
				rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
				break;
			case WallPosition.Back:
				position = new Vector3(0.0f, 0.0f, -p);
				rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				break;
			case WallPosition.Up:
				position = new Vector3(0.0f, p, 0.0f);
				rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
				break;
			case WallPosition.Down:
				position = new Vector3(0.0f, -p, 0.0f);
				rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
				break;
			case WallPosition.Left:
				position = new Vector3(-p, 0.0f, 0.0f);
				rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
				break;
			case WallPosition.Right:
				position = new Vector3(p, 0.0f, 0.0f);
				rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
				break;
		}
	}

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
