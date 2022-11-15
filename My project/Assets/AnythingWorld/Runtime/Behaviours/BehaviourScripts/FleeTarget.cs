using UnityEngine;
using System;
using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Behaviours;
public class FleeTarget : AWBehaviour
{
    #region Fields
    //AWBehaviour requireds
    //public new static string targetAnimationType = "walk";
    //Animator Reference
    //Behaviour params
    public bool Flee;
    public float moveSpeed = 12f;
    public float animationSpeed = 2f;
    public float turnSpeed = 1f;
    public float TargetRadius { get; set; } = 20f;
    public Transform TargetTransform { get; set; }
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };
    private Vector3 dir;
    #endregion

    #region Unity Callbacks
    void Update()
    {

        if (TargetTransform != null)
        {
            SolveMovement();
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Moves transform towards target position, stops when within targetRadius.
    /// </summary>
    /// <param name="_targetPosition">Transform to move target towards.</param>
    private void SolveMovement()
    {

        var targetPosition = new Vector3(TargetTransform.position.x, transform.position.y, TargetTransform.position.z);
        // Update position to target
        var m_DirToTarget = AWThingTransform.position - targetPosition;

        // Check if target is within target radius
        if (Mathf.Abs(m_DirToTarget.x) < TargetRadius && Mathf.Abs(m_DirToTarget.y) < TargetRadius && Mathf.Abs(m_DirToTarget.z) < TargetRadius)
        {
            Debug.Log("FLEEING");
            var animator = gameObject.GetComponent<AnythingAnimationController>();
            animator.SetMovementSpeed(animationSpeed);

            // Turn towards the target
            var normalizedLookDirection = m_DirToTarget.normalized;
            var m_LookRotation = Quaternion.LookRotation(normalizedLookDirection);
            AWThingTransform.rotation = Quaternion.Slerp(AWThingTransform.rotation, m_LookRotation, Time.deltaTime * turnSpeed);

            // Move animal towards the target
            AWThingTransform.position = Vector3.Lerp(AWThingTransform.position, AWThingTransform.position + AWThingTransform.forward, moveSpeed * Time.deltaTime);

        }
        else
        {
            //If within radius stop animation walk.
            if (animator)
            {
                animator.SetMovementSpeed(0);
            }
            else
            {
                AddAWAnimator();
            }

        }

    }
    #endregion

    #region Public Methods
    public Vector3 GetTargetTransformVector()
    {
        if (TargetTransform == null)
        {
            return new Vector3(0f, 0f, 0f);
        }
        else
        {
            return TargetTransform.position;
        }

    }
    #endregion 

}
