using UnityEditor;
using UnityEngine;

using AnythingWorld.Behaviours;

namespace AnythingWorld.Editors
{
    /// <summary>
    /// General custom editor for AWBehaviours (Usually overidden)
    /// </summary>
    [CustomEditor(typeof(AWBehaviour), true)]
    public class AWBehaviourEditor : Editor
    {
        #region Fields
        private AWBehaviour controller;
        #endregion

        #region Unity Callbacks
        private void OnEnable()
        {
            controller = (AWBehaviour)target;

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
