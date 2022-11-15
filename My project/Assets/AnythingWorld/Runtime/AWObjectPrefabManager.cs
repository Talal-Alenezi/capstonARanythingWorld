using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class AWObjectPrefabManager : MonoBehaviour
{
    void Update()
    {
        var eventType = Event.current.type;
        if (eventType == EventType.DragUpdated ||
            eventType == EventType.DragPerform)
        {
            // Show a copy icon on the drag
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                Debug.Log("Drag peformed");
                DragAndDrop.AcceptDrag();
            }
            if(eventType == EventType.MouseDrag)
            {
                Debug.Log("Mouse drag");
            }
            Event.current.Use();
        }
    }
}
#endif