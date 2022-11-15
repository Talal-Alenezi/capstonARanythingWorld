using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for the FollowTarget behaviour.
/// </summary>
[CustomEditor(typeof(FollowTarget))]
public class FollowTargetEditor : Editor
{
    #region Fields
    private FollowTarget controller;
    private float size = 1f;
    #endregion

    #region Unity Callbacks
    private void OnEnable()
    {
        controller = (FollowTarget)target;
    }
    private void OnSceneGUI()
    {

        if (controller.TargetTransform != null)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Transform transform = ((FollowTarget)target).transform;

                Vector3 targetPos = controller.TargetTransform.position;
                Handles.color = new Color(0.5f, 0f, 0.5f, 0.5f);
                Handles.DrawSolidDisc(targetPos, new Vector3(0, 1, 0), controller.TargetRadius);
                Handles.color = Handles.xAxisColor;
                Handles.SphereHandleCap(0, targetPos, transform.rotation, size, EventType.Repaint);

            }
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
