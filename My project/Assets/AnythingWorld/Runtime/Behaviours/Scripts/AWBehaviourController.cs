using AnythingWorld.Animation;
using AnythingWorld.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Behaviours
{
    [ExecuteInEditMode]
    [System.Serializable]
    ///<summary>
    ///Handles the adding and removing of AWBehaviours an AWObj.
    ///</summary>
    public class AWBehaviourController : MonoBehaviour
    {
        #region Fields
        //Container in which object is spawned
        public AWObj awObj;
        //Parent object for all limbs
        public GameObject awThing;
        private AWBehaviour[] activeBehaviours = null;
        public string[] availableBehaviourStrings = { "" };
        public string[] activeBehaviourStrings = { "" };
        private string prefabType = "";
        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            if (gameObject.TryGetComponent<AWObj>(out var awObjComponent))
            {
                awObj = awObjComponent;
            }
            else
            {
                Debug.Log($"No AWObj detected, cannot add behaviour controller");
                this.enabled = false;
                AnythingSafeDestroy.SafeDestroyDelayed(this);
                return;
            }
            awObj.onBehaviourRefreshMethod = RefreshBehaviours;
        }
        void Update()
        {
            if (!awThing)
            {
                GetObjectInContainer(gameObject.transform, "AWThing");
            }

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Add behaviour script to child awThing object.
        /// </summary>
        /// <remarks>
        /// Behaviour script will only be added if base type is AWBehaviour.
        /// </remarks>
        /// <param name="scriptName">Name of script to add to </param>
        public void AddBehaviourScript(string scriptName)
        {
            var behaviour = System.Type.GetType(scriptName);
            try
            {
                if (behaviour != null && behaviour.BaseType == typeof(AWBehaviour))
                {
                    awObj.AddBehaviourAsync(behaviour);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);

            }

        }


        public AWBehaviour AddBehaviourScript<T>() where T : AWBehaviour
        {
            try
            {
                return awObj.AddBehaviour<T>();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }



        }
        /// <summary>
        /// Removes behaviour matching script name from awThing model.
        /// </summary>
        /// <param name="scriptName">Name of script of thing to remove.</param>
        public void RemoveBehaviour(string scriptName)
        {
            //Debug.Log($"Removing behaviour {scriptName} from model {gameObject.name}");

            var behaviourType = System.Type.GetType(scriptName);
            if (awObj.behaviourObject.TryGetComponent(behaviourType, out var outputScript))
            {
                var behaviourScriptToDestroy = (AWBehaviour)outputScript;
                behaviourScriptToDestroy.RemoveAWAnimator();
                AnythingSafeDestroy.SafeDestroyImmediate(behaviourScriptToDestroy);
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log($"Removed behaviour {scriptName} from {gameObject.name}", awObj.gameObject);
                RefreshBehaviours();
            }
            else
            {
                //Debug.LogError($"No {scriptName} script to remove on {awObj.gameObject.name}", awObj.gameObject);
            }
        }


        public void RemoveBehaviourScript<T>() where T : AWBehaviour
        {
            AWBehaviour behaviourToRemove = GetComponentInChildren<T>();
            if (behaviourToRemove != null)
            {

            }
        }
        /// <summary>
        /// Finds all active behaviours on model.
        /// <summary>
        /// <remarks>
        /// Finds all scripts of type AWbehaviour in the child objects and adds to list.
        /// Updates string array of active scripts to pass to editor script.
        /// </remarks>
        public void FindActiveBehaviours()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return;
                try
                {
                    activeBehaviours = GetComponentsInChildren<AWBehaviour>();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Issue getting behaviours in children");
                    Debug.LogException(e);
                    return;
                }


                var scriptNames = new List<string>();
                foreach (var script in activeBehaviours)
                {
                    scriptNames.Add(script.GetType().ToString());
                }
                activeBehaviourStrings = scriptNames.ToArray();
            };
#else
                if (this == null) return;
                try
                {
                    activeBehaviours = GetComponentsInChildren<AWBehaviour>();
                }
                catch(Exception e)
                {
                    Debug.LogError($"Issue getting behaviours in children");
                    Debug.LogException(e);
                    return;
                }
 

                List<string> scriptNames = new List<string>();
                foreach (var script in activeBehaviours)
                {
                    scriptNames.Add(script.GetType().ToString());
                }
                activeBehaviourStrings = scriptNames.ToArray();
#endif
        }




        public void RemoveActiveBehaviours()
        {
            FindActiveBehaviours();
            foreach (var script in activeBehaviours)
            {
                RemoveBehaviour(script.name);
            }

        }

        /// <summary>
        /// Get array of names of available behaviour scripts.
        /// </summary>
        /// <returns>Array of name string of available scripts for this model type.</returns>
        public string[] GetAvailableBehaviourStrings()
        {
            return availableBehaviourStrings;
        }

        /// <summary>
        /// Get array of names of active behaviour scripts on this object/
        /// </summary>
        /// <returns>Array of name strings of active scripts for this model.</returns>
        public string[] GetActiveBehaviourStrings()
        {
            return activeBehaviourStrings;
        }

        public void RefreshBehaviours()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
               {
                   FindActiveBehaviours();
                   FindAvailableBehaviours();
               };
