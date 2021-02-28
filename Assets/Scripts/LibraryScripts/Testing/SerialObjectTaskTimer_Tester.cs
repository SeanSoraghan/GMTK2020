using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialObjectTaskTimer_Tester : MonoBehaviour
{
    SerialObjectTaskTimer timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = gameObject.AddComponent<SerialObjectTaskTimer>();
        for (int listIndex = 0; listIndex < 5; ++listIndex)
        {
            SerialObjectTaskTimer.ObjectTaskList objectList = new SerialObjectTaskTimer.ObjectTaskList(0.1f + 0.1f * listIndex);
            for (int objIndex = 0; objIndex < 5; ++objIndex)
            {
                objectList.objects.Add(new GameObject("List " + listIndex + " Object " + objIndex));
            }
            timer.AddObjectList(objectList);
        }
        timer.OnObjectPing += ObjectPing;
        timer.OnAllListsCompleted += OnObjectsCompleted;
        timer.BeginObjectTasks();
    }

    // Update is called once per frame
    void ObjectPing(GameObject obj)
    {
        Debug.Log(obj.name + " ping");
    }

    void OnObjectsCompleted()
    {
        Debug.Log("complete");
    }
}
