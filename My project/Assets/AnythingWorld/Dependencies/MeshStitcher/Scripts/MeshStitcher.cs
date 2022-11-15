using UnityEngine;

namespace MeshStitcher
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MeshStitcher : MonoBehaviour
    {
        #region Fields
        public MaterialPropertyBlock propertyBlock = null;
        public Transform attachedToTransform = null;
        public MeshFilter sourceMeshFilter = null;
        public MeshFilter targetMeshFilter = null;
        public MeshRenderer sourceRenderer = null;
        public static Shader StitchShader { get; set; } = null;

        public Shader overrideShader { get; set; } = null;
        public Material overrideMaterial { get; set; } = null;
        [SerializeField]
        public int StitchMatrixID = Shader.PropertyToID("TargetObjectTransform");
        [SerializeField]
        public int BaseMapID = Shader.PropertyToID("_BaseMap");
        //public int BaseMaterial = Shader.PropertyToID("BaseMaterial");
        public const string RenderTypeMaterialTag = "RenderType";
        public const float BindingVerticesTolerance = 0.01f;
        #endregion

        #region Unity Callbacks
        public void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            if (StitchShader == null)
                StitchShader = Shader.Find("Anything World/Simple Lit Stitched");
            //StitchShader = Shader.Find("Anything World/Simple Lit");
        }
        [RuntimeInitializeOnLoadMethod]
        private void ResetSourceRenderer()
        {
            propertyBlock = new MaterialPropertyBlock();
            if (StitchShader == null)
                StitchShader = Shader.Find("Anything World/Simple Lit Stitched");

            if (sourceRenderer)
            {
                if (propertyBlock != null && sourceRenderer != null)
                {
                    propertyBlock.SetMatrix(StitchMatrixID, attachedToTransform.localToWorldMatrix);
                    sourceRenderer.SetPropertyBlock(propertyBlock);
                }

            }

        }
        public void Update()
        {
            if (sourceRenderer)
            {
                if (propertyBlock != null && sourceRenderer != null)
                {
                    propertyBlock.SetMatrix(StitchMatrixID, attachedToTransform.localToWorldMatrix);
                    sourceRenderer.SetPropertyBlock(propertyBlock);
                }

            }
        }
        public void FixedUpdate()
        {

            if (sourceRenderer)
            {
                if (propertyBlock != null && sourceRenderer != null)
                {
                    propertyBlock.SetMatrix(StitchMatrixID, attachedToTransform.localToWorldMatrix);
                    sourceRenderer.SetPropertyBlock(propertyBlock);
                }

            }

        }

        public void LateUpdate()
        {
            if (sourceRenderer)
            {
                if (propertyBlock != null && sourceRenderer != null)
                {
                    propertyBlock.SetMatrix(StitchMatrixID, attachedToTransform.localToWorldMatrix);
                    sourceRenderer.SetPropertyBlock(propertyBlock);
                }

            }
          
        }
        #endregion

        #region Public Methods
        public void Initialize(MeshFilter sourceFilter, MeshFilter targetFilter, bool stitchModel = true)
        {
            if (sourceFilter == null)
            {
                Debug.LogError("Source mesh filter missing", this);
                return;
            }
            else if (targetFilter == null)
            {
                Debug.LogError("target mesh filter missing", this);
                return;
            }
            sourceMeshFilter = sourceFilter;
            targetMeshFilter = targetFilter;
            sourceMeshFilter.mesh.RecalculateNormals();
            targetMeshFilter.mesh.RecalculateNormals();
            sourceMeshFilter.mesh.RecalculateTangents();
            targetMeshFilter.mesh.RecalculateTangents();
            targetMeshFilter.mesh.Optimize();
            sourceMeshFilter.mesh.Optimize();
            var sourceMesh = sourceFilter.sharedMesh;
            var sourceVertices = sourceMesh.vertices;
            var stitchedVertices = targetFilter.sharedMesh.vertices;

            sourceRenderer = sourceFilter.GetComponent<MeshRenderer>();


            if (sourceRenderer == null)
            {
                Debug.LogError("Filter is present but MeshRenderer is missing.", sourceFilter);
                return;
            }



            if (overrideMaterial != null)
            {
                var tex = sourceRenderer.material.GetTexture(BaseMapID);
                sourceRenderer.material = overrideMaterial;
                sourceRenderer.material.SetTexture(BaseMapID, tex);
            }
            else
            {
                var materials = sourceRenderer.sharedMaterials;
                foreach (var material in materials)
                    InitializeMaterial(material, overrideShader);
                sourceRenderer.materials = materials;
            }

            attachedToTransform = targetFilter.transform;
            var isStitched = new Vector2[sourceMesh.vertexCount];
            if (stitchModel)
            {
                for (var i = 0; i < sourceVertices.Length; i++)
                {
                    for (var j = 0; j < stitchedVertices.Length; j++)
                    {
                        if (Vector3.Distance(sourceVertices[i], stitchedVertices[j]) <= BindingVerticesTolerance)
                        {
                            isStitched[i] = Vector2.one;
                        }
                    }
                }
            }


            sourceMesh.uv3 = isStitched;
            sourceMesh.UploadMeshData(false);

        }
        #endregion

        #region Private Methods
        private static void InitializeMaterial(Material material, Shader instanceShader = null)
        {
            var renderTypeTag = material.GetTag(RenderTypeMaterialTag, false);
            var renderQueue = material.renderQueue;
            if (instanceShader != null)
            {
                material.shader = instanceShader;
                material.renderQueue = 3000;
            }
            else
            {
                material.shader = StitchShader;
                material.renderQueue = renderQueue;
            }

            // materials may loose tags and shader queue values when changing shaders
            // so we need to copy those explicitly
            if (!string.IsNullOrEmpty(renderTypeTag))
                material.SetOverrideTag(RenderTypeMaterialTag, renderTypeTag);

        }
        #endregion
    }
}

