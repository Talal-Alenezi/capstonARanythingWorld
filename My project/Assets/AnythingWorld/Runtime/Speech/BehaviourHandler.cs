using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Utilities;

public class BehaviourHandler : MonoBehaviour
{
    private AnythingCreator anythingCreator;
    private Camera mainCamera;
    private CameraFollowUtility mainCameraFollower;
    private void OnEnable()
    {
        anythingCreator = AnythingCreator.Instance;
        mainCamera = Camera.main;
        mainCameraFollower = mainCamera.GetComponent<CameraFollowUtility>();
    }


    public IEnumerator ChaseCoroutine(string chasingAnimalName, string fleeingAnimalName, bool makeNew = false)
    {
        AWObj chasingAWObj = null;
        AWObj fleeingAWObj = null;
        FollowTarget followScript = null;

        if (makeNew)
        {
            Debug.Log("No chasing animal found " + chasingAnimalName + ", trying to make.");

            chasingAWObj = anythingCreator.MakeObject(chasingAnimalName);
            if (chasingAWObj != null)
            {
                followScript = chasingAWObj.AddBehaviour<FollowTarget>();
                followScript.SpeedMultiplier = 2f;
            }

            fleeingAWObj = anythingCreator.MakeObject(fleeingAnimalName);
            if (fleeingAWObj != null)
            {
                fleeingAWObj.AddBehaviour<RandomMovement>().SpeedMultiplier = 2f;
                if (followScript != null)
                {
                    followScript.targetController = fleeingAWObj;
                }
                else
                {
                    Debug.Log("No followers found");
                }

            }

            yield return null;
        }
        else
        {
            //Find chaser
            var foundItems = GameObject.FindObjectsOfType<AWObj>();
            yield return null;
            var foundList = new List<AWObj>(foundItems);
            foreach (var creature in foundList)
            {

                if (chasingAWObj != null && fleeingAWObj != null) break;

                Debug.Log(creature.ModelName);
                if (creature.ModelName == chasingAnimalName)
                {
                    chasingAWObj = creature;
                }
                yield return null;
                if (creature.ModelName == fleeingAnimalName && creature != chasingAWObj)
                {
                    fleeingAWObj = creature;
                }
                yield return null;
            }


            if (fleeingAWObj == null)
            {
                fleeingAWObj = anythingCreator.MakeObject(fleeingAnimalName);
            }
            if (chasingAWObj == null)
            {
                chasingAWObj = anythingCreator.MakeObject(chasingAnimalName);
            }


            fleeingAWObj.AddBehaviour<RandomMovement>().SpeedMultiplier = 2f;


            chasingAWObj.AddBehaviour<FollowTarget>().targetController = fleeingAWObj;
        }
    }

    public void StopRiding()
    {
        mainCameraFollower.SwitchModes(CameraFollowUtility.CameraMode.Follow);
    }
    public IEnumerator RideCoroutine(string animalToRideName, bool isMakeNew = false)
    {
        AWObj animalAWObj = null;

        if (!isMakeNew)
        {
            var foundItems = GameObject.FindObjectsOfType<AWObj>();
            var foundList = new List<AWObj>(foundItems);
            yield return null;


            foreach (var creature in foundList)
            {
                if (animalAWObj != null) break;
                Debug.Log(creature.ModelName);

                if (creature.ModelName == animalToRideName)
                {
                    animalAWObj = creature;
                    break;
                }
                yield return null;
            }

            if (animalAWObj == null)
            {
                animalAWObj = anythingCreator.MakeObject(animalToRideName);
                if (animalAWObj != null)
                {
                    animalAWObj.AddBehaviour<RandomMovement>();
                }
            }



        }
        else
        {
            animalAWObj = anythingCreator.MakeObject(animalToRideName);
            if (animalAWObj != null)
            {
                mainCamera.GetComponent<CameraFollowUtility>().RideTarget(animalAWObj);
            }
        }


        mainCamera.GetComponent<CameraFollowUtility>().RideTarget(animalAWObj);
        yield return null;
    }

