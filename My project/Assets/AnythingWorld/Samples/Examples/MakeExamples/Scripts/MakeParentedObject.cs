using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;

namespace AnythingWorld.Examples
{
    /// <summary>
    /// Create an object that is parented to another.
    /// </summary>
    public class MakeParentedObject : MonoBehaviour
    {
        public string objectToMake = "cat";
        public Transform parentTransform = null;
        private void Start()
        {
            AnythingCreator.Instance.MakeObject(objectToMake, parentTransform);
        }
    }
}