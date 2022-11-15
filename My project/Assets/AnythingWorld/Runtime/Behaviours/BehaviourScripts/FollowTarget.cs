using UnityEngine;
using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Utilities;
using AnythingWorld.Behaviours;
public class FollowTarget : AWBehaviour
{
    #region Fields
    public static string targetPrefabType = "quadruped";
    public float turnSpeed;
    public float moveSpeed;
    public bool ReachedTarget;
    public bool Flee;
    public float MoveSpeed { get; set; } = 3f;
    public float AnimationSpeed { get; set; } = 2f;
    public float TurnSpeed { get; set; } = 1f;
    public float TargetRadius { get; set; } = 1f;
    public float SpeedMultiplier { get; set; } = 1f;
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };

    public PathConfig config;

    public AWObj targetController = null;
    public Transform TargetTransform = null;
    private Vector3 backupTransform = new Vector3(0f, 0f, 0);
    private Vector3 targetPos;
    #endregion

    #region Unity Callbacks

    void Start()
    {
        ReachedTarget = false;
    }

    void Update()
    {
        if (!animatorInitialized) InitializeAnimator();
        if (TargetTransform == null)
        {
            if (targetController != null)
            {
                if (targetController.ObjMade == true && targetController.PrefabTransform.transform != null) TargetTransform = targetController.PrefabTransform.transform;
            }

        }
        else
        {
            SolveMovement();
        }
    }
    #endregion

    #region Private Methods
    private void SolveMovement()
    {
        if (TargetTransform == null)
        {
            targetPos = backupTransform;
        }
        else
        {
            targetPos = new Vector3(TargetTransform.position.x, AWThingTransform.position.y, TargetTransform.position.z);
        }


        // Update position to target
        var m_DirToTarget = targetPos - AWThingTransform.position;

        //DrawEditorGUIArrow.ForDebug(AWThingTransform.transform.position, m_DirToTarget, Color.red, 2f);
        // Check if the target hasn't been reached yet
        if (Mathf.Abs(m_DirToTarget.x) < TargetRadius && Mathf.Abs(m_DirToTarget.y) < TargetRadius && Mathf.Abs(m_DirToTarget.z) < TargetRadius)
        {
            ReachedTarget = true;
            //animator.SetMovementSpeed(0);
        }
        else
        {
            //Update animation speed.
            //animator.SetMovementSpeed(AnimationSpeed);
            // Turn towards the target
            var normalizedLookDirection = m_DirToTarget.normalized;
            var m_LookRotation = Quaternion.LookRotation(normalizedLookDirection);
            AWThingTransform.rotation = Quaternion.Slerp(AWThingTransform.rotation, m_LookRotation, Time.deltaTime * turnSpeed);
            // Move animal towards the target
            AWThingTransform.position = Vector3.Lerp(AWThingTransform.position, AWThingTransform.position + AWThingTransform.forward, SpeedMultiplier * moveSpeed * Time.deltaTime);

            // Debug.Log("moveSpeed = " + moveSpeed);
        }

    }
    public override void SetDefaultParametersValues()
    {
        var prefabType = AWThing.GetComponentInParent<AWObj>().GetObjCatBehaviour();
        var behaviour = "RandomMovement";
        var settings = ScriptableObject.CreateInstance<PrefabAnimationsDictionary>();
        var tSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "turnSpeed");
        if (tSpeed != 0) turnSpeed = tSpeed;
        var mSpeed = settings.GetDefaultParameterValue(prefabType, behaviour, "moveSpeed");
        if (mSpeed != 0) moveSpeed = mSpeed;
    }

    private Vector3 GetRandomTargetPos()
    {
        return new Vector3(Random.Range(-50f, 50f), transform.position.y, Random.Range(-50f, 50f));
    }
    #endregion

    #region Public Methods
    public override void InitializeAnimator()
    {
        base.InitializeAnimator();
        if (targetController != null)
        {
            if (targetController.PrefabTransform != null)
            {
                TargetTransform = targetController.PrefabTransform.transform;
            }
            // of no AWThing, must be Poly object without behaviours
            else
            {
                TargetTransform = targetController.transform;
            }
        }
        backupTransform = GetRandomTargetPos();
    }
    #endregion
}