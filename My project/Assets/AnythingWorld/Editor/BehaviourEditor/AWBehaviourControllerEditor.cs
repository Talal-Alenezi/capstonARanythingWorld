using UnityEditor;
using UnityEngine;
using AnythingWorld.Behaviours;
using AnythingWorld.Utilities;

namespace AnythingWorld.Editors
{
    /// <summary>
    /// Custom inspector override for the AWBehaviourController script.
    /// </summary>
    [CustomEditor(typeof(AWBehaviourController))]
    [CanEditMultipleObjects]
    public class AWBehaviourControllerEditor : Editor
    {
        #region Fields
        //private string[] activeScripts = { "" };
        //private string[] availableScripts = { "" };
        private int activeIndex = 0;
        private int availableIndex = 0;
        private bool gravityOn;
        private AWBehaviourController behaviourController;
        #endregion

        #region Unity Callbacks
        public void OnEnable()
        {

            //Set controller to be current AWBehaviourController inspector instance.
            behaviourController = (AWBehaviourController)target;
            //Inital refresh of behaviours
            behaviourController.RefreshBehaviours();

            if (behaviourController.transform.childCount > 0)
            {
                if (behaviourController.awThing == null)
                {
                    if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning("No AWThing found, removing behaviour controller");
                    AnythingSafeDestroy.SafeDestroyDelayed(behaviourController);
                    return;
                }
            }
        }
        public override void OnInspectorGUI()
        {
            if (activeIndex > behaviourController.activeBehaviourStrings.Length) activeIndex = 0;

            GUILayout.Label("Current Behaviour(s)");
            GUILayout.BeginHorizontal();
            if (behaviourController.activeBehaviourStrings.Length > 0)
            {
                activeIndex = EditorGUILayout.Popup(activeIndex, behaviourController.activeBehaviourStrings);

                if (GUILayout.Button("Remove Behaviour"))
                {
                    behaviourController.RemoveBehaviour(behaviourController.activeBehaviourStrings[activeIndex]);
                }
            }
            else
            {
                GUILayout.Label("No behaviour scripts on object");
            }

            GUILayout.EndHorizontal();




            GUILayout.Label("Available Behaviour Scripts");
            GUILayout.BeginHorizontal();
            if (behaviourController.availableBehaviourStrings.Length > 0)
            {
                availableIndex = EditorGUILayout.Popup(availableIndex, behaviourController.availableBehaviourStrings);
                if (GUILayout.Button("Add Behaviour"))
                {
                    behaviourController.AddBehaviourScript(behaviourController.availableBehaviourStrings[availableIndex]);
                }
            }
            else
            {
                GUILayout.Label("No available scripts");
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Refresh Behaviours", GUILayout.ExpandHeight(false)))
            {
                RefreshBehaviours();
                //Debug.Log("Refreshed behaviours");
            }
        }
        #endregion

        #region Public Methods

        public void RefreshBehaviours()
        {
            behaviourController.RefreshBehaviours();
        }
        #endregion

    }
}
