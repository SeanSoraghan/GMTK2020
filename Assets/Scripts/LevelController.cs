using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
	public enum MazeState
	{
		Starting = 0,
		InProgress,
		Finishing,
		LoadingLevel,
		NumStates
	}

	public static LevelController Instance;

	public delegate void MazeStateChanged(MazeState state);
	public event MazeStateChanged OnMazeStateChanged;

	// A collection of components who have work to do during a maze state (e.g. animations and such).
	// Mainly used with Starting and Finishing.

	Dictionary<Component, bool>[] workerFlags = 
	{
		new Dictionary<Component, bool>(),
		new Dictionary<Component, bool>(),
		new Dictionary<Component, bool>(),
		new Dictionary<Component, bool>()
	};

	MazeState _mazeState = MazeState.LoadingLevel;
	MazeState mazeState
	{
		get { return _mazeState; }
		set
		{
			_mazeState = value;
			if (_mazeState == MazeState.LoadingLevel)
			{
				LoadNextLevel();
			}
			OnMazeStateChanged?.Invoke(_mazeState);
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

	private void Start()
	{
		mazeState = MazeState.Starting;
	}

	void LoadNextLevel()
	{
		for (int i = 0; i < (int)MazeState.NumStates; ++i)
		{
			workerFlags[i].Clear();
		}
		Debug.Log("This is where I'd load a new level");
		SceneManager.sceneLoaded += SceneLoaded;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// This happens before Start() is called on some components, so we have to force their 
		// MazeStateChanged callback whenever they register themselves. Which isn't ideal.
		mazeState = MazeState.Starting;
		SceneManager.sceneLoaded -= SceneLoaded;
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
		Instance.workerFlags[(int)state][component] = true;
		foreach(var pair in Instance.workerFlags[(int)state])
		{
			if (!pair.Value)
				return;
		}
		Instance.mazeState = (MazeState)(((int)Instance.mazeState + 1) % (int)MazeState.NumStates);
	}
}
