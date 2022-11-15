using UnityEngine;
using UnityEngine.AI;
using AnythingWorld;
using AnythingWorld.Animation;
using AnythingWorld.Behaviours;
public class RandomMovementNavmesh : AWBehaviour
{
    #region Fields
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };
    public float turnSpeed;
    public float moveSpeed;
    public float m_Range = 25.0f;
    NavMeshAgent m_Agent;

    #endregion
    private float timeSet;
    void Start()
    {
        timeSet = Time.time;
        InitAgent();
    }

    void Update()
    {
        SolveMovement();
    }

    private void SolveMovement()
    {
        if (m_Agent != null)
        {
            var elapsed = Time.time - timeSet;

            if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh)
            {
                if (m_Agent.pathPending || m_Agent.remainingDistance > m_Agent.radius && elapsed < 10f)
                    return;
            }
            else
            {
                return;
            }


            var randomDir = Random.insideUnitSphere * m_Range;
            randomDir += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDir, out hit, m_Range, 1);
            var finalPos = hit.position;
            m_Agent.destination = finalPos;

            //m_Agent.destination =  m_Agent.gameObject.transform.position * (m_Range * Random.insideUnitCircle);
            timeSet = Time.time;

        }
        else
        {
            InitAgent();
        }

    }
    private void InitAgent()
    {
        if (AWThing != null)
        {
            m_Agent = AWThing.GetComponent<NavMeshAgent>();
            if (m_Agent == null)
            {
                AWThing.AddComponent<NavMeshAgent>();
            }
        }
    }
    public override void SetDefaultParametersValues()
    {
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
