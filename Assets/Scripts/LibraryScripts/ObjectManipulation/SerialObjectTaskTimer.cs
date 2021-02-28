using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialObjectTaskTimer : MonoBehaviour
{
    public class ObjectTaskList
    {
        public ObjectTaskList(List<GameObject> objectsList, float delay)
		{
            objects = objectsList;
            objectTaskDelay = delay;
		}

        public ObjectTaskList(float delay)
        {
            objects = new List<GameObject>();
            objectTaskDelay = delay;
        }

        public List<GameObject> objects = new List<GameObject>();
        public float objectTaskDelay;
    }

    public delegate void AllListsCompleted();
    public event AllListsCompleted OnAllListsCompleted;

    public delegate void StartingNewList();
    public event StartingNewList OnBeginNewObjectsList;

    public delegate void ObjectTaskPing(GameObject obj);
    public event ObjectTaskPing OnObjectPing;

    float timeUntilNextTask = 0.0f;
    float timeSinceLastTask = 0.0f;
    int currentObjectList = 0;
    public int CurrentObjectList
    {
        get
        {
            return currentObjectList;
        }
        private set
        {
            currentObjectList = value;
            timeSinceLastTask = 0.0f;
            if (currentObjectList < objectLists.Count)
            {
                OnBeginNewObjectsList?.Invoke();
                timeUntilNextTask = objectLists[currentObjectList].objectTaskDelay;
            }
            else
            {
                OnAllListsCompleted?.Invoke();
            }
        }
    }

    public readonly List<ObjectTaskList> objectLists = new List<ObjectTaskList>();

    public void Clear()
	{
        foreach (ObjectTaskList list in objectLists)
		{
            list.objects.Clear();
		}
        objectLists.Clear();
	}

    public void SetListRevealDelay(int listIndex, float delay)
	{
        objectLists[listIndex].objectTaskDelay = delay;
	}
    public void AddObjectList(ObjectTaskList objectList)
    {
        objectLists.Add(objectList);

    }

    public void BeginObjectTasks()
    {
        CurrentObjectList = 0;
    }

    public bool AreObjectTasksComplete()
    {
        return CurrentObjectList == objectLists.Count - 1;
    }

    public void AddObjectToObjectsList(GameObject obj, int listIndex)
    {
        if (obj != null && listIndex < objectLists.Count && listIndex >= 0)
        {
            objectLists[listIndex].objects.Add(obj);
        }
    }

    void Update()
    {
        if (CurrentObjectList < objectLists.Count)
        {
            timeSinceLastTask += Time.deltaTime;
            if (timeSinceLastTask >= timeUntilNextTask)
            {
                timeSinceLastTask = 0.0f;
                if (objectLists[currentObjectList].objects.Count > 0)
                {
                    OnObjectPing?.Invoke(objectLists[CurrentObjectList].objects[0]);
                    objectLists[CurrentObjectList].objects.RemoveAt(0);
                }
                if (objectLists[CurrentObjectList].objects.Count == 0)
                {
                    ++CurrentObjectList;
                }
            }
        }
    }
}
