using UnityEngine;

namespace AnythingWorld.Utilities
{
    public class RotateThing : MonoBehaviour
    {
        public Vector3 Rotation;
        public float RotateSpeed;
        void Update()
        {
            transform.Rotate(Rotation * RotateSpeed * Time.deltaTime,Space.Self);
        }
    }

}
