using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;

namespace AnythingWorld.Examples
{
    public class MakeObjectFullCustom : MonoBehaviour
    {
        public string objectToMake = "camel";
        public Vector3 objectPos = Vector3.zero;
        public Quaternion objectRotation = Quaternion.identity;
        public Vector3 objectScale = Vector3.one;
        public Transform parentTransform;
        // Start is called before the first frame update
        void Start()
        {
            //Makes object at position pobjectPos, rotation objectRotation, scale objectScale and parent to parentTransform.
            AnythingCreator.Instance.MakeObject(objectToMake, objectPos, objectRotation, objectScale, parentTransform);
        }
    }
}

