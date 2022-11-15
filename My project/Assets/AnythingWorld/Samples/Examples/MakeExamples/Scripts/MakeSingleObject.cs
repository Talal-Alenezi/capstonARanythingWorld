using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;

namespace AnythingWorld.Examples
{
    public class MakeSingleObject : MonoBehaviour
    {
        public string objectToName = "cat";
        void Start()
        {
            AnythingCreator.Instance.MakeObject("cat");
        }
    }
}

