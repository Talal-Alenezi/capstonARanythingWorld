using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;

namespace AnythingWorld.Examples
{
    /// <summary>
    /// Spawn object at transform location and parented to transform.
    /// </summary>
    public class MakeOnTransformParented : MonoBehaviour
    {
        public string objectToMake = "dog";
        public Transform parentTransform = null;
        public Transform spawnTransform = null;

        void Start()
        {
            //Spawns object parented to parentTransform at the spawnTransform
            AnythingCreator.Instance.MakeObject(objectToMake, spawnTransform, parentTransform);
        }
    }
}

