using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : PlayerTrigger
{
	public CameraPanel.DisplayPosition associatedDisplayPosition = CameraPanel.DisplayPosition.NumPositions;
	public override void PlayerEnteredTrigger(CubeController player)
	{
		base.PlayerEnteredTrigger(player);
		if (player != null)
		{
			if (player.associatedPanelPosition == associatedDisplayPosition)
				LevelController.SetCubeInTarget(associatedDisplayPosition, true);
			else
				LevelController.CubeEnteredIncorrectTarget();
		}
	}
	public override void PlayerExitedTrigger(CubeController player)
	{
		base.PlayerExitedTrigger(player);
		if (player != null)
		{
			if (player.associatedPanelPosition == associatedDisplayPosition)
				LevelController.SetCubeInTarget(associatedDisplayPosition, false);
		}
	}
}
