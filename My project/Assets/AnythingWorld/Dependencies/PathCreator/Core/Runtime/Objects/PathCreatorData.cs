using System.Collections.Generic;
using UnityEngine;

namespace PathCreation {
    /// Stores state data for the path creator editor

    [System.Serializable]
    public class PathCreatorData {
        public event System.Action bezierOrVertexPathModified;
        public event System.Action bezierCreated;

        [SerializeField]
        BezierPath _bezierPath;
        [SerializeField]
        VertexPath _vertexPath;

        [SerializeField]
        bool vertexPathUpToDate;

        [SerializeField]
        PathConfig _pathConfig;

        //Config Path Settings
        public string savedPathID = "";

        // vertex path settings
        public float vertexPathMaxAngleError = .3f;
        public float vertexPathMinVertexSpacing = 0.01f;

        // bezier display settings
        public bool showTransformTool = true;
        public bool showPathBounds;
        public bool showPerSegmentBounds;
        public bool displayAnchorPoints = true;
        public bool displayControlPoints = true;
        public float bezierHandleScale = 1;
        public bool globalDisplaySettingsFoldout;
        public bool keepConstantHandleSize = true;

        // vertex display settings
        public bool showNormalsInVertexMode;
        public bool showBezierPathInVertexMode;

        // Editor display states
        public bool showDisplayOptions;
        public bool showPathOptions = true;
        public bool showVertexPathDisplayOptions;
        public bool showVertexPathOptions = true;
        public bool showNormals;
        public bool showNormalsHelpInfo;
        public int tabIndex;
        public bool showLoadPathOptions = true;
        public bool showScalePathTool = false;
        public bool showSavePathOptions = true;

        public void Initialize(bool defaultIs2D) {
            if (_bezierPath == null) {
                CreateBezier(Vector3.zero, defaultIs2D);
            }
            vertexPathUpToDate = false;
            _bezierPath.OnModified -= BezierPathEdited;
            _bezierPath.OnModified += BezierPathEdited;
        }

        public void ResetBezierPath(Vector3 centre, bool defaultIs2D = false) {
            CreateBezier(centre, defaultIs2D);
        }

        void CreateBezier(Vector3 centre, bool defaultIs2D = false) {
            if (_bezierPath != null) {
                _bezierPath.OnModified -= BezierPathEdited;
            }

            var space = (defaultIs2D) ? PathSpace.xy : PathSpace.xyz;
            _bezierPath = new BezierPath(centre, false, space);

            _bezierPath.OnModified += BezierPathEdited;
            vertexPathUpToDate = false;

            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified();
            }
            if (bezierCreated != null) {
                bezierCreated();
            }
        }

        public BezierPath BezierPath {
            get {
                return _bezierPath;
            }
            set {
                _bezierPath.OnModified -= BezierPathEdited;
                vertexPathUpToDate = false;
                _bezierPath = value;
                _bezierPath.OnModified += BezierPathEdited;

                if (bezierOrVertexPathModified != null) {
                    bezierOrVertexPathModified();
                }
                if (bezierCreated != null) {
                    bezierCreated();
                }
                BezierPathEdited();
            }
        }

        public PathConfig PathConfig
        {
            get
            {
                return _pathConfig;
            }
            set
            {
                if (value != null)
                {
                    _pathConfig = value;
                }

            }
        }
        // Get the current vertex path
        public VertexPath GetVertexPath(Transform transform)
        {
            // create new vertex path if path was modified since this vertex path was created
            if (!vertexPathUpToDate || _vertexPath == null)
            {
                vertexPathUpToDate = true;
                _vertexPath = new VertexPath(BezierPath, transform, vertexPathMaxAngleError, vertexPathMinVertexSpacing);
            }
            return _vertexPath;
        }


        public void LoadBezierFromConfig(PathConfig _pathConfig, Transform _transform)
        {
            if (_pathConfig != null)
            {
                BezierPath = new BezierPath(_pathConfig.anchorPoints, _pathConfig.isClosed, _pathConfig.pathspace);
                //bezierPath.PerAnchorNormalsAngle = _pathConfig.perAnchorNormals;
                BezierPath.GlobalNormalsAngle = _pathConfig.globalNormal;
                BezierPath.LoadAnchorNormalsFromList(_pathConfig.perAnchorNormals);
                BezierPath.SetControlPoints(_pathConfig.points);

                //debug
                _vertexPath = GetVertexPath(_transform);
                if (bezierCreated != null)
                {
                    bezierCreated();
                }
                
            }
           
        }
        public void LoadBezierFromConfig(PathConfig _pathConfig, float _scalar)
        {
            if (_bezierPath != null)
            {
                _bezierPath.OnModified -= BezierPathEdited;
            }
            List<Vector3> scaledPoints = _pathConfig.points;
            for (int i = 0; i < scaledPoints.Count; i++)
            {
                scaledPoints[i] *= _scalar;
            }
            BezierPath = new BezierPath(scaledPoints, _pathConfig.isClosed, _pathConfig.pathspace);
            BezierPath.PerAnchorNormalsAngle = _pathConfig.perAnchorNormals;
            BezierPath.GlobalNormalsAngle = _pathConfig.globalNormal;


            _bezierPath.OnModified += BezierPathEdited;
            vertexPathUpToDate = false;

            if (bezierOrVertexPathModified != null)
            {
                bezierOrVertexPathModified();
            }
            if (bezierCreated != null)
            {
                bezierCreated();
            }
        }
        public VertexPath CreateVertexPath(Transform transform)
        {
            vertexPathUpToDate = true;
            _vertexPath = new VertexPath(BezierPath, transform, vertexPathMaxAngleError, vertexPathMinVertexSpacing);
            Debug.Log("Vertex created and assigned to editor data");
            return _vertexPath;
            
        }
        public void PathTransformed () {
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }
        public void VertexPathSettingsChanged () {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }
        public void PathModifiedByUndo () {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }
        void BezierPathEdited () {
            vertexPathUpToDate = false;
            if (bezierOrVertexPathModified != null) {
                bezierOrVertexPathModified ();
            }
        }

    }
}