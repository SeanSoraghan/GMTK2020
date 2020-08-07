using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
	/** This will stop the camera panel from moving when a cube enters this trigger */
	public bool stopPanelMovement = false;
	public bool RemoveOnTrigger = false;

	CubeController playerController;
	bool triggered = false;
    public virtual void PlayerEnteredTrigger(CubeController player)
	{

	}

	public virtual void PlayerExitedTrigger(CubeController player)
	{

	}

	private void OnDestroy()
	{
		if (playerController != null)
			playerController.stopPanelMovement = false;
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
			if (stopPanelMovement)
				player.stopPanelMovement = true;
			playerController = player;
			player.OnMovementEnded += OnPlayerMovementEnded;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		CubeController player = other.GetComponent<CubeController>();
		if (player != null)
		{
			player.stopPanelMovement = false;
			player.OnMovementEnded -= OnPlayerMovementEnded;
			PlayerExitedTrigger(player);
			playerController = null;
			triggered = false;
		}
	}
}
