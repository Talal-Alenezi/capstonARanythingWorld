using System.Collections;
using UnityEngine;


namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript handling the flapping of wings.
    /// </summary>
    public class AnythingWingRotationAnimationComponent : AnythingAnimationComponent
    {
        #region Fields
        public float RotationSpeed { get; set; }
        public Vector3 RotateTo { get; set; }


        private Vector3 _rotateFrom;
        private bool _shouldFlap;
        private Vector3 _currentAngle;
        //private float _reachedThreshold = 0.1f;
        private bool directionTo;
        private Transform flapTransform;

        /// <summary>
        /// Original position of wing.
        /// </summary>
        Vector3 _originalPosition;
        /// <summary>
        /// Original rotation of wing.
        /// </summary>
        Quaternion _originalRotation;
        #endregion

        #region Unity Callbacks
        public void Start()
        {
            _shouldFlap = false;
            var delayStart = Random.Range(0f, 0.4f);
            StartCoroutine("StartFlap", delayStart);

        }
        public void Update()
        {
            if (_shouldFlap)
            {

                var speedStep = Time.deltaTime * RotationSpeed;

                float angle;
                bool sameRotation;
                if (directionTo)
                {
                    _currentAngle = new Vector3(
                    Mathf.LerpAngle(_currentAngle.x, RotateTo.x, speedStep),
                    Mathf.LerpAngle(_currentAngle.y, RotateTo.y, speedStep),
                    Mathf.LerpAngle(_currentAngle.z, RotateTo.z, speedStep));


                    angle = Quaternion.Angle(Quaternion.Euler(_currentAngle), Quaternion.Euler(RotateTo));
                    sameRotation = Mathf.Abs(angle) < 1f;

                    if (sameRotation)
                    {
                        directionTo = false;
                    }
                }
                else
                {
                    _currentAngle = new Vector3(
                    Mathf.LerpAngle(_currentAngle.x, _rotateFrom.x, speedStep),
                    Mathf.LerpAngle(_currentAngle.y, _rotateFrom.y, speedStep),
                    Mathf.LerpAngle(_currentAngle.z, _rotateFrom.z, speedStep));

                    angle = Quaternion.Angle(Quaternion.Euler(_currentAngle), Quaternion.Euler(_rotateFrom));
                    sameRotation = Mathf.Abs(angle) < 1f;

                    if (sameRotation)
                    {
                        directionTo = true;
                    }
                }

                flapTransform.localEulerAngles = _currentAngle;
            }
            transform.localPosition = Vector3.zero;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public override void ModifyParameter(Parameter parameter)
        {
            if (parameter.ParameterScriptName == "RotationSpeed")
                RotationSpeed = parameter.Value;
            if (parameter.ParameterScriptName == "RotateTo")
                RotateTo = parameter.Vector3Value;

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

        #region Private Methods
        private IEnumerator StartFlap(float delayStart)
        {
            yield return new WaitForSeconds(delayStart);

            flapTransform = transform.Find("WavefrontObject");

            while (flapTransform == null)
            {
                flapTransform = transform.Find("WavefrontObject");
                yield return new WaitForEndOfFrame();
            }

            directionTo = true;
            _rotateFrom = flapTransform.eulerAngles;
            _currentAngle = flapTransform.localEulerAngles;

            _originalPosition = transform.localPosition;
            _originalRotation = transform.localRotation;

            _shouldFlap = true;
        }
        #endregion
    }

}
