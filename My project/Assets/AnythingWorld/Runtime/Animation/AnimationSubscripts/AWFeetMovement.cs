using System.Collections;
using UnityEngine;
using AnythingWorld.Utilities;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript handling movement of feet.
    /// </summary>
    public class AWFeetMovement : AnythingAnimationComponent
    {
        #region Fields
        /// <summary>
        /// Speed of footstep.
        /// </summary>
        public float StepSpeed { get; set; }

        /// <summary>
        /// Starting degree of footstep.
        /// </summary>
        public float StepDegree { get; set; }

        /// <summary>
        /// Size of footstep.
        /// </summary>
        public float StepRadius;


        /// <summary>
        /// Stores initial offset of foot at start of movement.
        /// </summary>
        [SerializeField]
        Vector3 offset;

        /// <summary>
        /// Original position of transform.
        /// </summary>
        Vector3 originalPosition;

        /// <summary>
        /// Original rotation of transform.
        /// </summary>
        Quaternion originalRotation;

        AddMeshColliders feetCreation;

        [SerializeField]
        bool offsetSet = false;
        #endregion

        #region Unity Callbacks
        public void Start()
        {
            //Debug.LogWarning("Foot script for " + transform.parent.parent.name + "starting");
            // TODO: careful! reliant on parent object script
            feetCreation = transform.parent.GetComponent<AddMeshColliders>();
            //feetCreation.transform.localPosition = Vector3.zero;
            if (feetCreation == null)
            {
                if(AnythingSettings.DebugEnabled)Debug.LogWarning($"Colliders for {gameObject.name} are not added automatically");
                SetOffsets();
                return;
            }

            StartCoroutine(WaitForCollidersSetupCompletion());
        }
        public void Update()
        {
            if (!offsetSet)
            {
                SetOffsets();
            }
            // Modify the angle
            StepDegree += StepSpeed * 200f * Time.deltaTime;
            StepDegree = StepDegree % 360;

            // Calculate the new position of the feet
            var radians = StepDegree * Mathf.PI / 180f;
            var goalX = StepRadius * Mathf.Cos(radians);
            var goalY = 0;
            var goalZ = StepRadius * Mathf.Sin(radians);
            if ((goalX > 0 && goalZ > 0) || (goalX < 0 && goalZ > 0))
            {
                goalZ = 0;
            }

            // Set new position of the feet
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(offset.x - goalY, offset.y - goalZ, offset.z + goalX), 0.4f);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public override void ModifyParameter(Parameter parameter)
        {
            if (parameter.ParameterScriptName == "StepSpeed")
                StepSpeed = parameter.Value;
            if (parameter.ParameterScriptName == "StepDegree")
                StepDegree = parameter.Value;
            if (parameter.ParameterScriptName == "StepRadius")
                StepRadius = parameter.Value;

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

        #region Private Methods
        private IEnumerator WaitForCollidersSetupCompletion()
        {
            while (!feetCreation.feetSetupComplete)
                yield return new WaitForEndOfFrame();
            SetOffsets();
        }
        private void SetOffsets()
        {
            if (!offsetSet)
            {
                transform.localRotation = Quaternion.identity;


                //Debug.Log("setting foot offset");
                //Set original position and rotation
                originalPosition = transform.localPosition;


                originalRotation = transform.localRotation;
                //Set the offests for the model
                offset = transform.localPosition;
                offsetSet = true;
            }

        }
        #endregion
    }
}

