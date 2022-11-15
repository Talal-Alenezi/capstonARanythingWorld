using System.Collections;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class FeetMovement : MonoBehaviour//: AWBehaviour
    {
        // The speed of the animal's feet movement
        public float stepSpeed = 4f;

        // Starting degree of the leg's movement
        public float stepDegree;

        // Size of the leg's step
        public float stepRadius = 1f;


        // The offsets storing initial position of a feet in relation to a model    
        [SerializeField]
        float _xOffset;
        [SerializeField]
        float _yOffset;
        [SerializeField]
        float _zOffset;

        AddMeshColliders _feetCreation;

        [SerializeField]
        bool offsetSet = false;

        // Start is called before the first frame update
        public void Start()
        {

            // TODO: careful! reliant on parent object script
            _feetCreation = transform.parent.GetComponent<AddMeshColliders>();
            if (_feetCreation == null)
            {
                //Debug.LogError($"Colliders for {gameObject.name} are not added automatically");
                SetOffsets();
                return;
            }

            StartCoroutine(WaitForCollidersSetupCompletion());
        }

        private IEnumerator WaitForCollidersSetupCompletion()
        {
            while (!_feetCreation.feetSetupComplete)
                yield return new WaitForEndOfFrame();

            SetOffsets();
        }

        private void SetOffsets()
        {
            if (offsetSet == false)
            {
                //Set the offests for the model
                _xOffset = transform.localPosition.x;
                _yOffset = transform.localPosition.y;
                _zOffset = transform.localPosition.z;
                offsetSet = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!offsetSet)
                return;

            // Modify the angle
            stepDegree += stepSpeed * 200f * Time.deltaTime;
            stepDegree = stepDegree % 360;

            // Calculate the new position of the feet
            var radians = stepDegree * Mathf.PI / 180f;
            var goalX = stepRadius * Mathf.Cos(radians);
            var goalY = 0;
            var goalZ = stepRadius * Mathf.Sin(radians);
            if ((goalX > 0 && goalZ > 0) || (goalX < 0 && goalZ > 0))
            {
                goalZ = 0;
            }
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(_xOffset - goalY, _yOffset - goalZ, _zOffset + goalX), 0.4f);
        }
    }

}
