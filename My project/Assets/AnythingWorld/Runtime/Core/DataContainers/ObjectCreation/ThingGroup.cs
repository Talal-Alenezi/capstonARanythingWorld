using AnythingWorld;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif
using UnityEngine;
using AnythingWorld.Utilities;

namespace AnythingWorld.DataContainers
{
    [Serializable]
    public class ThingGroup
    {
        [SerializeField]
        public string objectName;
        [SerializeField]
        public List<AWObj> awInstances;
        [SerializeField]
        public List<GameObject> objInstances;

        [SerializeField]
        private int goalInstances = -1;

#if UNITY_EDITOR
        private EditorCoroutine sliderWaitEditorRoutine;
#else
    private Coroutine sliderWaitRoutine;
#endif

        public int GoalInstances
        {
            get
            {
                if (goalInstances < 0)
                {
                    return objInstances.Count;
                }
                else
                {
                    return goalInstances;
                }
            }
            set
            {
                if (value != goalInstances)
                {
                    if (value >= 0)
                    {
                        goalInstances = value;
#if UNITY_EDITOR
                        if (sliderWaitEditorRoutine != null)
                            EditorCoroutineUtility.StopCoroutine(sliderWaitEditorRoutine);
                        sliderWaitEditorRoutine = EditorCoroutineUtility.StartCoroutineOwnerless(SliderAdjust(goalInstances));
#else
                    if (sliderWaitRoutine != null)
                        AnythingCreator.Instance.StopCoroutine(sliderWaitRoutine);
                    sliderWaitRoutine = AnythingCreator.Instance.StartCoroutine(SliderAdjust(goalInstances));
#endif
                    }
                    else
                    {
                        goalInstances = 0;
                    }
                }


            }
        }
        [SerializeField]
        private int heldGoalInstances = 1;
        public int HeldGoalInstances
        {
            get
            {
                return heldGoalInstances;
            }
            set
            {
                heldGoalInstances = value;
            }
        }


        public ThingGroup()
        {
            awInstances = new List<AWObj>();
            objInstances = new List<GameObject>();
        }
        public ThingGroup(string groupName)
        {
            objectName = groupName;
            awInstances = new List<AWObj>();
            objInstances = new List<GameObject>();

        }

        public void ClearObjects()
        {
            foreach (var creature in objInstances)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(creature);
            }
        }

        private IEnumerator SliderAdjust(int newCount)
        {
            while (AnythingCreator.Instance.MakeObjectRoutineRunning)
            {
#if UNITY_EDITOR
                yield return new EditorWaitForSeconds(0.05f);
#else
            yield return new WaitForEndOfFrame();
#endif
            }

            Adjust(newCount);
        }

        public void Adjust(int newCount)
        {
            if (AnythingSettings.Instance.showDebugMessages) Debug.Log("Adjust " + objectName + " to : " + newCount);

            var difference = newCount - objInstances.Count;
            if (difference == 0)
            {
                return;
            }
            else if (difference < 0)
            {
                var positiveDiff = Mathf.Abs(difference);

                for (var i = 0; i < positiveDiff; i++)
                {
                    var ind = objInstances.Count - 1;
                    AnythingSafeDestroy.SafeDestroyDelayed(objInstances[ind]);
                    objInstances.RemoveAt(ind);
                }
            }
            else
            {
                for (var i = 0; i < difference; i++)
                {
                    AnythingCreator.Instance.MakeObject(objectName);
                    AnythingCreator.Instance.ResetAutoLayout();
                }
            }
        }

    }
}
