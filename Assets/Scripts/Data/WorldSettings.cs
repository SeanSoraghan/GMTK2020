using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Maze Assets/World Settings")]
public class WorldSettings : ScriptableObject
{
	//	[SerializeField] private LevelController.LayoutMode LayoutMode = LevelController.LayoutMode.CentredPanels;
	//	[SerializeField] private int WorldExtent = 2;

	public LayoutMode layoutMode;
	public int worldExtent;
}
