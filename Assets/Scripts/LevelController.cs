using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public enum LayoutMode
{
	CentredPanels,
	PerspectiveCentre
};

[CreateAssetMenu(menuName = "Maze Assets/World Settings")]
public class WorldSettings : ScriptableObject
{
//	[SerializeField] private LevelController.LayoutMode LayoutMode = LevelController.LayoutMode.CentredPanels;
//	[SerializeField] private int WorldExtent = 2;

	public LayoutMode layoutMode;
	public int worldExtent;
}

public class LevelController : MonoBehaviour
{
	public enum MazeState
	{
		Starting = 0,
		Revealing,
		InProgress,
		Finishing,
		LoadingLevel,
		NumStates
	}

	public static LayoutMode layout = LayoutMode.PerspectiveCentre;
	public static int WORLD_CUBE_LIMIT = 2;

	public static LevelController Instance;

	//public MazeLevelCollection LevelCollection;
	//public WorldSettings worldSettings;
	public GameObject inputHandlerPrefab;
	public GameObject udlrCamControllerPrefab;
	public GameObject mazeLineCubePrefab;

	public delegate void MazeStateChanged(MazeState state);
	public event MazeStateChanged OnMazeStateChanged;

	ObjectVisibilityController visibilityController;

	int levelIndex = 0;

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
					visibilityController.BeginObjectsReveal();
				}
				if (_mazeState == MazeState.InProgress)
				{
					UDLRCameraController.StartPulsingSelectedCamera();
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

		visibilityController = GetComponent<ObjectVisibilityController>();
		Assert.IsNotNull(visibilityController);

		//WORLD_CUBE_LIMIT = worldSettings.worldExtent;
		//layout = worldSettings.layoutMode;
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
		visibilityController.OnObjectsRevealed += OnObjectsRevealed;
	}

	private void OnDestroy()
	{
		visibilityController.OnObjectsRevealed -= OnObjectsRevealed;
	}

	public static bool AreAllObjectsRevealed()
	{
		if (Instance != null)
			return Instance.visibilityController.AreObjectsRevealed();
		return false;
	}

	void OnObjectsRevealed()
	{
		if (AreAllWorkersComplete(mazeState))
			IncrementMazeState();
	}

	void LoadNextLevel()
	{
		ClearLevel();
		//if (levelIndex < LevelCollection.levels.Count)
		//{
			//MazeLevel level = LevelCollection.levels[levelIndex];
			visibilityController.SetupLevel(/*level*/);
			//levelIndex = levelIndex + 1 % LevelCollection.levels.Count;

			// This happens before Start() is called on some components, so we have to force their 
			// MazeStateChanged callback whenever they register themselves. Which isn't ideal.
			mazeState = MazeState.Starting;
		//}
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

		if (state == MazeState.Revealing && Instance.visibilityController.AreObjectsRevealed())
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
