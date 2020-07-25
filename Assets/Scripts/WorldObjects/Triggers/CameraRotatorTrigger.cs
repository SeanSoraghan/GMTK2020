using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotatorTrigger : PlayerTrigger
{
	public CamAnimator.CameraArcType arcType;
	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			player.camController.GetSelectedCameraAnimator().ArcCamera(arcType);
	}
}