#else
                   FindActiveBehaviours();
                   FindAvailableBehaviours();
#endif
        }

        public void EnableGravity()
        {
            if (awThing != null)
            {
                if (awThing.GetComponent<Rigidbody>() != null)
                {
                    if (!awThing.GetComponent<Rigidbody>().useGravity)
                    {
                        //Add main collider script 
                        if (awThing.GetComponent<AddCollider>() == null)
                        {
                            awThing.AddComponent(typeof(AddCollider));
                        }
                        //Enable gravity
                        awThing.GetComponent<Rigidbody>().useGravity = true;
                    }
                }
            }
        }

        public void DisableGravity()
        {
            if (transform.childCount > 0 && awThing != null)
            {
                if (awThing.GetComponent<Rigidbody>() != null)
                {
                    if (awThing.GetComponent<Rigidbody>().useGravity)
                    {
                        // Disable gravity
                        awThing.GetComponent<Rigidbody>().useGravity = false;

                        //Remove main collider script
                        if (awThing.GetComponent<AddCollider>())
                        {
                            AnythingSafeDestroy.SafeDestroyImmediate(awThing.gameObject.GetComponent<AddCollider>());
                        }

                        //Remove colliders
                        var prefabColliders = awThing.gameObject.GetComponents<Collider>();
                        foreach (var collider in prefabColliders)
                        {
                            AnythingSafeDestroy.SafeDestroyImmediate(collider);
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Finds parent transform matching the tag and assigns to awThing variable.
        /// </summary>
        /// <param name="parent">Parent transform to search beneath.</param>
        /// <param name="tag">Tag to find matches to.</param>
        private void GetObjectInContainer(Transform parent, string tag)
        {
            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.CompareTag(tag))
                {
                    awThing = child.gameObject;
                }
                if (child.childCount > 0)
                {
                    GetObjectInContainer(child, tag);
                }
            }
        }

        /// <summary>
        /// Loads all active AWBehaviours in hierarchy.
        /// </summary>
        /// <remarks>
        /// Finds all scripts in the Assets/Resources/Anythingworld/Behaviours/Scripts/*prefabcategory* folder
        /// Loads a string of the names of these scripts to the activeBehaviourStrings array.
        /// </remarks>
        private void FindAvailableBehaviours()
        {
            prefabType = awObj.GetObjCatBehaviour();
            if (prefabType != "")
            {
                //Load directory to find scripts for this type of prefab
                var dir = "Behaviours/Scripts/" + prefabType;
                //Load all scripts in directory into a list
                var names = new List<string>();
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var script in assemb.GetTypes())
                    {
                        if (script.BaseType == typeof(AWBehaviour))
                        {
                            names.Add(script.Name);
                        }
                    }
                }
                availableBehaviourStrings = names.ToArray();
            }
            else
            {
                //Debug.Log("No prefab type found on " + gameObject.name);
            }

        }
        #endregion
    }

}
