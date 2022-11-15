using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnythingWorld.Utilities
{
    public class VehicleWheelController : MonoBehaviour
    {
        private List<Transform> wheels;
        private List<Transform> wheelsVisual;
        private List<WheelCollider> _wheelColliders;
        private List<WheelCollider> _steerWheelColliders;
        private List<WheelCollider> _driveWheelColliders;
        public float enginePower = 150.0f;
        public float rotationSpeed = 5f;

        public float power = 0.0f;
        public float brake = 0.0f;
        public float steer = 0.0f;
        public float maxSteer = 25.0f;
        public Vector3 VehicleCenterOfMass = new Vector3(0f, -0.5f, 0f);
        public AWObj controllingAWObj;
        private Rigidbody _rBody;
        private Vector3 _target;

        private bool _readyToGo = false;

        void Start()
        {
         
            controllingAWObj = transform.parent.GetComponent<AWObj>();
            if (controllingAWObj == null)
            {
                Debug.LogError($"No AWObj found for {gameObject.name}");
                return;
            }
            else
            {
                StartCoroutine(WaitForAWObjCompletion());
            }


        }
        public void Initialize()
        {
            _rBody = GetComponent<Rigidbody>();
            _rBody.centerOfMass = VehicleCenterOfMass;
            UpdateTarget();
            // TODO: careful! reliant on parent object script
        }

        private IEnumerator WaitForAWObjCompletion()
        {
            while (!controllingAWObj.ObjMade)
                yield return new WaitForEndOfFrame();
            Initialize();
            StartCoroutine(CollectWheels());
        }

        private IEnumerator CollectWheels()
        {
            wheels = new List<Transform>();
            wheelsVisual = new List<Transform>();
            _wheelColliders = new List<WheelCollider>();
            _steerWheelColliders = new List<WheelCollider>();
            _driveWheelColliders = new List<WheelCollider>();

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
                        _wheelColliders.Add(wCollider);
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
                        _steerWheelColliders.Add(t.GetComponent<WheelCollider>());
                    }
                    if (tName.IndexOf("drive") != -1)
                    {
                        _driveWheelColliders.Add(t.GetComponent<WheelCollider>());
                    }
                }
            }


            for (var i = 0; i < wheels.Count; i++)
            {
                wheels[i].position = wheelsVisual[i].parent.GetComponent<CenterMeshPivot>().PivotCenter;
                wheels[i].GetComponent<WheelCollider>().radius = wheelsVisual[i].parent.GetComponent<CenterMeshPivot>().Radius;
            }

            foreach(var wheel in _wheelColliders)
            {
                wheel.enabled = true;
            }
            yield return new WaitForEndOfFrame();
            _readyToGo = true;

        }

        private void LateUpdate()
        {
            if (!_readyToGo) return;
            for (var i = 0; i < wheels.Count; i++)
            {
                var wheel = wheels[i];
                ApplyLocalPositionToVisuals(wheel.GetComponent<WheelCollider>(), i);
            }
        }

        void Update()
        {
            if (!_readyToGo)return;

            var relativePos = transform.position - _target;
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

            power = (accel * enginePower * Time.deltaTime * 250.0f) * -1;

            var targSteer = Mathf.Clamp(DeltaAngle, -maxSteer, maxSteer);

            if (steer != targSteer)
            {
                steer = Mathf.Lerp(steer, targSteer, 0.01f);
            }

            if (brake > 0f)
            {
                ChangeWheels(steer, brake, 0f);
            }
            else if (accel == 0f)
            {
                ChangeWheels(steer, 1f, 0f);
            }
            else
            {
                ChangeWheels(steer, 0f, power);
            }

            var targSqrMag = Vector3.SqrMagnitude(transform.position - _target);

            if (targSqrMag < 100f)
            {
                UpdateTarget();
            }
        }

        private void ChangeWheels(float steerAmount, float brakeAmount, float motorPower)
        {
            // steer
            foreach (var sCollider in _steerWheelColliders)
            {
                sCollider.steerAngle = steerAmount;
            }
            // brake
            foreach (var wCollider in _wheelColliders)
            {
                wCollider.brakeTorque = brakeAmount;
            }
            // motor torque
            foreach (var dCollider in _driveWheelColliders)
            {
                dCollider.motorTorque = motorPower;
            }
        }

        public void ApplyLocalPositionToVisuals(WheelCollider collider, int wheelIndex)
        {

            var visualWheel = wheelsVisual[wheelIndex];

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        private void UpdateTarget()
        {
            _target = new Vector3(Random.Range(-50f, 50f), transform.position.y, Random.Range(-50f, 50f));
        }

    }
}

