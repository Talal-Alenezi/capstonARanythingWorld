using AnythingWorld.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Base animation controller class from which other animation controllers inherit.
    /// </summary>
    [Serializable]
    public abstract class AnythingAnimationController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        protected AWObj controllingAWObj;
        // List of all the parameters used in this animation
        [SerializeField]
        protected List<Parameter> parametersList;
        [SerializeField]
        protected List<PrefabPartToAnimationSubscript> prefabNameToAnimationScriptMapping;
        [SerializeField]
        protected Dictionary<(string, string), AnythingAnimationComponent> partNameToScript;
        // Variable used to signal finishing of the scripts setup
        protected bool scriptSetupComplete = false;

        /// ParameterController object used for monitoring parameters' changes
        protected ParameterController paramControl;

        #endregion
        public void Start()
        {
            FindObjectController();
            InitializeAnimationParameters();
            StartCoroutine(WaitForAWObjCompletion());
        }

        public void Update()
        {
            if (paramControl == null) InitializeAnimationParameters();

            if (scriptSetupComplete)
            {
                if (paramControl == null) InitializeAnimationParameters();
                UpdateParameters();
            }
            else if ((controllingAWObj.ObjMade))
            {
                AddAnimationScripts();
                ActivateShader();
            }
        }

        public void FindObjectController()
        {
            if (!transform.parent.TryGetComponent<AWObj>(out controllingAWObj))
            {
                Debug.Log("No controlling AwObj found, removing animation script.", this);
                Destroy(this);
            }
        }
        public abstract void InitializeAnimationParameters();
        public void RemoveAnimationScripts()
        {
            //Debug.Log("removing animation scripts from " + transform.name);
            // Iterate over prefab's parts
            if (prefabNameToAnimationScriptMapping != null)
            {
                foreach (var prefab in prefabNameToAnimationScriptMapping)
                {
                    // Find gameobject related the the prefab's part
                    var prefabObject = transform;
                    if (!prefab.Key.Equals("parent"))
                    {
                        prefabObject = transform.Find(prefab.Key);
                    }

                    // Iterate over the scripts added to the prefab's part
                    foreach (var script in prefabNameToAnimationScriptMapping.Find(x => x.Key == prefab.Key).Value)
                    {
                        var namespaceScript = "AnythingWorld.Animation." + script;
                        // Add script to the prefab part and store it in the _prefabToScript dictionary
                        if (((AnythingAnimationComponent)prefabObject.gameObject.GetComponent(System.Type.GetType(namespaceScript))))
                        {
                            ((AnythingAnimationComponent)prefabObject.gameObject.GetComponent(System.Type.GetType(namespaceScript))).ResetState();
                            AnythingSafeDestroy.SafeDestroyImmediate(prefabObject.gameObject.GetComponent(System.Type.GetType(namespaceScript)));
                        }
                    }
                }

                if (partNameToScript != null)
                    partNameToScript.Clear();

                scriptSetupComplete = false;

            }

        }
        //        public void RemoveAnimationScriptsEditor()
        //        {
        //            // Iterate over prefab's parts
        //            if (prefabToScriptType != null)
        //            {
        //                foreach (var prefab in prefabToScriptType)
        //                {
        //                    // Find gameobject related the the prefab's part
        //                    var prefabObject = transform;
        //                    if (!prefab.Key.Equals("parent"))
        //                    {
        //                        prefabObject = transform.Find(prefab.Key);
        //                    }

        //                    // Iterate over the scripts added to the prefab's part
        //                    foreach (var script in prefabToScriptType.Find(x => x.Key == prefab.Key).Value)
        //                    {
        //                        // Add script to the prefab part and store it in the _prefabToScript dictionary
        //                        ((AnythingAnimationComponent)prefabObject.gameObject.GetComponent(System.Type.GetType(script))).ResetState();


        //                        // Remove must be called after destroy immediate so no benefit to
        //                        // safedestroying here.
        //                        if (!Application.isPlaying && Application.isEditor)
        //                        {
        //#if UNITY_EDITOR
        //                            UnityEditor.EditorApplication.delayCall += () =>
        //                            {
        //                                DestroyImmediate(prefabObject.gameObject.GetComponent(System.Type.GetType(script)));
        //                                prefabToScript.Remove((prefab.Key, script));
        //                            };
        //#endif

        //                        }
        //                        else
        //                        {
        //                            Destroy(prefabObject.gameObject.GetComponent(System.Type.GetType(script)));
        //                            prefabToScript.Remove((prefab.Key, script));
        //                        }

        //                    }
        //                }

        //                scriptSetupComplete = false;
        //            }
        //        }
        void OnDestroy()
        {
            //Debug.Log("OnDestroy animation " + transform.name);
            RemoveAnimationScripts();
        }
        public void DestroySubscripts()
        {
            RemoveAnimationScripts();
            DeactivateShader();
        }
        /// <summary>
        /// Method used for updating the speed of animation according to the speed of the body movement
        /// </summary>
        /// <param name="speed">New movement speed</param>
        public virtual void SetMovementSpeed(float speed) { }
        /// <summary>
        /// Method used for updating the size of the feet movement in the animation according to the model scale
        /// </summary>
        /// <param name="size">New movement step size</param>
        public virtual void UpdateMovementSizeScale(float scale) { }
        protected abstract void UpdateParameters();
        public void FetchAnimationSettings(string prefab, string behaviour)
        {
            // Obtain settings for this particular behaviour
            var _settings = ScriptableObject.CreateInstance<AnimationSettings>();
            prefabNameToAnimationScriptMapping = _settings.GetScriptsForPrefabAnimation(prefab, behaviour);
            partNameToScript = new Dictionary<(string, string), AnythingAnimationComponent>();
        }
        protected void AddAnimationScripts()
        {

            if (prefabNameToAnimationScriptMapping == null) return;
            //Debug.Log("adding animation scripts for "+transform.name);
            //Debug.Log("Adding Animation Scripts");
            // Iterate over prefab's parts
            foreach (var prefab in prefabNameToAnimationScriptMapping)
            {
                // Find gameobject related the the prefab's part
                var prefabObject = transform;
                if (!prefab.Key.Equals("parent"))
                {
                    prefabObject = transform.Find(prefab.Key);
                }

                if (prefabObject == null)
                {
                    Debug.Log($"Could not find prefab object for {prefab.Key}");
                    continue;
                }

                // Iterate over the scripts added to the prefab's part
                foreach (var script in prefabNameToAnimationScriptMapping.Find(x => x.Key == prefab.Key).Value)
                {
                    var animationScriptName = "AnythingWorld.Animation." + script;
                    //Debug.Log($"Adding {animationScriptName} to {prefabObject.name} for object {transform.name}");

                    try
                    {
                        if (prefabObject.gameObject.GetComponent(System.Type.GetType(animationScriptName)))
                        {
                            var existingComponent = prefabObject.gameObject.GetComponent(System.Type.GetType(animationScriptName)) as AnythingAnimationComponent;
                            partNameToScript.Add((prefab.Key, script), existingComponent);
                        }
                        else
                        {
                            var component = (AnythingAnimationComponent)prefabObject.gameObject.AddComponent(System.Type.GetType(animationScriptName));
                            partNameToScript.Add((prefab.Key, script), component);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log($"When when trying to add script {animationScriptName} to {prefabObject.gameObject.name}");
                        Debug.LogException(e);
                    }
                    // Add script to the prefab part and store it in the _prefabToScript dictionary
                }
            }



            ///Update all parameters for the behaviour in order to give them initial value
            foreach (var param in parametersList)
            {
                try
                {
                    partNameToScript[(param.PrefabPart, param.ScriptName)].ModifyParameter(param);
                }
                catch
                {
                    Debug.Log($"Error getting prefabToScript value for {param.PrefabPart},{param.ScriptName}.");
                }

            }
            UpdateMovementSizeScale(controllingAWObj.ObjectScale);
            scriptSetupComplete = true;
        }
        protected virtual void DeactivateShader() { }
        protected virtual void ActivateShader() { }
        protected IEnumerator WaitForAWObjCompletion()
        {
            while (!controllingAWObj.ObjMade)
                yield return new WaitForEndOfFrame();
            AddAnimationScripts();
            ActivateShader();
        }

    }
}
