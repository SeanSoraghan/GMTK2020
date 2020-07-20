using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoal : PlayerTrigger
{
	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			player.goalReached = true;
	}
}
