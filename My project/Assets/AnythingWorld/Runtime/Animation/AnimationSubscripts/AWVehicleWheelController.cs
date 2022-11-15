using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld.Utilities;

namespace AnythingWorld.Animation
{
    /// <summary>
    /// Animation subscript for controlling the wheels on a vehicle.
    /// </summary>
    public class AWVehicleWheelController : AnythingAnimationComponent
    {
        #region Fields
        private List<Transform> wheels;
        private List<Transform> wheelsVisual;
        private List<WheelCollider> wheelColliders;
        private List<WheelCollider> steerWheelColliders;
        private List<WheelCollider> driveWheelColliders;
        public float EnginePower { get; set; }
        public float RotationSpeed { get; set; }

        public float Power { get; set; }
        public float Brake { get; set; }
        public float Steer { get; set; }
        public float MaxSteer { get; set; }
        public Vector3 VehicleCenterOfMass { get; set; }

        private Rigidbody rBody;
        private Vector3 target;
        private AWObj controllingAWObj;
        private bool readyToGo = false;

        /// <summary>
        /// Original position.
        /// </summary>
        Vector3 originalPosition;
        /// <summary>
        /// Original rotation.
        /// </summary>
        Quaternion originalRotation;
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            readyToGo = false;
        }

        void Start()
        {
            //Set original position and rotation
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;

            rBody = GetComponent<Rigidbody>();
            rBody.centerOfMass = VehicleCenterOfMass;

            UpdateTarget();

            // TODO: careful! reliant on parent object script
            controllingAWObj = transform.parent.GetComponent<AWObj>();
            if (controllingAWObj == null)
            {
                Debug.LogError($"No AWObj found for {gameObject.name}");
                return;
            }

            StartCoroutine(WaitForAWObjCompletion());
        }
        void Update()
        {
            if (!readyToGo)
                return;

            var relativePos = transform.position - target;
            var targetRotation = Quaternion.LookRotation(relativePos);

            var transY = transform.eulerAngles.y;
            var DeltaAngle = Mathf.DeltaAngle(transY, targetRotation.eulerAngles.y) * -1;

            var accel = 0.6f;


            var toTarget = relativePos.normalized;

            // reverse if target behind     
            if (Vector3.Dot(toTarget, transform.forward) < 0)
            {
                accel *= -1;
            }

            Power = (accel * EnginePower * Time.deltaTime * 250.0f) * -1;

            var targSteer = Mathf.Clamp(DeltaAngle, -MaxSteer, MaxSteer);

            if (Steer != targSteer)
            {
                Steer = Mathf.Lerp(Steer, targSteer, 0.01f);
            }

            if (Brake > 0f)
            {
                ChangeWheels(Steer, Brake, 0f);
            }
            else if (accel == 0f)
            {
                ChangeWheels(Steer, 1f, 0f);
            }
            else
            {
                ChangeWheels(Steer, 0f, Power);
            }

            var targSqrMag = Vector3.SqrMagnitude(transform.position - target);

            if (targSqrMag < 100f)
            {
                UpdateTarget();
            }
        }
        #endregion

        #region Public Methods
        public void ApplyLocalPositionToVisuals(WheelCollider collider, int wheelIndex)
        {
            var visualWheel = wheelsVisual[wheelIndex];
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }
        /// <summary>
        /// Method allowing for the modification of script parameters.
        /// </summary>
        /// <param name="parameter"></param>
        public override void ModifyParameter(Parameter parameter)
        {
            if (parameter.ParameterScriptName == "EnginePower")
                EnginePower = parameter.Value;
            if (parameter.ParameterScriptName == "RotationSpeed")
                RotationSpeed = parameter.Value;
            if (parameter.ParameterScriptName == "Power")
                Power = parameter.Value;
            if (parameter.ParameterScriptName == "Brake")
                Brake = parameter.Value;
            if (parameter.ParameterScriptName == "Steer")
                Steer = parameter.Value;
            if (parameter.ParameterScriptName == "MaxSteer")
                MaxSteer = parameter.Value;
            if (parameter.ParameterScriptName == "VehicleCenterOfMass")
                VehicleCenterOfMass = parameter.Vector3Value;
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
        private IEnumerator WaitForAWObjCompletion()
        {
            while (!controllingAWObj.ObjMade)
                yield return new WaitForEndOfFrame();

            StartCoroutine(CollectWheels());
        }

        private IEnumerator CollectWheels()
        {

            wheels = new List<Transform>();
            wheelsVisual = new List<Transform>();

            wheelColliders = new List<WheelCollider>();
            steerWheelColliders = new List<WheelCollider>();
            driveWheelColliders = new List<WheelCollider>();

            foreach (var t in transform.GetComponentsInChildren<Transform>())
            {
                var tName = t.name.ToLower();
                if (t != transform)
                {
                    if (tName.IndexOf("wheel_collider") != -1)
                    {
                        var wCollider = t.GetComponent<WheelCollider>();
                        if (t.GetComponent<WheelCollider>() == null)
                        {
                            wCollider = t.gameObject.AddComponent<WheelCollider>();

                        }
                        wheels.Add(t);
                        wheelColliders.Add(wCollider);
                    }
                    else if (tName.IndexOf("wheel") != -1)
                    {

                        if (t.childCount > 0)
                        {

                            var visualWheel = t.GetChild(0);

                            wheelsVisual.Add(visualWheel);
                        }
                    }

                    if (tName.IndexOf("steer") != -1)
                    {
                        steerWheelColliders.Add(t.GetComponent<WheelCollider>());
                    }
                    if (tName.IndexOf("drive") != -1)
                    {
                        driveWheelColliders.Add(t.GetComponent<WheelCollider>());
                    }
                }
            }


            for (var i = 0; i < wheels.Count; i++)
            {
                wheels[i].position = wheelsVisual[i].parent.GetComponent<CenterMeshPivot>().PivotCenter;
                wheels[i].GetComponent<WheelCollider>().radius = wheelsVisual[i].parent.GetComponent<CenterMeshPivot>().Radius;
            }

            yield return new WaitForEndOfFrame();
            readyToGo = true;

        }

        private void LateUpdate()
        {
            if (!readyToGo)
                return;

            for (var i = 0; i < wheels.Count; i++)
            {
                var wheel = wheels[i];
                ApplyLocalPositionToVisuals(wheel.GetComponent<WheelCollider>(), i);
            }
        }
        private void ChangeWheels(float steerAmount, float brakeAmount, float motorPower)
        {
            // steer
            foreach (var sCollider in steerWheelColliders)
            {
                sCollider.steerAngle = steerAmount;
            }
            // brake
            foreach (var wCollider in wheelColliders)
            {
                wCollider.brakeTorque = brakeAmount;
            }
            // motor torque
            foreach (var dCollider in driveWheelColliders)
            {
                dCollider.motorTorque = motorPower;
            }
        }
        private void UpdateTarget()
        {
            target = new Vector3(Random.Range(-50f, 50f), transform.position.y, Random.Range(-50f, 50f));
        }
        #endregion
















    }
}
