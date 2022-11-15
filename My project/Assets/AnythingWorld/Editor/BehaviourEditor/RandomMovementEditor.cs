using UnityEditor;
using UnityEngine;

namespace AnythingWorld.Editors
{
    /// <summary>
    /// Custom editor for the RandomMovement behaviour.
    /// </summary>
    [CustomEditor(typeof(RandomMovement))]
    public class RandomMovementEditor : Editor
    {
        #region Fields
        private RandomMovement controller;
        private float size = 1f;
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            controller = (RandomMovement)target;
        }


        Vector3 check = new Vector3(0, 0, 0);
        private void OnSceneGUI()
        {

            Vector3 labelOffset = new Vector3(0f, 3f, 0f);
            if (controller.showInSceneControls)
            {
                Handles.color = new Color(0.5f, 0f, 0.5f, 0.5f);
                Handles.DrawSolidDisc(controller.AnchorPos + new Vector3(0f, 0.1f, 0f), new Vector3(0, 1, 0), controller.targetSpawnRadius);



                Handles.color = Color.magenta;
                EditorGUI.BeginChangeCheck();
                Vector3 newAnchorPos = Handles.PositionHandle(controller.AnchorPos, controller.transform.rotation);


                GUIStyle inSceneLabelStyle = new GUIStyle(EditorStyles.label);
                inSceneLabelStyle.alignment = TextAnchor.MiddleCenter;
                inSceneLabelStyle.font = AnythingEditor.POPPINS_MEDIUM;
                inSceneLabelStyle.normal.textColor = Color.white;
                inSceneLabelStyle.fontSize = 12;

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed RandomMovement anchor position");
                    controller.AnchorPos = newAnchorPos;
                }
                Handles.Label(controller.AnchorPos + labelOffset, "Anchor Position", inSceneLabelStyle);



                Handles.SphereHandleCap(0, controller.randomTargetPoint, Quaternion.identity, size, EventType.Repaint);
                Handles.Label(controller.randomTargetPoint + labelOffset, "Target Point", inSceneLabelStyle);

                Vector3 radiusLabelPos = new Vector3(controller.AnchorPos.x + controller.targetSpawnRadius, controller.AnchorPos.y, controller.AnchorPos.z);
                Vector3 radiusLabelVec = (controller.AnchorPos - radiusLabelPos).normalized;
                Handles.color = Color.cyan;
                EditorGUI.BeginChangeCheck();
                Vector3 radiusAdjustment = Handles.Slider(radiusLabelPos, radiusLabelVec, 2f, Handles.SphereHandleCap, 0.01f);
                if (EditorGUI.EndChangeCheck())
                {
                    controller.targetSpawnRadius = radiusAdjustment.x - controller.AnchorPos.x;
                }

                Handles.DrawLine(radiusLabelPos, controller.AnchorPos);
                Handles.color = Color.white;
                Vector3 midpoint = new Vector3(radiusLabelPos.x / 2, radiusLabelPos.y, radiusLabelPos.z);
                Handles.Label(radiusLabelPos + labelOffset * 1.5f, "Target Spawn Radius", inSceneLabelStyle);
                Handles.Label(midpoint - labelOffset * 0.5f, controller.targetSpawnRadius.ToString("#.##"), inSceneLabelStyle);
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
}

