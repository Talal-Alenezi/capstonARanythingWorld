using UnityEngine;
using AnythingWorld;
using AnythingWorld.Utilities;
public class CarryTarget : MonoBehaviour
{
    #region Fields
    public float ThrowSpeed = 20f;
    public float TurnSpeed = 20f;
    public float TargetRadius = 1f;


    public PathConfig config;

    public AWObj targetController = null;
    public Transform TargetTransform = null;
    private Vector3 targetPos;
    private Vector3 startPos;

    private Transform AWThingTransform;
    private AWObj carriedAWObj;

    private Transform headTransform;
    private Transform carriedTransform;

    #endregion

    #region Unity Callbacks

    void Start()
    {
        startPos = transform.position;
        carriedTransform = transform;
    }

    void Update()
    {
        if (TargetTransform == null)
        {
            if (targetController != null)
            {
                if (targetController.ObjMade == true && targetController.PrefabTransform.transform != null) TargetTransform = targetController.PrefabTransform.transform;
                carriedAWObj = gameObject.GetComponentInChildren<AWObj>();
                carriedAWObj.ActivateRBs(false);

                var carriedColliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (var carriedCollider in carriedColliders)
                {
                    carriedCollider.enabled = false;
                }

                transform.Rotate(30, 0, 90);

                var targetTransforms = targetController.GetComponentsInChildren<Transform>();
                foreach (var t in targetTransforms)
                {
                    if (t.name == "head")
                    {
                        var objRenderers = t.GetComponentsInChildren<Renderer>();
                        var bounds = new Bounds(t.position, Vector3.zero);
                        foreach (var objRenderer in objRenderers)
                        {
                            bounds.Encapsulate(objRenderer.bounds);
                            var headCube = new GameObject();
                            headCube.transform.position = bounds.center + bounds.extents;
                            headCube.transform.parent = t;
                            TargetTransform = headCube.transform;
                        }
                    }
                }

                var thisTransforms = transform.GetComponentsInChildren<Transform>();
                foreach (var t in thisTransforms)
                {
                    if (t.tag == "AWThing")
                    {
                        carriedTransform = t;
                        startPos = carriedTransform.position;
                    }
                }
            }

        }
        else
        {
            SolveMovement();
        }
    }
    #endregion

    #region Private Methods
    private void SolveMovement()
    {
        targetPos = TargetTransform.position;

        // Update position to target
        var m_DirToTarget = targetPos - carriedTransform.position;

        DrawEditorGUIArrow.ForDebug(carriedTransform.position, m_DirToTarget, Color.red, 2f);

        var currentPos = Vector3.Lerp(carriedTransform.position, targetPos, 0.9f);

        carriedTransform.position = targetPos;
        carriedTransform.eulerAngles = TargetTransform.eulerAngles - new Vector3(0, 90, -90);
    }
    #endregion

    #region Public Methods
    #endregion
}