using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGoal : PlayerTrigger
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	public override void PlayerEnteredTrigger(CubeController player)
	{
		if (player != null)
			LevelController.SetMazeCompleted();
	}
}
