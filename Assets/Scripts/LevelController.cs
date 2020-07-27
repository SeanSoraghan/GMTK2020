using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
	public static LevelController Instance;

	public delegate void MazeCompleted();
	public event MazeCompleted OnMazeCompleted;

	// A collection of components who have work to do after the maze is completed (e.g. animations and such).
	// When the last one is flagged as complete, the next level will load.
	Dictionary<Component, bool> postMazeCompletedWorkerFlags = new Dictionary<Component, bool>();

	bool _mazeComplete = false;
	bool mazeComplete
	{
		get { return _mazeComplete; }
		set
		{
			_mazeComplete = value;
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
	}

	void LoadLevel()
	{
		mazeComplete = false;
		postMazeCompletedWorkerFlags.Clear();
		Debug.Log("This is where I'd load a new level");
	}

	public static bool IsMazeCompleted()
	{
		return Instance? Instance.mazeComplete : false;
	}

	public static void SetMazeCompleted()
	{
		if (Instance != null && !Instance.mazeComplete)
		{
			Instance.mazeComplete = true;
			Instance.OnMazeCompleted?.Invoke();
		}
	}

	public static void RegisterPostMazeCompleteWorker(Component component)
	{
		if (Instance == null)
			return;
		if (!Instance.postMazeCompletedWorkerFlags.ContainsKey(component))
			Instance.postMazeCompletedWorkerFlags[component] = false;
	}

	public static void PostMazeCompleteWorkDone(Component component)
	{
		if (Instance == null)
			return;
		Instance.postMazeCompletedWorkerFlags[component] = true;
		foreach(var pair in Instance.postMazeCompletedWorkerFlags)
		{
			if (!pair.Value)
				return;
		}
		Instance.LoadLevel();
	}
}