    public IEnumerator ThrowCoroutine(string nameOfThingToThrow, string nameOfThingThrownAt, string quantity, bool isMakingNew = false)
    {
        AWObj throwAWObj = null;
        AWObj throwAtAWObj = null;
        ThrowTarget throwScript = null;

        var throwStartPos = Camera.main.transform.position - new Vector3(0, 10f, 0f);

        if (isMakingNew)
        {

            throwAWObj = anythingCreator.MakeObject(nameOfThingToThrow, throwStartPos, false);
            if (throwAWObj != null)
            {
                throwScript = throwAWObj.gameObject.AddComponent<ThrowTarget>();
            }

            throwAtAWObj = anythingCreator.MakeObject(nameOfThingThrownAt);
            if (throwAtAWObj != null)
            {
                if (throwScript != null)
                {
                    throwScript.TargetTransform = throwAtAWObj.transform;
                }
                else
                {
                    Debug.Log("Nothing to throw at found");
                }
            }

            yield return null;
        }
        else
        {
            //Find thrower
            var foundItems = GameObject.FindObjectsOfType<AWObj>();
            yield return null;
            var foundList = new List<AWObj>(foundItems);
            foreach (var creature in foundList)
            {

                if (throwAWObj != null && throwAtAWObj != null) break;


                if (creature.ModelName == nameOfThingToThrow)
                {
                    Debug.Log("found object to throw -> " + creature.ModelName);
                    throwAWObj = creature;
                }
                yield return null;
                if (creature.ModelName == nameOfThingThrownAt && creature != throwAWObj)
                {
                    Debug.Log("found object to be thrown at -> " + creature.ModelName);
                    throwAtAWObj = creature;
                }
                yield return null;
            }


            if (throwAWObj == null)
            {

                throwAWObj = anythingCreator.MakeObject(nameOfThingToThrow, throwStartPos, false);
                while (!throwAWObj.ObjMade)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            if (throwAtAWObj == null && !String.IsNullOrEmpty(nameOfThingThrownAt))
            {
                throwAtAWObj = anythingCreator.MakeObject(nameOfThingThrownAt);
                while (!throwAtAWObj.ObjMade)
                {
                    yield return new WaitForEndOfFrame();
                }

            }

            if (!String.IsNullOrEmpty(nameOfThingThrownAt))
            {
                var throwTarget = throwAWObj.gameObject.AddComponent<ThrowTarget>();
                throwTarget.TargetTransform = throwAtAWObj.transform;
            }



        }
    }

    public IEnumerator CarryCoroutine(string thingCarryingName, string thingCarryName, bool isMakeNew = false)
    {
        AWObj carryingAWObj = null;
        AWObj carriedAWObj = null;
        FollowTarget followScript = null;

        if (isMakeNew)
        {

            carryingAWObj = anythingCreator.MakeObject(thingCarryingName);
            if (carryingAWObj != null)
            {
                followScript = carryingAWObj.AddBehaviour<FollowTarget>();
            }

            carriedAWObj = anythingCreator.MakeObject(thingCarryName);
            if (carriedAWObj != null)
            {
                carriedAWObj.AddBehaviour<RandomMovement>().SpeedMultiplier = 2f; ;
                if (followScript != null)
                {
                    followScript.targetController = carriedAWObj;
                }
                else
                {
                    Debug.Log("No followers found");
                }

            }

            yield return null;
        }
        else
        {
            var foundItems = GameObject.FindObjectsOfType<AWObj>();
            yield return null;
            var foundList = new List<AWObj>(foundItems);
            foreach (var creature in foundList)
            {

                if (carryingAWObj != null && carriedAWObj != null) break;

                if (creature.ModelName == thingCarryingName)
                {
                    carryingAWObj = creature;
                }
                yield return null;
                if (creature.ModelName == thingCarryName && creature != carriedAWObj)
                {
                    carriedAWObj = creature;
                }
                yield return null;
            }


            if (carriedAWObj == null)
            {
                carriedAWObj = anythingCreator.MakeObject(thingCarryName);
            }
            if (carryingAWObj == null)
            {
                carryingAWObj = anythingCreator.MakeObject(thingCarryingName);
            }

            followScript = carryingAWObj.AddBehaviour<FollowTarget>();
            followScript.targetController = carriedAWObj;
        }



        followScript.TargetRadius = 10f;

        while (!followScript.ReachedTarget)
        {
            followScript.moveSpeed *= 1.1f;
            yield return new WaitForEndOfFrame();
        }

        carriedAWObj.RemoveExistingBehaviours();
        var carryTarget = carriedAWObj.gameObject.AddComponent<CarryTarget>();
        carryTarget.targetController = carryingAWObj;
        carryingAWObj.AddBehaviour<RandomMovement>();

    }
}