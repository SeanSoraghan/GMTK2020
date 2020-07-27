using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : PlayerTrigger
{
	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			LevelController.SetMazeCompleted();
	}
}
