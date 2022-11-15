using System.Collections;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Utilities;
using AnythingWorld.Behaviours;
public class PreyBehaviour : AWBehaviour
{
    [Range(0, 360)]
    public float escapeAngle = 180f;
    [Range(0, 360)]
    public float escapeAngleOffset = -90f;
    public float fleeRadius = 20f;
    public Vector3 movementDir;


    [HideInInspector]
    public Vector3 targetPos;
    public float moveSpeed = 5f;
    public float turnSpeed = 0.5f;
    public float randomTargetRegenTime = 2f;
    //bool beingChased = false;
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };

    private SphereCollider fleeRadiusCollider;

    private Vector3 escapeDir;
    private void Start()
    {
        InitializeAnimator();
        InitSphereTrigger();
        escapeDir = Vector3.zero;
        StartCoroutine(RegenRandomTarget());
    }

    private void InitSphereTrigger()
    {
        gameObject.AddComponent<Rigidbody>().useGravity = false;
        fleeRadiusCollider = gameObject.AddComponent<SphereCollider>();
        fleeRadiusCollider.isTrigger = true;
        fleeRadiusCollider.radius = fleeRadius;
    }
    private void Update()
    {
        gameObject.transform.position = AWThingTransform.position;
        SolveMovement();
    }



    private void SolveMovement()
    {
        if (ControllingAWObj == null)
        {

        }
        else
        {

            if (escapeDir != Vector3.zero)
            {
                escapeDir = escapeDir.normalized;
                RotateToDir(escapeDir);
                MoveToDir(escapeDir);
                DrawEditorGUIArrow.ForDebug(AWThingTransform.position, escapeDir *5, Color.red, 2f);
            }
            else
            {
                movementDir = (targetPos - AWThing.transform.localPosition);
                movementDir = new Vector3(movementDir.x, 0, movementDir.z);
                movementDir = movementDir.normalized;
                RotateToDir(movementDir);
                MoveToDir(movementDir);
                DrawEditorGUIArrow.ForDebug(AWThingTransform.position, movementDir * 5, Color.green, 2f);
            }


            #region Ex code
            /*
Vector3 m_DirToTarget = TargetPosition - AWThingTransform.position;

DrawArrow.ForDebug(AWThingTransform.transform.position, m_DirToTarget, Color.green, 2f);
// Check if the target hasn't been reached yet
if (Mathf.Abs(m_DirToTarget.x) < TargetRadius && Mathf.Abs(m_DirToTarget.y) < TargetRadius && Mathf.Abs(m_DirToTarget.z) < TargetRadius)
{
    // Pick new target position
    GetRandomTargetPos();
    m_DirToTarget = TargetPosition - AWThingTransform.position;
}
// Turn towards the target
Vector3 normalizedLookDirection = m_DirToTarget.normalized;
Quaternion m_LookRotation = Quaternion.LookRotation(normalizedLookDirection);
AWThingTransform.rotation = Quaternion.Slerp(AWThingTransform.rotation, m_LookRotation, Time.deltaTime * turnSpeed);
// Move animal towards the target
AWThingTransform.position =
*/
            #endregion

        }
    }

    private void RotateToDir(Vector3 dir)
    {
        // Turn towards the target
        if (dir != Vector3.zero)
        {
            var normalizedLookDirection = dir.normalized;
            var m_LookRotation = Quaternion.LookRotation(normalizedLookDirection);
            AWThingTransform.rotation = Quaternion.Slerp(AWThingTransform.rotation, m_LookRotation, Time.deltaTime * turnSpeed);
        }
    }
    private void MoveToDir(Vector3 dir)
    {
        // Move animal towards the target
        AWThingTransform.position = Vector3.Lerp(AWThingTransform.position, AWThingTransform.position + AWThingTransform.forward, moveSpeed * Time.deltaTime);
    }
    private IEnumerator RegenRandomTarget()
    {
        while (true)
        {
            GetRandomTargetPos();
            yield return new WaitForSeconds(randomTargetRegenTime);
        }
    }

    /// <summary>
    /// Method used for specifying the new target position.
    /// </summary>
    private void GetRandomTargetPos()
    {
        targetPos = new Vector3(Random.Range(-50f, 50f), AWThingTransform.position.y, Random.Range(-50f, 50f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PredatorBehaviour>())
        {
            var predatorForward = other.transform.position - AWThing.transform.position;
            var newDir = GenerateRandAngle(predatorForward);
            escapeDir = newDir;
        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Colliding");
    }
    private void OnTriggerStay(Collider other)
    {
        /*
        if (other.gameObject.GetComponent<PredatorBehaviour>())
        {
            if (escapeDir == Vector3.zero)
            {
                Debug.Log("Predator behaviour found");
                Vector3 predatorForward = other.transform.position - AWThing.transform.position;
                Vector3 newDir = GenerateRandAngle(predatorForward);
            }
            else
            {
                escapeDir = GenerateRandAngle(-escapeDir, -5, 5);
            }


        }
        else
        {
            if (escapeDir != Vector3.zero)
            {
                escapeDir = escapeDir + Vector3.left;
            }
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PredatorBehaviour>())
        {
            targetPos = escapeDir + AWThing.transform.localPosition;
            escapeDir = Vector3.zero;
        }
    }

    private Vector3 GenerateRandAngle(Vector3 dir, float minAngle = -45f, float maxAngle = 45f)
    {
        var angle = Random.Range(minAngle, maxAngle);
        var quaternion = Quaternion.Euler(0, angle, 0);
        var escapeAngle = quaternion * -dir;

        return escapeAngle;
    }
}
