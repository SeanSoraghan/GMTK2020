﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
	public bool RemoveOnTrigger = false;

	CubeController playerController;
	bool triggered = false;
    public virtual void PlayerEnteredTrigger(CubeController player)
	{

	}

	void OnPlayerMovementEnded()
	{
		if (playerController != null && !triggered)
		{
			PlayerEnteredTrigger(playerController);
			triggered = true;
			if (RemoveOnTrigger)
			{
				playerController.OnMovementEnded -= OnPlayerMovementEnded;
				Destroy(gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		CubeController player = other.GetComponent<CubeController>();
		if (player != null)
		{
			playerController = player;
			player.OnMovementEnded += OnPlayerMovementEnded;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		CubeController player = other.GetComponent<CubeController>();
		if (player != null)
		{
			player.OnMovementEnded -= OnPlayerMovementEnded;
			playerController = null;
			triggered = false;
		}
	}
}
