using System.Collections;
using UnityEngine;
using AnythingWorld;
public class FlyingVehicleController : MonoBehaviour
{
    // The speed of the vehicle movement
    public float moveSpeed = 500f;

    // The speed of the vehicle rotation
    public float turnSpeed = 2f;

    // The radius of the sphere inside which target's position can be specified
    public float targetSpawnRadius = 75f;

    // The difference in height between the old and new target position 
    public int targetHeightsDifference = 25;

    // Shake degree
    public float shakeDegree = 15f;

    // Banking degree
    public float bankingDegree = 30f;


    // Target position the vehicle is trying to reach
    Vector3 targetPosition;

    // Maximum frequency of the shakes
    int N = 1800;

    // Minimum frequency of the shakes
    int M = 700;

    // Direction of the rotation that simulates the shake of the plane
    float shakeDirection = 1.0f;

    // Helper variable used for the shaking movement
    int timeCount;

    private AWObj _controllingAWObj;
    private bool _startMotion;

    /// <summary>
    /// Initialization of the system. Setting the target position and plane rotation.
    /// </summary>
    void Start()
    {
        _startMotion = false;


        _controllingAWObj = transform.parent.GetComponent<AWObj>();
        if (_controllingAWObj == null)
        {
            Debug.LogError($"No AWObj found for {gameObject.name}");
            return;
        }

        StartCoroutine(WaitForAWObjCompletion());
    }

    private IEnumerator WaitForAWObjCompletion()
    {
        while (!_controllingAWObj.ObjMade)
            yield return new WaitForEndOfFrame();

        // Pick the position of the target 
        GetRandomTargetPos();

        // Set the rotation of the plane 
        transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));

        timeCount = N;
        _startMotion = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_startMotion)
            return;

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
        var m_DirToTarget = targetPosition - transform.position;


        // Check if the target hasn't been reached yet
        if (Mathf.Abs(m_DirToTarget.x) < 20 && Mathf.Abs(m_DirToTarget.y) < 20 && Mathf.Abs(m_DirToTarget.z) < 20)
        {
            // Pick new target position
            GetRandomTargetPos();
            m_DirToTarget = targetPosition - transform.position;
        }

        // Turn towards the target adding banking and shaking
        var normalizedLookDirection = m_DirToTarget.normalized;
        var bank = bankingDegree * -Vector3.Dot(transform.right, normalizedLookDirection); // banking movement
        var shake = shakeDirection * shakeDegree; // shaking movement
        var m_LookRotation = Quaternion.LookRotation(normalizedLookDirection) * Quaternion.AngleAxis(bank, Vector3.forward) * Quaternion.AngleAxis(shake, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_LookRotation, Time.deltaTime * turnSpeed);

        // Move vehicle towards the target
        transform.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(transform.position, transform.position + transform.forward, moveSpeed * Time.deltaTime));

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
