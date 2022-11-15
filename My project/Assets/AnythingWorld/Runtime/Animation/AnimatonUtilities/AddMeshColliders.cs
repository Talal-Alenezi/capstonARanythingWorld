using AnythingWorld.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnythingWorld.Animation
{
    public class AddMeshColliders : MonoBehaviour
    {
        public float scaleParameter = 0.7f;
        public bool setCollidersAsTriggers = true;
        private AWObj _controllingAWObj;

        // Dictionary of the colliders created for the model
        private Dictionary<string, Collider> modelColliders = new Dictionary<string, Collider>();

        // Field used for synchronisation with the feet movement script
        public bool feetSetupComplete { get; set; } = false;

        private Dictionary<string, string> renaming = new Dictionary<string, string>() { { "head", "head_holder" }, { "tail", "tail_holder" }, };

        [SerializeField]
        private bool collidersSet = false;

        private void Start()
        {
            if (transform.parent != null)
            {
                _controllingAWObj = transform.parent.GetComponent<AWObj>();
                if (_controllingAWObj == null)
                {
                    Debug.LogError($"No AWObj found for {gameObject.name}");
                    return;
                }
            }

            if (!collidersSet)
            {
                StartCoroutine(WaitForAWObjCompletion());
                collidersSet = true;
            }
            else
            {
                feetSetupComplete = true;
            }
        }

        private IEnumerator WaitForAWObjCompletion()
        {
            if (_controllingAWObj != null)
            {
                while (!_controllingAWObj.ObjMade)
                    yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }

            // Create colliders for the model
            UpdateMeshColliders();

            // Setup colliders for model's feet and hands
            FootSetup();
            //HandsSetup();
            feetSetupComplete = true;

            // Set up model's joints
            SetupJoints();
            if (setCollidersAsTriggers) SetCollidersToTriggers();
        }

        private void SetCollidersToTriggers()
        {
            if (createdColliders != null)
            {
                //Debug.Log($"Setting {createdColliders.Count} colliders to triggers");
                foreach (var collider in createdColliders)
                {
                    collider.isTrigger = true;
                }
            }
        }

        private List<BoxCollider> createdColliders;

        public void UpdateMeshColliders()
        {
            // Collect bounds of all the meshes that construct the object
            var childrenMeshes = transform.GetComponentsInChildren<MeshFilter>();

            createdColliders = new List<BoxCollider>();
            for (var i = 0; i < childrenMeshes.Length; i++)
            {
                var meshObject = childrenMeshes[i].gameObject;
                var meshObjectScale = meshObject.transform.localScale;
                // Find the parent of the mesh to obtain the scale
                var parent = ((MeshFilter)childrenMeshes.GetValue(i)).GetComponentInParent<Transform>().parent;
                if (parent.name == "MeshPivot")
                    continue;
                var prefabParent = parent.parent; // Find the parent that is the prefab element to which the collider is to be added

                // Do not create colliders for wheels
                if (prefabParent.name.IndexOf("wheel") != -1)
                    continue;

                BoxCollider modelCollider;
                if (renaming.ContainsKey(prefabParent.name) && transform.Find(renaming[prefabParent.name]) != null)
                    prefabParent = transform.Find(renaming[prefabParent.name]);


                if (prefabParent.gameObject.GetComponent<BoxCollider>())
                {
                    //Find Existing Collider
                    modelCollider = prefabParent.gameObject.GetComponent<BoxCollider>();
                }
                else
                {
                    // Create collider for the transform
                    modelCollider = prefabParent.gameObject.AddComponent<BoxCollider>();
                    createdColliders.Add(modelCollider);
                    modelColliders.Add(prefabParent.name, modelCollider);
                }


                // Calculate scaled mesh size and center
                var scaledSize = Vector3.Scale(new Vector3(scaleParameter, scaleParameter, scaleParameter), Vector3.Scale(((MeshFilter)childrenMeshes.GetValue(i)).sharedMesh.bounds.size, parent.localScale)); //colliders slighthly scaled down so that they do not push each other away
                var scaledCenter = Vector3.Scale(((MeshFilter)childrenMeshes.GetValue(i)).sharedMesh.bounds.center, parent.localScale);
                scaledCenter = Vector3.Scale(scaledCenter, meshObjectScale);

          //Set size and center;
                modelCollider.size = scaledSize;
                modelCollider.center = scaledCenter + parent.localPosition;
               
            }
        }

        private void SetupJoints()
        {
            // Collect joints of all the parts that construct the object
            var childrenJoints = transform.GetComponentsInChildren<Joint>();

            for (var i = 0; i < childrenJoints.Length; i++)
            {
                // Get joints anchor and modify its location
                var joint = (Joint)childrenJoints.GetValue(i);
                if (modelColliders.ContainsKey(joint.transform.name))
                {
                    try
                    {
                        var inverseScaleParameter = 1 / scaleParameter;
                        joint.anchor = ((BoxCollider)modelColliders[joint.transform.name]).center +
                            Vector3.Scale(Vector3.Scale(new Vector3(inverseScaleParameter, inverseScaleParameter, inverseScaleParameter), ((BoxCollider)modelColliders[joint.transform.name]).size), joint.anchor);
                    }
                    catch
                    {
                        if (joint.transform.childCount == 0)
                            Destroy(joint);
                    }
                }
                else
                {
                    if (joint.transform.childCount == 0)
                        Destroy(joint);
                }
            }
        }

        private void FootSetup()
        {
            // Check if feet colliders have been already created
            var feetcolliders = modelColliders.Where(kvp => kvp.Key.IndexOf("foot") != -1 || kvp.Key.IndexOf("feet") != -1);
            if (feetcolliders.ToList().Count != 0)
                return;

            // Create feet colliders in case they where not created earlier
            for (var i = 0; i < transform.childCount; i++)
            {
                // Find feet transform
                if (transform.GetChild(i).name.IndexOf("foot") != -1 || transform.GetChild(i).name.IndexOf("feet") != -1)
                {
                    // Find which feet it is
                    var feetSide = transform.GetChild(i).name.Substring(5);

                    // Get bottom part of the leg
                    var legColliders = modelColliders.Where(kvp => kvp.Key.IndexOf(feetSide) != -1 && kvp.Key.IndexOf("bot") != -1 && kvp.Key.IndexOf("leg") != -1);
                    if (legColliders.ToList().Count == 0)
                        legColliders = modelColliders.Where(kvp => kvp.Key.IndexOf(feetSide) != -1 && kvp.Key.IndexOf("leg") != -1);

                    try
                    {
                        var legCollider = legColliders.First();
                        // Specify the position of the feet
                        var position = ((BoxCollider)legCollider.Value).center;
                        position.y -= (1 / scaleParameter) * ((BoxCollider)legCollider.Value).size.y;

                        // Create collider for the transform
                        var modelCollider = transform.GetChild(i).gameObject.AddComponent<SphereCollider>();
                        modelCollider.radius = 0.15f;

                        modelColliders.Add(transform.GetChild(i).name, modelCollider);

                        //Change position of the feet transform
                        transform.GetChild(i).localPosition = position;
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        private void HandsSetup()
        {
            // Check if hand colliders have been already created
            var handcolliders = modelColliders.Where(kvp => kvp.Key.IndexOf("hand") != -1);
            if (handcolliders.ToList().Count != 0)
                return;

            // Create hand colliders in case they where not created earlier
            for (var i = 0; i < transform.childCount; i++)
            {
                // Find hand transform
                if (transform.GetChild(i).name.IndexOf("hand") != -1)
                {
                    // Find which hand it is
                    var handSide = transform.GetChild(i).name.Substring(5);

                    // Get bottom part of the arm
                    var armColliders = modelColliders.Where(kvp => kvp.Key.IndexOf(handSide) != -1 && kvp.Key.IndexOf("bot") != -1 && kvp.Key.IndexOf("arm") != -1);
                    if (armColliders.ToList().Count == 0)
                        armColliders = modelColliders.Where(kvp => kvp.Key.IndexOf(handSide) != -1 && kvp.Key.IndexOf("arm") != -1);

                    var armCollider = armColliders.First();

                    // Specify the position of the hand
                    var position = ((BoxCollider)armCollider.Value).center;
                    position.y -= (1 / scaleParameter) * ((BoxCollider)armCollider.Value).size.y;

                    // Create collider for the transform
                    var modelCollider = transform.GetChild(i).gameObject.AddComponent<SphereCollider>();
                    modelCollider.radius = 0.15f;

                    modelColliders.Add(transform.GetChild(i).name, modelCollider);

                    //Change position of the hand transform
                    transform.GetChild(i).localPosition = position;
                }
            }
        }

        public void RemoveColliders()
        {
            foreach (var collider in modelColliders.Keys)
            {
                AnythingSafeDestroy.SafeDestroyDelayed(modelColliders[collider]);
            }

            modelColliders.Clear();
        }

        private void OnDestroy()
        {
            RemoveColliders();
        }
    }
}