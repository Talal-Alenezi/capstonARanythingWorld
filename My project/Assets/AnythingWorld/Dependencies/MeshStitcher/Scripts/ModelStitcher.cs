using AnythingWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MeshStitcher
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ModelStitcher : MonoBehaviour
    {
        #region Fields
        [Tooltip("Applied to model at runtime, must have StichVerticesNode attached to Position node (object space)")]
        public Shader overrideStitchedShader = null;
        public Shader overrideStitchedTransparentShader = null;
        public Material overrideMaterial = null;
        private AWObj targetAWObj;
        #endregion
        public UnityEvent onFinishStitching;
        private Shader stitchedShaderDefault;
        private Shader stitchedShaderTransparent;
        #region Unity Callbacks
        public void Start()
        {
            if (transform.parent.TryGetComponent<AWObj>(out var awObj))
            {
                if (awObj.ObjMade)
                {
                    Initialize();
                }
                else
                {
                    awObj.onObjectMadeSuccessfullyDelegate += Initialize;
                }
            }
            else
            {
                Initialize();
            }
        }
        public void OnEnable()
        {
            if (overrideStitchedShader == null && overrideMaterial == null)
            {

                stitchedShaderDefault = AnythingSettings.Instance.DefaultStitchedShader;
            }
            else
            {
                stitchedShaderDefault = overrideStitchedShader;
            }
            if (overrideStitchedTransparentShader == null)
            {
                stitchedShaderTransparent = AnythingSettings.Instance.defaultStitchedTransparentShader;
            }
            else
            {
                stitchedShaderTransparent = overrideStitchedTransparentShader;
            }
        }
        #endregion

        public void Initialize()
        {
            StitchAllJoints();
            SetAllUnstitched();
            onFinishStitching.Invoke();
        }

        private void StitchAllJoints()
        {
            var joints = GetComponentsInChildren<Joint>(true);
            //Adds mesh stitcher to main body game object and stitches it to itself, no change in position.
            var bodyGO = gameObject.transform.GetChild(0).gameObject;
            // if no meshfilter on first child, search for body instead
            // TODO: standardise this, seems a little risky to me - GM
            if (bodyGO.GetComponentInChildren<MeshFilter>() == null)
            {
                bodyGO = gameObject.transform.Find("body").gameObject;
            }
            if (bodyGO != null && bodyGO.GetComponentInChildren<MeshFilter>() != null)
            {
                InitializeMeshStitcher(bodyGO, bodyGO.GetComponentInChildren<MeshFilter>(), bodyGO.GetComponentInChildren<MeshFilter>(), stitchedShaderDefault, overrideMaterial, false);
            }
            else
            {
                Debug.LogWarning("Initialization of mesh stitcher for body failed");
                return;
            }
            //Iterates through meshes with joints and adds a mesh stitcher to each.
            foreach (var joint in joints)
            {
                if (joint.gameObject.tag != "AWStitcherDisable")
                {
                    var connectedTo = joint.connectedBody;
                    if (connectedTo != null)
                    {
                        var stitchToThisMeshFilter = connectedTo.GetComponentInChildren<MeshFilter>(true);
                        var addMeshStitcherTo = joint.gameObject.GetComponentInChildren<MeshFilter>(true);
                        if (stitchToThisMeshFilter != null && addMeshStitcherTo != null)
                        {
                            InitializeMeshStitcher(addMeshStitcherTo.gameObject, addMeshStitcherTo, stitchToThisMeshFilter, stitchedShaderDefault, overrideMaterial);
                        }
                    }
                    else
                    {
                        if(AnythingSettings.DebugEnabled) Debug.LogWarning($"Joint {joint.gameObject.name} is not connected to anything, default trying to connect to body.");
                        if (bodyGO.TryGetComponent<Rigidbody>(out var rigidOut))
                        {
                            try
                            {
                                joint.connectedBody = rigidOut;
                                connectedTo = rigidOut;
                                var stitchToThisMeshFilter = connectedTo.GetComponentInChildren<MeshFilter>(true);
                                var addMeshStitcherTo = joint.gameObject.GetComponentInChildren<MeshFilter>(true);
                                if (stitchToThisMeshFilter != null && addMeshStitcherTo != null)
                                {
                                    InitializeMeshStitcher(addMeshStitcherTo.gameObject, addMeshStitcherTo, stitchToThisMeshFilter, stitchedShaderDefault, overrideMaterial);
                                }
                                else
                                {
                                    Debug.LogWarning("Missing mesh filter in child components");
                                }
                            }
                            catch
                            {
                                Debug.LogError($"Failed to stitch {joint.gameObject.name} to body");
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        var meshRenderer = joint.gameObject.GetComponentInChildren<MeshRenderer>(true);

                        var rendererMaterials = meshRenderer.sharedMaterials;

                        if (AnythingSettings.Instance.defaultUnstitchedShader != null)
                        {
                            var materialOverrideList = new List<Material>();
                            foreach (var item in rendererMaterials.Select((value, i) => new { i, value }))
                            {
                                var mat = item.value;
                                var index = item.i;
                                var newMat = new Material(AnythingSettings.Instance.defaultUnstitchedShader);
                                newMat.CopyPropertiesFromMaterial(mat);
                                newMat.name = $"{mat} - Shader Override";
                                materialOverrideList.Add(newMat);
                            }
                            meshRenderer.materials = materialOverrideList.ToArray();
                        }
                        else
                        {
                            if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning("No default unstitched shader found");
                        }
                    }
                    catch
                    {
                        if (AnythingSettings.Instance.showDebugMessages) Debug.LogError("Error switching meshes to Simple Lit shader");
                    }
                }
            }
        }
        private void SetAllUnstitched()
        {
            var unstitchedPartTags = GetComponentsInChildren<UnstitchedPartTag>();
            foreach (var partTag in unstitchedPartTags)
            {
                if (partTag.GetComponentInChildren<MeshRenderer>())
                {
                    foreach (var component in partTag.GetComponentsInChildren<MeshRenderer>(true))
                    {
                        var unstitchedMeshRenderer = component;

                        //Debug.Log($"Found unstitched MeshRenderer on {partTag.gameObject.name}");

                        try
                        {

                            var rendererMaterials = unstitchedMeshRenderer.sharedMaterials;
                            var defaultShader = AnythingSettings.Instance.DefaultUnstitchedShader;
                            if (defaultShader != null)
                            {
                                var materialOverrideList = new List<Material>();
                                foreach (var item in rendererMaterials.Select((value, i) => new { i, value }))
                                {
                                    var mat = item.value;
                                    var index = item.i;

                                    var newMat = new Material(defaultShader);
                                    newMat.CopyPropertiesFromMaterial(mat);
                                    newMat.name = $"{mat.name} - Shader Override";
                                    materialOverrideList.Add(newMat);
                                }

                                unstitchedMeshRenderer.materials = materialOverrideList.ToArray();
                            }
                            else
                            {
                                if (AnythingSettings.Instance.showDebugMessages) Debug.LogWarning("No simpleLit shader found");
                            }
                        }
                        catch
                        {
                            if (AnythingSettings.Instance.showDebugMessages) Debug.LogError("Error switching meshes to Simple Lit shader");
                        }
                    }
                }
            }
        }

        private static void InitializeMeshStitcher(GameObject mesh, MeshFilter source, MeshFilter target, Shader overrideShader = null, Material overrideMaterial = null, bool stitchComponent = true)
        {
            var newMeshStitcher = mesh.GetComponent<MeshStitcher>();
            if (newMeshStitcher == null)
            {
                newMeshStitcher = mesh.AddComponent<MeshStitcher>();
            }
            if (overrideShader)
            {
                newMeshStitcher.overrideShader = overrideShader;
            }
            if (overrideMaterial)
            {
                newMeshStitcher.overrideMaterial = overrideMaterial;
            }
            newMeshStitcher.Initialize(source, target, stitchComponent);
        }

    }
}
