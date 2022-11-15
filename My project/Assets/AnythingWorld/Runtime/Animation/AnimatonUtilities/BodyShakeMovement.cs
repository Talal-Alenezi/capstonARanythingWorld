using UnityEngine;

namespace AnythingWorld.Animation
{
    public class BodyShakeMovement : MonoBehaviour
    {
        // The speed of the vehicle movement
        public float moveSpeed = 2f;

        // Maximum frequency of the shakes
        public float shakeFrequency = 0.4f;

        // Direction of the rotation that simulates the shake of the body
        float shakeDirection = 1.0f;

        // Helper variable used for the shaking movement
        float timeCount = 0;


        // Start is called before the first frame update
        void Start()
        {
            timeCount = shakeFrequency;
        }

        // Update is called once per frame
        void Update()
        {
            // Define the shake rotation of the body
            timeCount -= Time.deltaTime;

            // Change the direction of the shake
            if (shakeDirection == 1.0f && timeCount < 0)
                shakeDirection = -1.0f;
            else if (shakeDirection == -1.0f && timeCount < 0)
                shakeDirection = 1.0f;

            // Define the new shake frequency
            if (timeCount < 0)
                timeCount = shakeFrequency;


            transform.position = Vector3.Lerp(transform.position, transform.position + shakeDirection * transform.up, moveSpeed * Time.deltaTime);

        }
    }
}

