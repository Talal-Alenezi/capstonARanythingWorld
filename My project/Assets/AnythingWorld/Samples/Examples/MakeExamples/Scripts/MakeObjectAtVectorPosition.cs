using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;

namespace AnythingWorld.Examples
{
    public class MakeObjectAtVectorPosition : MonoBehaviour
    {
        public string objectToMake = "cow";
        public Vector3 spawnPosition = new Vector3(2, 1, 2);

        private void Start()
        {
            AnythingCreator.Instance.MakeObject(objectToMake, spawnPosition);
        }
        void OnDrawGizmosSelected()
        {
            // Draw a semitransparent blue cube at the transforms position
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(spawnPosition, 1);
        }
    }
}

