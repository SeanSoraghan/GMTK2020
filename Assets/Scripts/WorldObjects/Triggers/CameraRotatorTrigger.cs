using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotatorTrigger : PlayerTrigger
{
	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			player.camimator.StartMovement();
	}
}
