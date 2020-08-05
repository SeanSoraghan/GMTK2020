using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Maze Assets/Level Collection")]
public class MazeLevelCollection : ScriptableObject
{
	[SerializeField] private List<MazeLevel> Levels = new List<MazeLevel>();
	public List<MazeLevel> levels => Levels;
}
