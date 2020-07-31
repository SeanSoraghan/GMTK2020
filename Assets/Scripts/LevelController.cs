using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

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

	static float initialRevealTime = 0.2f;
	static float revealDelayGrowth = 1.6f;

	public static LevelController Instance;

	public delegate void MazeStateChanged(MazeState state);
	public event MazeStateChanged OnMazeStateChanged;

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
					foreach (PlayerTrigger trigger in FindObjectsOfType<PlayerTrigger>())
					{
						trigger.gameObject.SetActive(false);
						objectsToReveal.Add(trigger.gameObject);
					}
					foreach (CubeController player in FindObjectsOfType<CubeController>())
					{
						player.gameObject.SetActive(false);
						objectsToReveal.Add(player.gameObject);
					}
				}
				OnMazeStateChanged?.Invoke(_mazeState);
			}
		}
	}

	private void Awake()
	{
		for (int i = 0; i < (int)MazeState.NumStates; ++i)
		{
			workerFlags.Add(new Dictionary<Component, bool>());
		}

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
