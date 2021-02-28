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
	public float blocksInitialRevealDelay = 0.2f;
	public float blocksRevealDelayGrowth = 1.6f;
	public float maxRevealDelay = 0.5f;

	public SerialObjectTaskTimer objectRevealTimer;

	VisibilityState _visibilityState = VisibilityState.Revealed;
	public VisibilityState visibilityState
	{
		get { return _visibilityState; }
		set
		{
			_visibilityState = value;
			if (_visibilityState == VisibilityState.Revealed)
			{
				OnObjectsRevealed?.Invoke();
			}
		}
	}

	private void Awake()
	{
		objectRevealTimer = gameObject.AddComponent<SerialObjectTaskTimer>();
		objectRevealTimer.OnObjectPing += ObjectPing;
		objectRevealTimer.OnBeginNewObjectsList += BeginningObjectRevealList;
		objectRevealTimer.OnAllListsCompleted += AllRevealListsComplete;
	}
	private void OnDestroy()
	{
		objectRevealTimer.OnAllListsCompleted -= AllRevealListsComplete;
		objectRevealTimer.OnBeginNewObjectsList -= BeginningObjectRevealList;
		objectRevealTimer.OnObjectPing -= ObjectPing;
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
		objectRevealTimer.Clear();
		objectRevealTimer.AddObjectList(new SerialObjectTaskTimer.ObjectTaskList(blocksInitialRevealDelay));
		objectRevealTimer.AddObjectList(new SerialObjectTaskTimer.ObjectTaskList(camRevealDelay));
		objectRevealTimer.AddObjectList(new SerialObjectTaskTimer.ObjectTaskList(panelRevealDelay));

		if (UDLRCameraController.Instance != null)
		{
			CamAnimator[] animators = UDLRCameraController.Instance.GetCameraAnimators();
			CameraPanel.DisplayPosition[] camRevealOrder = { CameraPanel.DisplayPosition.TopLeft, CameraPanel.DisplayPosition.TopRight,
															CameraPanel.DisplayPosition.BottomRight, CameraPanel.DisplayPosition.BottomLeft };
			for (int i = 0; i < camRevealOrder.Length; ++i)
			{
				foreach (CamAnimator camAnim in animators)
				{
					if (camAnim != null && camAnim.CameraPanel.camPosition == camRevealOrder[i])
					{
						HideObjectAndAddToRevealList(camAnim.gameObject, (int)VisibilityState.RevealingCameras);
						break;
					}
				}
			}
		}
		foreach (RotatorTriggerData rotator in levelData.rotators)
		{
			GameObject rotatorTriggerObj = Instantiate(rotatorCubePrefab, rotator.position, Quaternion.identity);
			CameraRotatorTrigger rotatorTrigger = rotatorTriggerObj.GetComponent<CameraRotatorTrigger>();
			if (rotatorTrigger != null)
				rotatorTrigger.arcType = rotator.arcType;
			HideObjectAndAddToRevealList(rotatorTriggerObj, (int)VisibilityState.RevealingObjects);
		}
		if (InputHandler.Instance != null)
		{
			for (int panelPos = 0; panelPos < (int)CameraPanel.DisplayPosition.NumPositions; ++panelPos)
			{
				if (UDLRCameraController.Instance.GetCameraAnimators()[panelPos] != null)
				{
					GameObject cube = Instantiate(playerCubePrefab, levelData.cubeStartPositions[panelPos], Quaternion.identity);
					CubeController cubeController = cube.GetComponent<CubeController>();
					Assert.IsNotNull(cubeController);
					InputHandler.Instance.SetCubeController(panelPos, cubeController);
					cubeController.associatedPanelPosition = (CameraPanel.DisplayPosition)panelPos;
					cube.layer = LayerMask.NameToLayer(CameraPanel.PositionLayerNames[panelPos]);
					HideObjectAndAddToRevealList(cube, (int)VisibilityState.RevealingObjects);
					GameObject goal = Instantiate(goalCubePrefab, levelData.goalPositions[panelPos], Quaternion.identity);
					MazeGoal goalController = goal.GetComponent<MazeGoal>();
					Assert.IsNotNull(goalController);
					goal.layer = LayerMask.NameToLayer(CameraPanel.PositionLayerNames[panelPos]);
					HideObjectAndAddToRevealList(goal, (int)VisibilityState.RevealingObjects);
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
		}
		if (UDLRCameraController.Instance != null)
		{
			UDLRCameraController.Instance.panelController.enabled = false;
			objectRevealTimer.AddObjectToObjectsList(UDLRCameraController.Instance.gameObject, (int)VisibilityState.RevealingPanel);
		}
		objectRevealTimer.BeginObjectTasks();
	}

	void HideObjectAndAddToRevealList(GameObject obj, int listIndex)
	{
		if (obj != null)
		{
			obj.SetActive(false);
			objectRevealTimer.AddObjectToObjectsList(obj, listIndex);
		}
	}

	void ObjectPing(GameObject obj)
	{
		if (objectRevealTimer.CurrentObjectList == (int)VisibilityState.RevealingObjects)
		{
			obj.SetActive(true);
			float currentDelay = objectRevealTimer.objectLists[objectRevealTimer.CurrentObjectList].objectTaskDelay;
			if (currentDelay < maxRevealDelay)
				objectRevealTimer.SetListRevealDelay(objectRevealTimer.CurrentObjectList, Mathf.Clamp(currentDelay * blocksRevealDelayGrowth, 0.0f, maxRevealDelay));
		}
		else if (objectRevealTimer.CurrentObjectList == (int)VisibilityState.RevealingCameras)
		{
			obj.SetActive(true);
			CameraPanel camPanel = obj.GetComponentInChildren<CameraPanel>();
			if (camPanel != null)
				camPanel.PostActivate();
		}
		else if (objectRevealTimer.CurrentObjectList == (int)VisibilityState.RevealingPanel)
		{
			if (UDLRCameraController.Instance != null)
			{
				UDLRCameraController.Instance.panelController.enabled = true;
			}
		}
	}

	void BeginningObjectRevealList()
	{
		visibilityState = (VisibilityState)objectRevealTimer.CurrentObjectList;
	}

	void AllRevealListsComplete()
	{
		visibilityState = VisibilityState.Revealed;
	}
}
