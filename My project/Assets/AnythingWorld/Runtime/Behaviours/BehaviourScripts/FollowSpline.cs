using PathCreation;
using UnityEngine;
using AnythingWorld;
using AnythingWorld.Utilities;
using AnythingWorld.Behaviours;
public class FollowSpline : AWBehaviour
{
    #region Fields
    [SerializeField]
    public PathCreator pathCreator;
    [SerializeField]
    public PathFollower pathFollower;
    [SerializeField]
    public GameObject pathHolder;

    public bool loadDefaultPath = true;
    //AWBehaviour requireds
    public bool spawnAtObject = true;
    public static string targetPrefabType = "quadruped";
    //public new static string targetAnimationType = "walk";
    [SerializeField, HideInInspector]
    protected override string[] targetAnimationType { get; set; } = { "walk", "default" };
    private bool pathInitialized = false;

    #endregion

    #region Unity Callbacks

    #endregion

    #region AWBehaviour Methods
    public override void InitializeAnimator()
    {
        base.InitializeAnimator();
        if (AWThing.gameObject.GetComponent<Rigidbody>())
        {
            AWThing.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        //ParentAWObj.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        if (!pathInitialized)
        {
            if (pathHolder == null)
            {
                pathHolder = new GameObject();
                pathHolder.transform.parent = transform.parent;


                if (spawnAtObject)
                {
                    pathHolder.transform.position = AWThingTransform.position;
                }

                pathHolder.name = "PathHolder";
                //pathHolder.transform.parent = null;
            }
            if (!pathCreator)
            {
                pathCreator = pathHolder.AddComponent<PathCreator>();
            }
            if (!pathFollower)
            {
                pathFollower = AWThing.AddComponent<PathFollower>();
                pathFollower.pathCreator = pathCreator;
            }
            else
            {
                if (!pathFollower.pathCreator) pathFollower.pathCreator = pathCreator;
            }

            if (gameObject.GetComponent<Rigidbody>())
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            LoadDefaultPathConfig();
            pathInitialized = true;
        }
    }
    public void LoadDefaultPathConfig()
    {
        if (pathCreator == null) return;


        if (pathCreator.PathConfig == null && loadDefaultPath)
        {
            var defaultPath = Resources.Load<PathConfig>("PathConfig/Default");
            if (defaultPath != null)
            {

                pathCreator.PathConfig = defaultPath;
                pathCreator.LoadPath();
            }
            else
            {
                Debug.LogError("No default PathConfig found at path");
            }
        }
    }
    public override void RemoveAWAnimator()
    {
        base.RemoveAWAnimator();
        if (pathInitialized)
        {
            if (pathFollower!=null)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(pathFollower);
            }
            if (pathCreator != null)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(pathHolder);
            }
        }
    }
    #endregion

    #region Behaviour Methods
    public void RemoveSplinePath()
    {
        AnythingSafeDestroy.SafeDestroyDelayed(pathHolder);
    }

    #endregion

}
