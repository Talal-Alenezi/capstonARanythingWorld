using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PreyBehaviourEditor : Editor
{
    #region Fields
    private PreyBehaviour controller;
    private float size = 1f;
    #endregion

    private void OnEnable()
    {
        controller = (PreyBehaviour)target;
    }
    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {



            
            Transform transform = ((PreyBehaviour)target).transform;
            Vector3 targetPos = controller.targetPos;
            Handles.color = new Color(0.5f, 0f, 0.5f, 0.5f);
            Handles.DrawSolidDisc(targetPos, new Vector3(0, 1, 0), 5);
            Handles.color = Handles.xAxisColor;
            Handles.SphereHandleCap(0, targetPos, transform.rotation, size, EventType.Repaint);
            


        }
    }


}