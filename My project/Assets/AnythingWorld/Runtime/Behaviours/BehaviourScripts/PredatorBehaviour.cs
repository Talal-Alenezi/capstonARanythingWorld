using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Utilities;
using AnythingWorld.Behaviours;

public class PredatorBehaviour : AWBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 1f;
    public float fleeRadius = 20f;
    public float chaseTimeInSeconds = 5f;
    public float randomTargetRegenTime = 2f;
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };

    private SphereCollider chaseRadiusCollider;
    [HideInInspector]
    private Vector3 targetPos;
    private Vector3 chaseDir;
    public Vector3 movementDir;



    private void InitSphereTrigger()
    {
        gameObject.AddComponent<Rigidbody>().useGravity = false;
        chaseRadiusCollider = gameObject.AddComponent<SphereCollider>();
        chaseRadiusCollider.isTrigger = true;
        chaseRadiusCollider.radius = fleeRadius;
    }
    private void Start()
    {
        InitSphereTrigger();
        StartCoroutine(RegenRandomTarget());
        chaseDir = Vector3.zero;
        movementDir = Vector3.one;
        
    }
    private void Update()
    {
        SolveMovement();
    }
    private void SolveMovement()
    {
        if (ControllingAWObj != null)
        {
            gameObject.transform.position = AWThingTransform.position;
            if (chaseDir != Vector3.zero)
            {
                chaseDir = chaseDir.normalized;
                RotateToDir(chaseDir);
                MoveToDir(chaseDir);
                DrawEditorGUIArrow.ForDebug(AWThingTransform.position, chaseDir * 5, Color.red, 2f);
            }
            else
            {
                movementDir = (targetPos - AWThing.transform.localPosition) + new Vector3(0.1f, 0.1f, 0.1f);
                movementDir = new Vector3(movementDir.x, 0, movementDir.z);
                movementDir = movementDir.normalized;
                RotateToDir(movementDir);
                MoveToDir(movementDir);
                DrawEditorGUIArrow.ForDebug(AWThingTransform.position, movementDir * 5, Color.green, 2f);
            }
        }
    }
    private void RotateToDir(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            // Turn towards the target
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
    private void GetRandomTargetPos()
    {
        targetPos = new Vector3(Random.Range(-50f, 50f), gameObject.transform.position.y, Random.Range(-50f, 50f));
    }
    private Transform currentPreyTransform;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PreyBehaviour>())
        {

            currentPreyTransform = other.gameObject.transform;
            Debug.Log("Moving towards prey!");
            chaseDir = currentPreyTransform.position - gameObject.transform.position;
            StartCoroutine(ChaseRoutine());
        }
    }
    private IEnumerator ChaseRoutine()
    {
        for(var x = 0; x < chaseTimeInSeconds; x++)
        {
            chaseDir = currentPreyTransform.position - gameObject.transform.position;
            yield return new WaitForSeconds(1);
        }
        chaseDir = Vector3.zero;
    }

}
