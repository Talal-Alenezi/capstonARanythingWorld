using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Behaviours;
using UnityEngine;
public class HoppingMovement : AWBehaviour
{
    #region Fields
    public float movementSpeed = 6f;
    public float turnSpeed = 5f;
    public float jumpAmount = 10f;
    public float jumpTime = 1f;
    private Rigidbody rBody;
    private float forceMultiplier = 1000f;
    private float startY;
    private Quaternion targetRotation;
    private Vector3 jumpForce;

    [SerializeField]
    [HideInInspector]
    private bool inputsMultiplied = false;
    private Vector3 targetPosition;
    private GroundDetect groundDetect;
    protected override string[] targetAnimationType { get; set; } = { "hop", "default" };

    //private bool _isJumping = true;
    #endregion
    public void OnEnable()
    {
        InitializeAnimator();
        Init();
    }
    public void FixedUpdate()
    {
        if (!behaviourInitialized) OnEnable();
        var lookRotation = targetPosition;
        lookRotation.y = AWThingTransform.position.y;

        var targRot = Quaternion.Lerp(AWThingTransform.rotation, Quaternion.LookRotation(lookRotation - AWThingTransform.position), turnSpeed * Time.deltaTime);

        targetRotation = targRot;

        AWThingTransform.rotation = targetRotation;

        var targSqrMag = Vector3.SqrMagnitude(AWThingTransform.position - targetPosition);

        if (targSqrMag < 50f)
        {
            UpdateTarget();
        }
        var BodyTransform = AWThingTransform.Find("body");
        if (BodyTransform)
        {
            BodyTransform.position = AWThingTransform.position;
            BodyTransform.rotation = AWThingTransform.rotation;
        }
        rBody.velocity = new Vector3(0f, rBody.velocity.y, 0f) + (AWThingTransform.forward * movementSpeed);
    }




    public void AddJumpForce()
    {
        var hopperForce = Vector3.zero;
        GetJumpForce();

        if (jumpForce != Vector3.zero)
        {
            hopperForce += jumpForce;

            jumpForce = Vector3.zero;
        }

        rBody.AddForce(hopperForce, ForceMode.Force);





    }
    private void Init()
    {
        try
        {
            if (!inputsMultiplied)
            {
                MultiplyInputs();
            }
            jumpForce = Vector3.zero;
            rBody = AWThingTransform.GetComponent<Rigidbody>();
            startY = AWThingTransform.localPosition.y;
            groundDetect = AWThingTransform.gameObject.AddComponent<GroundDetect>();
            UpdateTarget();
            groundDetect.hitsGroundDelegate = AddJumpForce;
            rBody.AddForce(AWThingTransform.forward);
            behaviourInitialized = true;
        }
        catch
        {
            Debug.LogError("Error initializing HoppingMovement");
            return;
        }
    }

    private void MultiplyInputs()
    {
        //MovementSpeed *= delaMultiplier;
        jumpAmount *= forceMultiplier / 10;
        inputsMultiplied = true;
    }
    private void GetJumpForce()
    {
        jumpForce = Vector3.up * jumpAmount;
    }
    private void UpdateTarget()
    {
        targetPosition = new Vector3(Random.Range(-50f, 50f), AWThingTransform.position.y, Random.Range(-50f, 50f));
    }

    public override void SetDefaultParametersValues()
    {
        var prefabType = gameObject.GetComponentInParent<AWObj>().GetObjCatBehaviour();
        var behaviour = "HoppingMovement";
        var settings = ScriptableObject.CreateInstance<PrefabAnimationsDictionary>();
        movementSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "MovementSpeed");
        turnSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "TurnSpeed");
        jumpAmount = settings.GetDefaultParameterValue(prefabType, behaviour, "JumpAmount");
        jumpTime = settings.GetDefaultParameterValue(prefabType, behaviour, "JumpTime");


    }


}
