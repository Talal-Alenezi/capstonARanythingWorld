using UnityEngine;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript handling model wobbling.
    /// </summary>
    public class AWWobble : AnythingAnimationComponent
    {
        #region Fields
        /// The speed of the wobble
        public float WobbleSpeed { get; set; }

        /// Maximum angle of rotation
        public float MaxRotation { get; set; }


        /// Original position and rotation
        Vector3 _originalPosition;
        Quaternion _originalRotation;
        #endregion

        #region Unity Callbacks
        void Start()
        {
            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;
        }

        void Update()
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, MaxRotation * Mathf.Sin(Time.time * WobbleSpeed));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public override void ModifyParameter(Parameter parameter)
        {
            if (parameter.ParameterScriptName == "WobbleSpeed")
                WobbleSpeed = parameter.Value;
            if (parameter.ParameterScriptName == "MaxRotation")
                MaxRotation = parameter.Value;

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
