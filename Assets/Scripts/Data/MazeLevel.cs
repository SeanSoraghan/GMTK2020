using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RotatorTriggerData //: ScriptableObject
{
	[SerializeField] private Vector3 Position;
	[SerializeField] private Rotator.ArcType ArcType;

	public Vector3 position => Position;
	public Rotator.ArcType arcType => ArcType;
}

[CreateAssetMenu(menuName = "Maze Assets/Level")]
public class MazeLevel : ScriptableObject
{
	[SerializeField] private bool[] CameraToggles = { true, true, true, true };
	[SerializeField] private Vector3[] CubeStartPositions = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
	[SerializeField] private Vector3[] GoalPositions = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
	[SerializeField] private List<RotatorTriggerData> Rotators = new List<RotatorTriggerData>();

	public CameraPanel.DisplayPosition InitialPanelPosition = CameraPanel.DisplayPosition.TopLeft;
	public bool[] cameraToggles => CameraToggles;
	public Vector3[] cubeStartPositions => CubeStartPositions;
	public Vector3[] goalPositions => GoalPositions;
	public List<RotatorTriggerData> rotators => Rotators;
}
