using UnityEngine;

namespace AnythingWorld.Examples
{
    public class MakeObjectQuaternion : MonoBehaviour
    {
        public string objectToMake = "dog";
        public Vector3 vectorPosition = new Vector3(2, 1, 2);
        public Quaternion quaternionRotation = Quaternion.identity;
        private void Start()
        {
            AnythingCreator.Instance.MakeObject(objectToMake, vectorPosition, quaternionRotation);
        }
    }
}

