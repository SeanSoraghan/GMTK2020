using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReset : PlayerTrigger
{
	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			player.shouldReset = true;
	}
}
