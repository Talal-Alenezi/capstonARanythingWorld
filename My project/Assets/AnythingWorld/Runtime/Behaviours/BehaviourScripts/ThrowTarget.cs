using UnityEngine;
using AnythingWorld;
using AnythingWorld.Utilities;
public class ThrowTarget : MonoBehaviour
{
    #region Fields

    public float ThrowSpeed = 20f;
    public float TurnSpeed = 20f;
    public float TargetRadius = 1f;


    public PathConfig config;

    public AWObj targetController = null;
    public Transform TargetTransform = null;
    private Vector3 backupTransform = new Vector3(0f, 0f, 0);
    private Vector3 targetPos;
    private Vector3 startPos;
    private float curveHeight = 10f;
    private float incrementor;

    private Transform AWThingTransform;

    #endregion

    #region Unity Callbacks

    void Start()
    {
        startPos = transform.position;
        incrementor = 0f;
    }

    void Update()
    {
        if (TargetTransform != null)
        {
            if (AWThingTransform != null)
            {
                SolveMovement();
            }
            else
            {
                AWThingTransform = transform;
            }
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
            targetPos = new Vector3(TargetTransform.position.x, TargetTransform.position.y, TargetTransform.position.z);
        }


        // Update position to target
        var m_DirToTarget = targetPos - AWThingTransform.position;

        DrawEditorGUIArrow.ForDebug(AWThingTransform.transform.position, m_DirToTarget, Color.red, 2f);
        // Check if the target hasn't been reached yet
        if (Mathf.Abs(m_DirToTarget.x) < TargetRadius && Mathf.Abs(m_DirToTarget.y) < TargetRadius && Mathf.Abs(m_DirToTarget.z) < TargetRadius)
        {
            TargetTransform = null;
        }
        else
        {
            incrementor += 0.04f;
            var currentPos = Vector3.Lerp(startPos, targetPos, incrementor);
            currentPos.y += curveHeight * Mathf.Sin(Mathf.Clamp01(incrementor) * Mathf.PI);
            transform.position = currentPos;
        }

    }
    private Vector3 GetRandomTargetPos()
    {
        return new Vector3(Random.Range(-50f, 50f), transform.position.y, Random.Range(-50f, 50f));
    }
    #endregion

    #region Public Methods
    #endregion
}