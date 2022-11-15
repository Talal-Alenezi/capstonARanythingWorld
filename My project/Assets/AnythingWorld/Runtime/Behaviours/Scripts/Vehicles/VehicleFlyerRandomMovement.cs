using System.Collections.Generic;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Behaviours;
public class VehicleFlyerRandomMovement : AWBehaviour
{
    private AWObj _controllingAWObj;

    // Behaviour settings
    public float movementSpeed = 500;
    public float movementTurnSpeed = 2;
    public float targetSpawnRadius = 75;
    public int targetHeightsDifference = 25;
    public float shakeDegree = 15;
    public float bankingDegree = 30;
    int startingHeightsDifference = 20;

    // Helper variable used for the shaking movement
    int timeCount;
    // Direction of the rotation that simulates the shake of the plane
    float shakeDirection = 1.0f;
    // List of all the parameters used in this behaviour
    List<Parameter> _parametersList;
    // Target position the vehicle is trying to reach
    Vector3 targetPosition;
    // Maximum frequency of the shakes
    int N = 1800;
    // Minimum frequency of the shakes
    int M = 700;


    // ParameterController object used for monitoring parameters' changes
    ParameterController paramControl;

    // BehaviourSettings object storing all the needed information regarding behaviour
    BehaviourSettings _settings;

    List<PrefabPartToAnimationSubscript> _prefabToScriptType;
    //Dictionary<(string, string), AWBehaviourSubscripts> _prefabToScript;

    protected override string[] targetAnimationType { get; set; } = { "default" };

    void Start()
    {
        targetHeightsDifference = startingHeightsDifference;
        //ControllingAWObj = gameObject.GetComponentInParent<AWObj>();
        // Pick the position of the target 
        GetRandomTargetPos();

        // Set the rotation of the plane 
        AWThingTransform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));
    }

    // Update is called once per frame
    void Update()
    {
        // Define the shake rotation of the plane
        timeCount--;

        // Change the direction of the shake
        if (shakeDirection == 1.0f && timeCount == 0)
            shakeDirection = -1.0f;
        else if (shakeDirection == -1.0f && timeCount == 0)
            shakeDirection = 1.0f;

        // Define the new shake frequency
        if (timeCount == 0)
            timeCount = Random.Range(M, N);


        // Update position to target
        var m_DirToTarget = targetPosition - AWThingTransform.position;


        // Check if the target hasn't been reached yet
        if (Mathf.Abs(m_DirToTarget.x) < 20 && Mathf.Abs(m_DirToTarget.y) < 20 && Mathf.Abs(m_DirToTarget.z) < 20)
        {
            // Pick new target position
            GetRandomTargetPos();
            m_DirToTarget = targetPosition - AWThingTransform.position;
        }

        // Turn towards the target adding banking and shaking
        var normalizedLookDirection = m_DirToTarget.normalized;
        var bank = bankingDegree * -Vector3.Dot(AWThingTransform.right, normalizedLookDirection); // banking movement
        var shake = shakeDirection * shakeDegree; // shaking movement
        var m_LookRotation = Quaternion.LookRotation(normalizedLookDirection) * Quaternion.AngleAxis(bank, Vector3.forward) * Quaternion.AngleAxis(shake, Vector3.forward);
        AWThingTransform.rotation = Quaternion.Slerp(AWThingTransform.rotation, m_LookRotation, Time.deltaTime * movementTurnSpeed);

        // Move vehicle towards the target
        AWThingTransform.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(AWThingTransform.position, AWThingTransform.position + AWThingTransform.forward, movementSpeed * Time.deltaTime));

    }

    /// <summary>
    /// Method used for specifying the new target position. New target position is restricted by the distance between the new and the old target position along Y axis,
    /// so that the plane is not moving rapidly down.
    /// </summary>
    void GetRandomTargetPos()
    {
        var oldPosition = targetPosition;
        do
        {
            targetPosition = Random.insideUnitSphere * targetSpawnRadius;
            targetPosition.y += targetSpawnRadius;
        } while (Mathf.Abs(oldPosition.y - targetPosition.y) > targetHeightsDifference);
    }

}
