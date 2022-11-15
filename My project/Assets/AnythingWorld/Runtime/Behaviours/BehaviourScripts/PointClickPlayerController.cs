using UnityEngine;
using UnityEngine.AI;
using AnythingWorld;
using AnythingWorld.Utilities;
using AnythingWorld.Behaviours;
/// <summary>
/// Generates navmesh and allows user to point and click to move agent.
/// </summary>
public class PointClickPlayerController : AWBehaviour
{
    #region Fields
    //public new string targetAnimationType = "walk";
    public float baseOffset = 5f;
    public float moveSpeed = 3f;
    public float animationSpeed = 2f;
    public float turnSpeed = 1f;
    public Camera cam;
    //private bool initialized = false;
    private float animSpeed;
    [SerializeField]
    private NavMeshSourceTagParent sourcetag;
    [SerializeField]
    private LocalNavMeshBuilder navbuilder;
    [SerializeField]
    public NavMeshAgent agent;
    RaycastHit m_HitInfo = new RaycastHit();
    private bool behaviourInitialised = false;
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };
    #endregion

    #region Unity Callback
    private void Start()
    {
        animSpeed = 1f;
    }
    void Update()
    {
        if (behaviourInitialised)
        {
            CreateNewNavMeshAgent();
            if (agent.isOnNavMesh)
            {
                SolveMovement();
            }
            else
            {
                DestroyNavMeshAgent();
            }

        }
        else
        {
            InitializeAnimator();
        }
    }
    public void OnEnable()
    {
        CreateNewNavMeshAgent();
    }
    #endregion

    #region Public 

    public override void RemoveAWAnimator()
    {
        base.RemoveAWAnimator();
        AnythingSafeDestroy.DestroyListOfObjects(new Object[] { agent, sourcetag, navbuilder });
    }
    public override void InitializeAnimator()
    {
        //if (AWThing == null) return;
        base.InitializeAnimator();

        if (!behaviourInitialised && AWThing != null)
        {
            this.AWThing.GetComponent<Rigidbody>().isKinematic = true;
            //this.GetComponent<Rigidbody>().isKinematic = true;
            if (cam == null)
            {
                cam = Camera.main;
            }
            CreateEnvironmentNavMesh();
            CreateNavMeshBuilder();
            behaviourInitialised = true;
        }
    }

    #endregion

    #region Private Methods

    private void SolveMovement()
    {
        //transform.localPosition = new Vector3(0f, transform.localPosition.y, 0f);
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                agent.destination = m_HitInfo.point;
        }
        if (agent.velocity.magnitude < 0.1f)
        {
            animator.SetMovementSpeed(0);
        }
        else
        {
            animator.SetMovementSpeed(animSpeed);
        }
    }
    private void CreateEnvironmentNavMesh()
    {
        if (sourcetag == null)
        {
            var env = GameObject.FindGameObjectWithTag("AWEnvironment");
            if (env)
            {
                if (!env.GetComponent<NavMeshSourceTag>())
                {
                    sourcetag = env.AddComponent<NavMeshSourceTagParent>();
                }
                else
                {
                    sourcetag = env.GetComponent<NavMeshSourceTagParent>();
                }
            }
        }
    }
    private void DestroyNavMeshAgent()
    {
        AnythingSafeDestroy.SafeDestroyDelayed(agent);
    }
    private void CreateNewNavMeshAgent()
    {
        if (agent == null && AWThing != null)
        {
            SnapToNavMesh();
            if (AWThing.gameObject.GetComponent<NavMeshAgent>())
            {
                agent = AWThing.gameObject.GetComponent<NavMeshAgent>();
            }
            else
            {
                agent = AWThing.gameObject.AddComponent<NavMeshAgent>();
            }
            agent.baseOffset = 5f;
            agent.angularSpeed = 60f;
            agent.speed = 6f;
        }
    }
    private void CreateNavMeshBuilder()
    {
        if (navbuilder == null)
        {
            if (gameObject.GetComponent<LocalNavMeshBuilder>())
            {
                navbuilder = gameObject.GetComponent<LocalNavMeshBuilder>();
            }
            else
            {
                navbuilder = gameObject.AddComponent<LocalNavMeshBuilder>();
            }

            navbuilder.m_Tracked = AWThing.gameObject.transform;
        }
    }
    private NavMeshHit SnapToNavMesh()
    {
        var hit = new NavMeshHit();
        var maxRadius = 20f;
        float radius = 0;

        while (radius < maxRadius)
        {
            NavMesh.SamplePosition(transform.position, out hit, radius, NavMesh.AllAreas);
            if (hit.hit)
            {

                transform.position = hit.position + new Vector3(0f, baseOffset, 0f);
                return hit;
            }
            radius += 2;
        }
        return hit;
    }
    #endregion
}
