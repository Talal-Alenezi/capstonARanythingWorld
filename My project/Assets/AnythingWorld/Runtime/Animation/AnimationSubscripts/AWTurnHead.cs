using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript handling head turning.
    /// </summary>
    public class AWTurnHead : AnythingAnimationComponent
    {
        #region Fields
        /// <summary>
        /// Speed of the head rotation.
        /// </summary>
        public float TurnSpeed { get; set; }

        /// <summary>
        /// Maximum angle of rotation.
        /// </summary>
        public float MaxRotation { get; set; }


        /// <summary>
        /// Original position of transform.
        /// </summary>
        Vector3 originalPosition;
        /// <summary>
        /// Original rotation of transform.
        /// </summary>
        Quaternion originalRotation;

        #endregion

        #region Unity Callbacks
        void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }

        void Update()
        {
            transform.localRotation = Quaternion.Euler(MaxRotation * Mathf.Sin(Time.time * TurnSpeed), 0f, 0f);
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
            if (parameter.ParameterScriptName == "MaxRotation")
                MaxRotation = parameter.Value;

        }

        /// <summary>
        /// Method used for reseting position of the body part.
        /// </summary>
        public override void ResetState()
        {
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
        }
        #endregion

    }


}
