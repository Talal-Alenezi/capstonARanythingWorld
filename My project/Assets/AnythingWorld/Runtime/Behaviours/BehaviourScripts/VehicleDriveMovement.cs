using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Behaviours;
using AnythingWorld.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class VehicleDriveMovement : AWBehaviour
{
    #region Fields
    private new string parentPrefabType;
    public float enginePower;
    public float rotationSpeed;
    public float power;
    public float brake;
    public float steer;
    public float maxSteer;
    public Vector3 VehicleCenterOfMass;

    [SerializeField]
    public List<Transform> wheelColliderTransforms;
    [SerializeField]
    public List<Transform> wheelMeshes;
    [SerializeField]
    public List<CenterMeshPivot> centerMeshPivots;

    [SerializeField]
    public List<WheelCollider> _wheelColliders;
    [SerializeField]
    private List<WheelCollider> _steerWheelColliders;
    [SerializeField]
    private List<WheelCollider> _driveWheelColliders;

    private Rigidbody _rBody;
    private Vector3 targetPosition;

    protected override string[] targetAnimationType { get; set; } = { "drive", "default" };
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        behaviourInitialized = false;
    }
    public void Start()
    {
        StartCoroutine(WaitForAWObjCompletion());
    }

    public void Initialize()
    {
        if (ControllingAWObj == null)
        {
            Debug.LogError($"No AWObj found for {gameObject.name}");
            return;
        }

        StartCoroutine(CollectWheels());

    }

    public void FixedUpdate()
    {
        if (!animatorInitialized) InitializeAnimator();
        if (!behaviourInitialized) return;



        var relativePos = AWThing.transform.position - targetPosition;
        var targetRotation = Quaternion.LookRotation(relativePos);

        var transY = AWThingTransform.eulerAngles.y;
        var DeltaAngle = Mathf.DeltaAngle(transY, targetRotation.eulerAngles.y) * -1;

        var accel = 0.6f;


        var toTarget = relativePos.normalized;

        // reverse if target behind     
        if (Vector3.Dot(toTarget, AWThingTransform.forward) < 0)
        {
            accel *= -1;
        }

        power = (accel * enginePower * Time.deltaTime * 250.0f) * -1;
        power = Mathf.Clamp(power, -5000, 5000);
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


        for (int i = 0; i < wheelColliderTransforms.Count; i++)
        {
            RotateWheelVisuals(wheelColliderTransforms[i].GetComponent<WheelCollider>(), i);
        }
    }
    #endregion

    #region Private Methods
    private IEnumerator WaitForAWObjCompletion()
    {
        while (!ControllingAWObj.ObjPositioned)
            yield return new WaitForEndOfFrame();
        Initialize();
    }
    private IEnumerator CollectWheels()
    {
        //Debug.Log("collect wheels", this);
        StartCoroutine(RandomiseTarget());
        _rBody = AWThingTransform.GetComponent<Rigidbody>(); // TODO: get this working in editor
        _rBody.centerOfMass = VehicleCenterOfMass;
        wheelColliderTransforms = new List<Transform>();
        wheelMeshes = new List<Transform>();
        _wheelColliders = new List<WheelCollider>();
        _steerWheelColliders = new List<WheelCollider>();
        _driveWheelColliders = new List<WheelCollider>();

        foreach (var t in AWThingTransform.GetComponentsInChildren<Transform>())
        {
            var tName = t.name.ToLower();
            if (t != AWThingTransform)
            {
                if (tName.IndexOf("wheel_collider") != -1)
                {
                    var wCollider = t.GetComponent<WheelCollider>();
                    if (t.GetComponent<WheelCollider>() == null)
                    {
                        wCollider = t.gameObject.AddComponent<WheelCollider>();

                    }
                    wheelColliderTransforms.Add(t);
                    _wheelColliders.Add(wCollider);
                }
                else if (tName.IndexOf("wheel") != -1)
                {

                    if (t.childCount > 0)
                    {
                        //var visualWheel = t.GetChild(0);
                        var visualWheel = t.GetComponent<CenterMeshPivot>().PivotTransform;
                        wheelMeshes.Add(visualWheel);
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

        for (var i = 0; i < wheelColliderTransforms.Count; i++)
        {
            var wheelColliderTransform = wheelColliderTransforms[i];
            if (wheelColliderTransform != null)
            {
                var wheelMesh = wheelMeshes[i];
                if (wheelMesh != null)
                {
                    var meshPivotScript = wheelMesh.GetComponentInParent<CenterMeshPivot>();
                    if (centerMeshPivots == null) centerMeshPivots = new List<CenterMeshPivot>();
                    centerMeshPivots.Add(meshPivotScript);
                    if (meshPivotScript != null)
                    {
                        wheelColliderTransform.position = meshPivotScript.PivotCenter;
                        wheelColliderTransform.GetComponent<WheelCollider>().radius = meshPivotScript.Radius;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find mesh pivot script");
                    }
                }
                else
                {
                    Debug.LogWarning($"Wheel {i} mesh not found");
                }
            }
            else
            {
                Debug.LogWarning($"Could not find wheel transform {i}");
            }
        }
        behaviourInitialized = true;
        yield return new WaitForEndOfFrame();
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
    private IEnumerator RandomiseTarget()
    {
        while (AWThingTransform != null)
        {
            targetPosition = new Vector3(Random.Range(-50f, 50f), AWThingTransform.position.y, Random.Range(-50f, 50f));
            yield return new WaitForSeconds(5);
        }
        yield return null;
    }
    #endregion

    #region Public Methods

    public void RotateWheelVisuals(WheelCollider wheelCollider, int wheelIndex)
    {
        //get loadedobject transform of wheel
        var centerPivotScript = centerMeshPivots[wheelIndex];

        wheelCollider.GetWorldPose(out var position, out var rotation);

        //wheelPivot.position = position;
        centerPivotScript.newPivotObject.transform.rotation = rotation;

    }
    public override void SetDefaultParametersValues()
    {
        var prefabType = gameObject.GetComponentInParent<AWObj>().GetObjCatBehaviour();
        var behaviour = "VehicleDriveMovement";

        var settings = ScriptableObject.CreateInstance<PrefabAnimationsDictionary>();

        enginePower = settings.GetDefaultParameterValue(prefabType, behaviour, "enginePower");
        rotationSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "rotationSpeed");
        power = settings.GetDefaultParameterValue(prefabType, behaviour, "power");
        brake = settings.GetDefaultParameterValue(prefabType, behaviour, "brake");
        steer = settings.GetDefaultParameterValue(prefabType, behaviour, "steer");
        maxSteer = settings.GetDefaultParameterValue(prefabType, behaviour, "maxSteer");
        VehicleCenterOfMass = settings.GetDefaultParameterVector3Value(prefabType, behaviour, "VehicleCenterOfMass");
    }
    #endregion

}
