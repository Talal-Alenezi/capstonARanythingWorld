using UnityEngine;
using AnythingWorld.Behaviours;

/// <summary>
/// Third person character controller, moves relative to camera forward direction.
/// </summary>
public class ThirdPersonCharacterController : AWBehaviour
{
    public Transform cam;
    private CharacterController controller;
    private Transform lastAngle;
    public float turnSmoothTime = 0.5f;
    private float smoothTurnVelocity = 0.05f;
    private float speed = 6f;
    private float animSpeed = 2f;

    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };

    private void Start()
    {
        SetUpController();
    }

    /// <summary>
    /// Character controller added to the AWThing transform and main camera found.
    /// Removes rigidbody that constrains model by default.
    /// </summary>
    private void SetUpController()
    {
        controller = AWThing.GetComponent<CharacterController>();
        if (!controller)
        {
            controller = AWThing.AddComponent<CharacterController>();
        }
        cam = Camera.main.transform;

        Destroy(AWThing.GetComponent<Rigidbody>());
    }

    //Chooses type of animator used for this behaviour, walk and then default animation.

    void Update()

    {

        SolveMovement();

    }

    /// <summary>
    /// Get horizontal and vertical movement components and the apply to character
    /// controller relative to the main camera direction.
    /// Adjusts animation speed.
    /// </summary>
    private void SolveMovement()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        Vector4 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(AWThing.transform.eulerAngles.y, targetAngle, ref smoothTurnVelocity, turnSmoothTime);
            AWThing.transform.localRotation = Quaternion.Euler(0f, angle, 0f);

            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            animator.SetMovementSpeed(animSpeed);

        }
        else
        {
            animator.SetMovementSpeed(0);
        }
    }
}
