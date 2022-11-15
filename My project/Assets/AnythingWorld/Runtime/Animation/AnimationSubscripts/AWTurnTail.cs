using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript for animating the tail.
    /// </summary>
    public class AWTurnTail : AnythingAnimationComponent
    {
        #region Fields
        /// <summary>
        /// Speed of tail rotation.
        /// </summary>
        public float TurnSpeed { get; set; }

        /// <summary>
        /// Maximum frequency of shakes.
        /// </summary>
        public float N { get; set; }

        /// <summary>
        /// Direction of the rotation that simulates the shake of the plane
        /// </summary>
        float shakeDirection = 1.0f;

        /// <summary>
        /// Helper variable used for the shaking movement
        /// </summary>
        float timeCount;

        /// <summary>
        /// Original position of tail transform.
        /// </summary>
        Vector3 _originalPosition;

        /// <summary>
        /// Original rotation of tail transform.
        /// </summary>
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
            // Define the shake rotation of the plane
            timeCount--;

            // Change the direction of the shake
            if (shakeDirection == 1.0f && timeCount == 0)
                shakeDirection = -1.0f;
            else if (shakeDirection == -1.0f && timeCount == 0)
                shakeDirection = 1.0f;

            // Define the new shake frequency
            if (timeCount == 0)
                timeCount = N;


            transform.position = Vector3.Lerp(transform.position, transform.position + shakeDirection * transform.right, TurnSpeed * Time.deltaTime);

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public override void ModifyParameter(Parameter parameter)
        {
            if (parameter.ParameterScriptName == "TurnSpeed")
                TurnSpeed = parameter.Value;
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

