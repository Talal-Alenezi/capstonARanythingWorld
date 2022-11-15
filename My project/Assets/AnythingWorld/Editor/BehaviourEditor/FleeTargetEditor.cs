using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for the FleeTarget behaviour script.
/// </summary>
[CustomEditor(typeof(FleeTarget))]
public class FleeTargetEditor : Editor
{
    #region Fields
    private FleeTarget controller;
    private float size = 1f;
    #endregion

    #region Unity Callbacks
    private void OnEnable()
    {
        controller = (FleeTarget)target;
    }
    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Transform transform = ((FleeTarget)target).transform;
            controller.GetTargetTransformVector();
            Vector3 targetPos = controller.GetTargetTransformVector();
            Handles.color = new Color(0.5f, 0f, 0.5f, 0.5f);
            Handles.DrawSolidDisc(targetPos, new Vector3(0, 1, 0), controller.TargetRadius);
            Handles.color = Handles.xAxisColor;
            Handles.SphereHandleCap(0, targetPos, transform.rotation, size, EventType.Repaint);
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh Animator"))
        {
            controller.AddAWAnimator();
        }
    }
    #endregion

}
