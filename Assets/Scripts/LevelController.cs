using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Maze Assets/World Settings")]
public class WorldSettings : ScriptableObject
{
	[SerializeField] private LevelController.LayoutMode LayoutMode = LevelController.LayoutMode.CentredPanels;
	[SerializeField] private int WorldExtent = 2;

	public LevelController.LayoutMode layoutMode => LayoutMode;
	public int worldExtent => WorldExtent;
}

public class LevelController : MonoBehaviour
{
	public enum LayoutMode
	{
		CentredPanels,
		PerspectiveCentre
	}

	public enum MazeState
	{
		Starting = 0,
		Revealing,
		InProgress,
		Finishing,
		LoadingLevel,
		NumStates
	}

	public static LayoutMode layout = LayoutMode.CentredPanels;
	public static int WORLD_CUBE_LIMIT = 2;

	static float initialRevealTime = 0.2f;
	static float revealDelayGrowth = 1.6f;

	public static LevelController Instance;

	public MazeLevelCollection LevelCollection;
	public WorldSettings worldSettings;
	public GameObject inputHandlerPrefab;
	public GameObject udlrCamControllerPrefab;
	public GameObject mazeLineCubePrefab;
	public GameObject goalCubePrefab;
	public GameObject playerCubePrefab;
	public GameObject rotatorCubePrefab;

	public delegate void MazeStateChanged(MazeState state);
	public event MazeStateChanged OnMazeStateChanged;

	int levelIndex = 0;

	List<GameObject> objectsToReveal = new List<GameObject>();
	float timeUntilNextReveal = initialRevealTime;
	float timeSinceLastReveal = 0.0f;

	// A collection of components who have work to do during a maze state (e.g. animations and such).
	// Mainly used with Starting and Finishing.

	List<Dictionary<Component, bool>> workerFlags = new List<Dictionary<Component, bool>>();

	MazeState _mazeState = MazeState.LoadingLevel;
	MazeState mazeState
	{
		get { return _mazeState; }
		set
		{
			if (_mazeState != value)
			{
				_mazeState = value;
				if (_mazeState == MazeState.LoadingLevel)
				{
					LoadNextLevel();
				}
				if (_mazeState == MazeState.Starting)
				{
					timeSinceLastReveal = 0.0f;
					timeUntilNextReveal = initialRevealTime;
				}
				OnMazeStateChanged?.Invoke(_mazeState);
			}
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}

		WORLD_CUBE_LIMIT = worldSettings.worldExtent;
		layout = worldSettings.layoutMode;
		Instantiate(inputHandlerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		Instantiate(udlrCamControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		Instantiate(mazeLineCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);

		for (int i = 0; i < (int)MazeState.NumStates; ++i)
		{
			workerFlags.Add(new Dictionary<Component, bool>());
		}
	}

	private void Start()
	{
		LoadNextLevel();
	}

	private void Update()
	{
		if (mazeState == MazeState.Revealing && objectsToReveal.Count > 0)
		{
			timeSinceLastReveal += Time.deltaTime;
			if (timeSinceLastReveal >= timeUntilNextReveal)
			{
				timeSinceLastReveal = 0.0f;
				timeUntilNextReveal *= revealDelayGrowth;
				objectsToReveal[0].SetActive(true);
				objectsToReveal.RemoveAt(0);

				if (objectsToReveal.Count == 0)
				{
					if (AreAllWorkersComplete(mazeState))
						IncrementMazeState();
				}
			}
		}
	}

	void LoadNextLevel()
	{
		ClearLevel();
		if (levelIndex < LevelCollection.levels.Count)
		{
			LoadLevel();

			// This happens before Start() is called on some components, so we have to force their 
			// MazeStateChanged callback whenever they register themselves. Which isn't ideal.
			mazeState = MazeState.Starting;
		}
	}

	void ClearLevel()
	{
		for (int i = 0; i < (int)MazeState.NumStates; ++i)
		{
			List<Component> keys = new List<Component>(workerFlags[i].Keys);
			foreach(Component key in keys)
			{
				workerFlags[i][key] = false;
			}
		}
		foreach (PlayerTrigger trigger in FindObjectsOfType<PlayerTrigger>())
		{
			Destroy(trigger.gameObject);
		}
		foreach (CubeController player in FindObjectsOfType<CubeController>())
		{
			Destroy(player.gameObject);
		}
	}

	void LoadLevel()
	{
		MazeLevel level = LevelCollection.levels[levelIndex];
		AddObjectToRevealList(Instantiate(goalCubePrefab, new Vector3(0, 0, 0), Quaternion.identity));
		foreach (RotatorTriggerData rotator in level.rotators)
		{
			GameObject rotatorTriggerObj = Instantiate(rotatorCubePrefab, rotator.position, Quaternion.identity);
			CameraRotatorTrigger rotatorTrigger = rotatorTriggerObj.GetComponent<CameraRotatorTrigger>();
			if (rotatorTrigger != null)
				rotatorTrigger.arcType = rotator.arcType;
			AddObjectToRevealList(rotatorTriggerObj);
		}
		AddObjectToRevealList(Instantiate(playerCubePrefab, level.playerStart, Quaternion.identity));
		++levelIndex;
	}

	void AddObjectToRevealList(GameObject obj)
	{
		if (obj != null)
		{
			obj.SetActive(false);
			objectsToReveal.Add(obj);
		}
	}

	public static bool IsMazeCompleted()
	{
		return Instance? Instance.mazeState == MazeState.Finishing : false;
	}

	public static MazeState GetMazeState()
	{
		if (Instance != null)
			return Instance.mazeState;
		return MazeState.LoadingLevel;
	}
	public static void SetMazeCompleted()
	{
		if (Instance != null && Instance.mazeState == MazeState.InProgress)
		{
			Instance.mazeState = MazeState.Finishing;
		}
	}

	public static void RegisterMazeStateWorker(MazeState state, Component component)
	{
		if (Instance == null)
			return;
		if (!Instance.workerFlags[(int)state].ContainsKey(component))
			Instance.workerFlags[(int)state][component] = false;
	}

	public static void MazeStateWorkerComplete(MazeState state, Component component)
	{
		if (Instance == null)
			return;
		Assert.IsTrue(state == Instance.mazeState);
		Instance.workerFlags[(int)state][component] = true;

		if (state == MazeState.Revealing && Instance.objectsToReveal.Count > 0)
			return;

		if (Instance.AreAllWorkersComplete(state))
			Instance.IncrementMazeState();
	}

	bool AreAllWorkersComplete(MazeState state)
	{
		foreach (var pair in Instance.workerFlags[(int)state])
		{
			if (!pair.Value)
				return false;
		}
		return true;
	}

	void IncrementMazeState() { mazeState = (MazeState)(((int)mazeState + 1) % (int)MazeState.NumStates); }
}
