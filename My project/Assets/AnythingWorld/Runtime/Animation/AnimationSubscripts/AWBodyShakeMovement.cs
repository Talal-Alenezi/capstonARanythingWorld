using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript handling the body shaking movements.
    /// </summary>
    public class AWBodyShakeMovement : AnythingAnimationComponent
    {
        #region Fields
        /// The speed of the object movement
        public float MoveSpeed { get; set; }

        /// Maximum frequency of the shakes
        public float N { get; set; }

        /// Direction of the rotation that simulates the shake of the body
        float shakeDirection = 1.0f;

        /// Helper variable used for the shaking movement
        float timeCount;

        /// Original position and rotation
        Vector3 _originalPosition;
        Quaternion _originalRotation;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            timeCount = N;
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;
        }
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
                timeCount = N;


            transform.position = Vector3.Lerp(transform.position, transform.position + shakeDirection * (transform.up*transform.lossyScale.y),  MoveSpeed * Time.deltaTime);

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public override void ModifyParameter(Parameter parameter)
        {
            if (parameter.ParameterScriptName == "MoveSpeed")
                MoveSpeed = parameter.Value;
            if (parameter.ParameterScriptName == "Frequency")
                N = parameter.Value;

        }

        /// <summary>
        /// Method used for reseting position of the body part.
        /// </summary>
        public override void ResetState()
        {
            transform.localPosition = _originalPosition;
            transform.localRotation = _originalRotation;
        }
        #endregion

    }

}
