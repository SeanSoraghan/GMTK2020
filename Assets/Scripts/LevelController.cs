using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public enum MazeState
	{
		Starting = 0,
		InProgress,
		Finishing,
		LoadingLevel
	}

	public static LevelController Instance;

	public delegate void MazeCompleted();
	public event MazeCompleted OnMazeCompleted;

	public delegate void MazeStarted();
	public event MazeStarted OnMazeStarted;

	// A collection of components who have work to do after the maze is completed (e.g. animations and such).
	// When the last one is flagged as complete, the next level will load.
	Dictionary<Component, bool> mazeFinishingWorkerFlags = new Dictionary<Component, bool>();
	// A collection of components who have work to do while the maze is starting
	Dictionary<Component, bool> mazeStartingWorkerFlags = new Dictionary<Component, bool>();

	MazeState _mazeState = MazeState.Starting;
	MazeState mazeState
	{
		get { return _mazeState; }
		set
		{
			_mazeState = value;
			if (_mazeState == MazeState.Finishing)
			{
				OnMazeCompleted?.Invoke();
			}
			else if (_mazeState == MazeState.Starting)
			{
				OnMazeStarted?.Invoke();
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
		mazeState = MazeState.Starting;
	}

	void LoadLevel()
	{
		mazeState = MazeState.LoadingLevel;
		mazeStartingWorkerFlags.Clear();
		mazeFinishingWorkerFlags.Clear();
		Debug.Log("This is where I'd load a new level");
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

	public static void RegisterMazeFinishingWorker(Component component)
	{
		if (Instance == null)
			return;
		if (!Instance.mazeFinishingWorkerFlags.ContainsKey(component))
			Instance.mazeFinishingWorkerFlags[component] = false;
	}

	public static void RegisterMazeStartingWorker(Component component)
	{
		if (Instance == null)
			return;
		if (!Instance.mazeStartingWorkerFlags.ContainsKey(component))
			Instance.mazeStartingWorkerFlags[component] = false;
	}

	public static void MazeFinishingWorkerDone(Component component)
	{
		if (Instance == null)
			return;
		Instance.mazeFinishingWorkerFlags[component] = true;
		foreach(var pair in Instance.mazeFinishingWorkerFlags)
		{
			if (!pair.Value)
				return;
		}
		Instance.LoadLevel();
	}

	public static void MazeStartingWorkerDone(Component component)
	{
		if (Instance == null)
			return;
		Instance.mazeStartingWorkerFlags[component] = true;
		foreach (var pair in Instance.mazeStartingWorkerFlags)
		{
			if (!pair.Value)
				return;
		}
		Instance.mazeState = MazeState.InProgress;
	}
}
