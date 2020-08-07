using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectVisibilityController : MonoBehaviour
{
	public enum VisibilityState
	{
		RevealingObjects,
		RevealingCameras,
		RevealingPanel,
		Revealed,
		NumStates
	}


	public delegate void ObjectsRevealed();
	public event ObjectsRevealed OnObjectsRevealed;

	public GameObject goalCubePrefab;
	public GameObject playerCubePrefab;

	public GameObject rotatorCubePrefab;

	public float panelRevealDelay = 0.35f; 
	public float camRevealDelay = 0.35f;
	public float initialRevealTime = 0.2f;
	public float revealDelayGrowth = 1.6f;
	public float maxRevealDelay = 0.8f;

	List<GameObject> camerasToReveal = new List<GameObject>();
	List<GameObject> mazeBlocksToReveal = new List<GameObject>();
	float timeUntilNextReveal = 0.0f;
	float timeSinceLastReveal = 0.0f;

	VisibilityState _visibilityState = VisibilityState.Revealed;
	public VisibilityState visibilityState
	{
		get { return _visibilityState; }
		set
		{
			_visibilityState = value;
			if (_visibilityState == VisibilityState.RevealingCameras)
			{
				timeSinceLastReveal = 0.0f;
				timeUntilNextReveal = camRevealDelay;
			}
			else if (_visibilityState == VisibilityState.RevealingObjects)
			{
				timeSinceLastReveal = 0.0f;
				timeUntilNextReveal = initialRevealTime;
			}
			else if (_visibilityState == VisibilityState.RevealingPanel)
			{
				timeSinceLastReveal = 0.0f;
				timeUntilNextReveal = panelRevealDelay;
			}
			else if (_visibilityState == VisibilityState.Revealed)
			{
				if (UDLRCameraController.Instance != null)
				{
					UDLRCameraController.Instance.panelController.enabled = true; // panel will be the last thing that gets displayed
				}
				OnObjectsRevealed?.Invoke();
			}
		}
	}

	public void BeginObjectsReveal()
	{
		visibilityState = (VisibilityState)0;
	}

	public bool AreObjectsRevealed()
	{
		return visibilityState == VisibilityState.Revealed;
	}

	public void SetupLevel(MazeLevel levelData)
	{
		if (UDLRCameraController.Instance != null)
		{
			CamAnimator[] animators = UDLRCameraController.Instance.GetCameraAnimators();
			AddObjectToRevealList(animators[(int)CameraPanel.DisplayPosition.TopLeft].gameObject, ref camerasToReveal);
			AddObjectToRevealList(animators[(int)CameraPanel.DisplayPosition.TopRight].gameObject, ref camerasToReveal);
			AddObjectToRevealList(animators[(int)CameraPanel.DisplayPosition.BottomRight].gameObject, ref camerasToReveal);
			AddObjectToRevealList(animators[(int)CameraPanel.DisplayPosition.BottomLeft].gameObject, ref camerasToReveal);
		}
		foreach (RotatorTriggerData rotator in levelData.rotators)
		{
			GameObject rotatorTriggerObj = Instantiate(rotatorCubePrefab, rotator.position, Quaternion.identity);
			CameraRotatorTrigger rotatorTrigger = rotatorTriggerObj.GetComponent<CameraRotatorTrigger>();
			if (rotatorTrigger != null)
				rotatorTrigger.arcType = rotator.arcType;
			AddObjectToRevealList(rotatorTriggerObj, ref mazeBlocksToReveal);
		}
		if (InputHandler.Instance != null)
		{
			for (int panelPos = 0; panelPos < (int)CameraPanel.DisplayPosition.NumPositions; ++panelPos)
			{
				GameObject cube = Instantiate(playerCubePrefab, levelData.cubeStartPositions[panelPos], Quaternion.identity);
				CubeController cubeController = cube.GetComponent<CubeController>();
				Assert.IsNotNull(cubeController);
				InputHandler.Instance.SetCubeController(panelPos, cubeController);
				cubeController.associatedPanelPosition = (CameraPanel.DisplayPosition)panelPos;
				AddObjectToRevealList(cube, ref mazeBlocksToReveal);
				GameObject goal = Instantiate(goalCubePrefab, levelData.goalPositions[panelPos], Quaternion.identity);
				MazeGoal goalController = goal.GetComponent<MazeGoal>();
				Assert.IsNotNull(goalController);
				AddObjectToRevealList(goal, ref mazeBlocksToReveal);
				goalController.associatedDisplayPosition = (CameraPanel.DisplayPosition)panelPos;
				if (UDLRCameraController.Instance != null)
				{
					MeshRenderer meshRenderer = cube.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
						meshRenderer.material.SetColor("_EmissionColor", UDLRCameraController.Instance.GetCameraAnimators()[panelPos].GetCamObjColour());
					MeshRenderer goalMeshRenderer = goal.GetComponent<MeshRenderer>();
					if (goalMeshRenderer != null)
						goalMeshRenderer.material.SetColor("_EmissionColor", UDLRCameraController.Instance.GetCameraAnimators()[panelPos].GetCamObjColour());
				}
			}
		}
		if (UDLRCameraController.Instance != null)
		{
			UDLRCameraController.Instance.panelController.enabled = false; // panel will be the last thing that gets displayed
		}
	}

	void AddObjectToRevealList(GameObject obj, ref List<GameObject> list)
	{
		if (obj != null)
		{
			obj.SetActive(false);
			list.Add(obj);
		}
	}

	private void Update()
	{
		if (visibilityState == VisibilityState.RevealingCameras && camerasToReveal.Count > 0)
		{
			timeSinceLastReveal += Time.deltaTime;
			if (timeSinceLastReveal >= timeUntilNextReveal)
			{
				timeSinceLastReveal = 0.0f;
				camerasToReveal[0].SetActive(true);
				CameraPanel camPanel = camerasToReveal[0].GetComponentInChildren<CameraPanel>();
				if (camPanel != null)
					camPanel.PostActivate();
				camerasToReveal.RemoveAt(0);

				if (camerasToReveal.Count == 0)
				{
					IncrementVisibilityState();
				}
			}
		}
		if (visibilityState == VisibilityState.RevealingObjects && mazeBlocksToReveal.Count > 0)
		{
			timeSinceLastReveal += Time.deltaTime;
			if (timeSinceLastReveal >= timeUntilNextReveal)
			{
				timeSinceLastReveal = 0.0f;
				if (timeUntilNextReveal < maxRevealDelay)
					timeUntilNextReveal *= revealDelayGrowth;
				mazeBlocksToReveal[0].SetActive(true);
				mazeBlocksToReveal.RemoveAt(0);

				if (mazeBlocksToReveal.Count == 0)
				{
					IncrementVisibilityState();
				}
			}
		}
		if (visibilityState == VisibilityState.RevealingPanel)
		{
			timeSinceLastReveal += Time.deltaTime;
			if (timeSinceLastReveal >= timeUntilNextReveal)
			{
				timeSinceLastReveal = 0.0f;
				IncrementVisibilityState();
			}
		}
	}

	void IncrementVisibilityState() { visibilityState = (VisibilityState)(((int)visibilityState + 1) % (int)VisibilityState.NumStates); }
}
