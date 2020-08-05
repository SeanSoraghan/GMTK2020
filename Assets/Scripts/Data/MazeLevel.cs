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
	[SerializeField] private Vector3 PlayerStart = Vector3.zero;
	[SerializeField] private List<RotatorTriggerData> Rotators = new List<RotatorTriggerData>();

	public Vector3 playerStart => PlayerStart;
	public List<RotatorTriggerData> rotators => Rotators;
}
