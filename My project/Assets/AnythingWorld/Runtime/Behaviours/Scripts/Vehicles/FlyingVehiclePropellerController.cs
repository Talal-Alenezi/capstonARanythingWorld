using AnythingWorld.Utilities;
using UnityEngine;
using AnythingWorld;
public class FlyingVehiclePropellerController : MonoBehaviour
{
    // The speed of the vehicle's movement
    public float moveSpeed = 70f;

    // The speed of the vehicle's rotation
    public float turnSpeed = 2f;

    // The speed of the vehicle's wings
    public float wingsSpeed = 100f;

    // The radius of the sphere inside which target's position can be specified
    public float targetSpawnRadius = 75f;

    // The minimum difference in height between the old and new target position 
    public int targetHeightsDifference = 35;



    // The degree defining the movement of the vehicle's wings
    private float wingsSpinDegree = 10.0f;

    // The degree defining the vehicle's tilt
    //float tiltDegree = 35.0f;

    // Banking degree
    //float bankingDegree = 30f;

    // Transform storing the vehicle's wings
    private Transform top_wings;

    // Transform storing the vehicle's body and wings
    private Transform body;

    // Target position the vehicle is trying to reach
    private Vector3 targetPosition;

    private AWObj parentAwobj;
    private bool controllerInitialized;
    /// <summary>
    ///  Initialization of the system. Setting the target position and plane rotation. Assigning needed transforms and setting the center of the wings
    /// </summary>
    void Start()
    {
        if(transform.parent.TryGetComponent<AWObj>(out var awobj))
        {
            parentAwobj = awobj;
        }
        else
        {
            Debug.LogError("No awobj attached to this object");
            Destroy(this);
        }

    }

    
    private void InitializeController()
    {
        if (!parentAwobj.ObjMade) return;
        // Get body and wings Transforms
        body = GetComponentInChildren<GetPart>().transform;
        top_wings = body.GetChild(0).transform;

        top_wings.position = GetWingsCenter(top_wings.position.y);

        //Set the rotation of the plane
        transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));
        controllerInitialized = true;
    }

    /// <summary>
    /// Function used to obtain the center point between the vehicle's wings.
    /// </summary>
    /// <returns></returns>
    Vector3 GetWingsCenter(float yPos)
    {
        var center = new Vector3();

        // Find a center of each wing and sum them
        for (var i = 0; i < top_wings.childCount; i++)
        {
            if (top_wings.GetChild(i))
            {
                if (top_wings.GetChild(i).TryGetComponent<MeshRenderer>(out var renderer))
                {
                    center += renderer.bounds.center;
                }
            }
            center += top_wings.GetChild(i).GetComponentInChildren<MeshRenderer>().bounds.center;
        }

        // Find the average of the 3 points
        center = center / top_wings.childCount;
        center.y = yPos;

        return center;
    }

    private void FixedUpdate()
    {
        if (controllerInitialized)
        {
            // Rotate top wings
            top_wings.Rotate(0, wingsSpinDegree * Time.deltaTime * wingsSpeed, 0);
        }
        else
        {
            InitializeController();
        }


    }

}
