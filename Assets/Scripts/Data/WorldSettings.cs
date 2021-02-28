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
	[Range(0, 1)]
	public float cameraViewportSidelength = 0.8f;
	public int BackgroundCamFOV = 25;
}
