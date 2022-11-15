using AnythingWorld.Animation;
using AnythingWorld.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace AnythingWorld.Behaviours
{
    /// <summary>
    /// Base class from which AWBehaviours are derived.
    /// </summary>
    public abstract class AWBehaviour : MonoBehaviour
    {
        protected abstract string[] targetAnimationType { get; set; }

        public bool behaviourInitialized = false;
        public string parentPrefabType { get; set; }
        protected PrefabAnimationsDictionary settings;

        [SerializeField, HideInInspector]
        protected AnythingAnimationController animator;
        [SerializeField, HideInInspector]
        public bool animatorInitialized = false;
        [SerializeField, HideInInspector]
        public GameObject AWThing
        {
            get
            {
                if (ControllingAWObj == null) return null;
                else
                {
                    return ControllingAWObj.gameObject.FindComponentInChildWithTag<Transform>("AWThing").gameObject;
                }
            }
        }
        public GameObject BehaviourObject
        {
            get
            {
                return AWThing.GetComponentInChildren<AWBehaviour>().gameObject;
            }
        }


        [SerializeField]
        private AWObj controllingAWObj = null;
        public AWObj ControllingAWObj
        {
            get
            {
                if(controllingAWObj==null) controllingAWObj = gameObject.GetComponentInParent<AWObj>();
                return controllingAWObj;
            }
            //protected set { controllingAWObj = value; }
            
        }


        public Transform AWThingTransform
        {
            get
            {
                if (AWThing == null)
                    return null;
                else
                    return AWThing.transform;
            }
        }
        // inherit behaviours from this class to make sure they do not run prematurely when created at runtime!


        public virtual void SetDefaultParametersValues() { }
        public virtual void InitializeAnimator()
        {

            if (!animatorInitialized)
            {
                AddAWAnimator();
                SetDefaultParametersValues();
                animatorInitialized = true;
            }
        }

        public virtual void AddAWAnimator()
        {
            // special cases for flock objects, where AWObj is removed on creation
            if (ControllingAWObj == null)
            {
                if (AnythingSettings.Instance.showDebugMessages) Debug.LogError("No parentAWObj found");
                return;
            }
            parentPrefabType = ControllingAWObj.GetObjCatBehaviour();

            RemoveAWAnimator();

            if (parentPrefabType != null)
            {
                if (targetAnimationType != null)
                {
                    settings = ScriptableObject.CreateInstance<AnythingWorld.Animation.PrefabAnimationsDictionary>();
                    if (settings != null)
                    {
                        System.Type animatorScript = FindMatchingAnimationScript();
                        AddAnimatorToObject(animatorScript);
                    }
                    else
                    {
                        Debug.LogWarning("Animation map settings could not be created.");
                        return;
                    }
                }
                else
                {
                    Debug.Log("No animation type provided");
                    return;
                }
            }
            else
            {
                Debug.Log("No parent prefab part found");
                return;
            }
            return;
        }

        private void AddAnimatorToObject(System.Type animatorScript)
        {
            if (animatorScript != null)
            {
                animator = AWThing.AddComponent(animatorScript) as AnythingAnimationController;

                animator.InitializeAnimationParameters();
                if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Animator " + animator + " added to " + ControllingAWObj.ModelName);
            }
            else
            {

                if (AnythingSettings.DebugEnabled) Debug.LogWarning($"Animator script for target animation type not found");
            }
        }

        private System.Type FindMatchingAnimationScript()
        {
            string animatorScriptstring = null;
            System.Type animatorScript = null;

            foreach (var animationType in targetAnimationType)
            {
                //Debug.Log(parentPrefabType + " " + animationType);
                animatorScriptstring = settings.GetAnimationScriptName(parentPrefabType, animationType);
                if (animatorScriptstring != null)
                {

                    animatorScriptstring = "AnythingWorld.Animation." + animatorScriptstring;
                    //Debug.Log("found" + animatorScriptstring);
                    animatorScript = System.Type.GetType(animatorScriptstring);
                    break;
                }
            }

            return animatorScript;
        }

        public virtual void RemoveAWAnimator()
        {
            if (animator != null)
            {
                //Debug.Log("RemoveAWAnimator for " + transform.name);
                animator.DestroySubscripts();
                AnythingSafeDestroy.SafeDestroyImmediate(animator);
            }
        }
        public void OnDestroy()
        {
            //Debug.Log("on destroy behaviour "+transform.parent.name);
            //RemoveAWAnimator();
        }
    }

}
