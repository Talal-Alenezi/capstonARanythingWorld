
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Utilities;
using AnythingWorld.Behaviours;

/// <summary>
/// Generates random points on the X/Z plane and moves agent towards it.
/// </summary>
public class RandomMovement : AWBehaviour
{
    #region Fields
    protected override string[] targetAnimationType { get; set; } = { "walk", "drive", "default" };
    public float turnSpeed;
    public float moveSpeed;
    // Target position the animal is trying to reach.
    public Vector3 TargetPosition { get; set; } = new Vector3(0f, 0f, 0f);
    // Radius for target position.
    public float targetRadius = 15f;
    public float SpeedMultiplier { get; set; } = 1f;

    [SerializeField]
    public Vector3 randomTargetPoint = new Vector3(0f, 0f, 0f);
    [SerializeField]
    public GameObject anchor;

    [SerializeField]
    public float targetSpawnRadius = 40f;
    public bool showInSceneControls = true;
    [SerializeField]
    private Vector3 anchorPos = Vector3.zero;
    private Vector3 m_DirToTarget = Vector3.zero;
    public Vector3 AnchorPos
    {
        get
        {
            return anchorPos;
        }
        set
        {
            anchorPos = value;
        }
    }

    public float GenerationRadius
    {
        get
        {
            return Mathf.Abs(targetSpawnRadius);
        }
        set
        {
            targetSpawnRadius = Mathf.Abs(value);
        }
    }
    #endregion

    void Start()
    {
        GetRandomTargetPos();
        InitializeAnimator();
    }
    public void Update()
    {
        SolveMovement();
    }

    private void SolveMovement()
    {
        if (ControllingAWObj != null)
        {
            //Debug.Log("solving movement" + controllingAWObj.gameObject.name);
            gameObject.transform.position = AWThing.transform.position;
            m_DirToTarget = new Vector3(randomTargetPoint.x, transform.position.y, randomTargetPoint.z) - AWThingTransform.position;

            //DrawEditorGUIArrow.ForDebug(AWThingTransform.transform.position, m_DirToTarget, Color.green, 2f);

            if (Vector3.Distance(randomTargetPoint, anchorPos) > GenerationRadius)
            {
                GetRandomTargetPos();
            }

            if (Vector3.Distance(AWThing.transform.position, randomTargetPoint) < targetRadius)
            {
                GetRandomTargetPos();
                m_DirToTarget = randomTargetPoint - AWThingTransform.position;
            }
            // Turn towards the target
            var normalizedLookDirection = m_DirToTarget.normalized;
            var m_LookRotation = Quaternion.LookRotation(normalizedLookDirection);
            AWThingTransform.rotation = Quaternion.Slerp(AWThingTransform.rotation, m_LookRotation, Time.deltaTime * turnSpeed);
            // Move animal towards the target
            AWThingTransform.position = Vector3.Lerp(AWThingTransform.position, AWThingTransform.position + (AWThingTransform.forward*AWThingTransform.lossyScale.magnitude), SpeedMultiplier * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Method used for specifying the new target position.
    /// </summary>
    private void GetRandomTargetPos()
    {
        var randomPosition = Random.insideUnitSphere * targetSpawnRadius;
        randomPosition = new Vector3(randomPosition.x, anchorPos.y + 1f, randomPosition.z);
        randomPosition = anchorPos + randomPosition;
        randomTargetPoint = randomPosition;

    }
    public void OnDrawGizmosSelected()
    {
        //Debug.DrawRay(AWThingTransform.transform.position, m_DirToTarget, Color.green);
    }

    /// <summary>
    /// Finds parentPrefab type and gets animator script from animatorMapSettings, adds to object.
    /// </summary>
    public override void SetDefaultParametersValues()
    {
        base.SetDefaultParametersValues();
        var prefabType = AWThing.GetComponentInParent<AWObj>().GetObjCatBehaviour();
        var behaviour = "RandomMovement";
        var settings = ScriptableObject.CreateInstance<PrefabAnimationsDictionary>();
        var tSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "turnSpeed");

        if (turnSpeed == 0)
        {
            turnSpeed = tSpeed;
        }
        var mSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "moveSpeed");

        if (moveSpeed == 0)
        {
            moveSpeed = mSpeed;
        }
    }
}
