using System.Collections.Generic;
using UnityEngine;


namespace PathCreation {
    [System.Serializable]
    public class PathCreator : MonoBehaviour {

        /// This class stores data for the path editor, and provides accessors to get the current vertex and bezier path.
        /// Attach to a GameObject to create a new path editor.

        public event System.Action pathUpdated;

        //[SerializeField, HideInInspector]
        [SerializeField]
        PathCreatorData editorData;
        [SerializeField, HideInInspector]
        bool initialized;
        [SerializeField]
        public GameObject spawner;
        GlobalDisplaySettings globalEditorDisplaySettings;

        // Vertex path created from the current bezier path
        public VertexPath path {
            get {
                //If the editorData is not initialized initialize it.
                if (!initialized) {
                    InitializeEditorData (false);
                }
                return editorData.GetVertexPath(transform);
            }
        }

        // The bezier path created in the editor
        [SerializeField]
        public BezierPath bezierPath
        {
            get
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                return editorData.BezierPath;
            }
            set
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                editorData.BezierPath = value;
            }
        }


        [SerializeField]
        public PathConfig PathConfig
        {
            get
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                return editorData.PathConfig;
            }
            set
            {
                if (!initialized)
                {
                    InitializeEditorData(false);
                }
                editorData.PathConfig = value;
                //editorData.LoadBezierFromConfig(editorData.PathConfig, transform);
            }
        }
        #region Internal methods

        /// Used by the path editor to initialise some data
        public void InitializeEditorData (bool in2DMode) {
            if (editorData == null) {
                editorData = new PathCreatorData ();
            }
            editorData.bezierOrVertexPathModified -= TriggerPathUpdate;
            editorData.bezierOrVertexPathModified += TriggerPathUpdate;
            editorData.Initialize (in2DMode);
            initialized = true;
        }

        public PathCreatorData EditorData {
            get 
            {
                return editorData;
            }
        }

        public void TriggerPathUpdate () {
            if (pathUpdated != null) {
                pathUpdated ();
            }
        }

        public PathConfig CreateNewConfigFromPath()
        {
            
            PathConfig = ScriptableObject.CreateInstance<PathConfig>();
            PathConfig.Initialize(bezierPath.points,
                    bezierPath.GetAnchorPointList(),
                    bezierPath.GetControlPointList(),
                    bezierPath.isClosed,
                    bezierPath.space,
                    bezierPath.GlobalNormalsAngle,
                    bezierPath.PerAnchorNormalsAngle);
            return PathConfig;

        }
        public void SavePath()
        {
            if (PathConfig)
            {
                PathConfig.Initialize(bezierPath.points, 
                    bezierPath.GetAnchorPointList(), 
                    bezierPath.GetControlPointList(), 
                    bezierPath.isClosed, 
                    bezierPath.space, 
                    bezierPath.GlobalNormalsAngle, 
                    bezierPath.PerAnchorNormalsAngle);
                Debug.Log(PathConfig);
            }
            
        }
        public void LoadPath()
        {
            if (PathConfig)
            {
                initialized = false;
                editorData.LoadBezierFromConfig(PathConfig, transform);
            }
            else
            {
                Debug.Log("Error: No config attached");
            }
        }
        public void LoadPath(PathConfig _pathConfig)
        {
            PathConfig = _pathConfig;
            LoadPath();
            Debug.Log(editorData);
        }
        public void LoadPath(float _pathScalar)
        {

            if (PathConfig)
            {
                editorData.LoadBezierFromConfig(PathConfig, _pathScalar);
                //editorData.CreateVertexPath(transform);
                //InitializeEditorData(false);
                //Debug.Log(pathConfig);
            }
            else
            {
                Debug.Log("Error: No config attached");
            }

        }

#if UNITY_EDITOR

        // Draw the path when path objected is not selected (if enabled in settings)
        void OnDrawGizmos () {

            // Only draw path gizmo if the path object is not selected
            // (editor script is resposible for drawing when selected)
            GameObject selectedObj = UnityEditor.Selection.activeGameObject;
            if (selectedObj != gameObject) {

                if (path != null) {
                    path.UpdateTransform (transform);

                    if (globalEditorDisplaySettings == null) {
                        globalEditorDisplaySettings = GlobalDisplaySettings.Load ();
                    }

                    if (globalEditorDisplaySettings.visibleWhenNotSelected) {

                        Gizmos.color = globalEditorDisplaySettings.bezierPath;

                        for (int i = 0; i < path.NumPoints; i++) {
                            int nextI = i + 1;
                            if (nextI >= path.NumPoints) {
                                if (path.isClosedLoop) {
                                    nextI %= path.NumPoints;
                                } else {
                                    break;
                                }
                            }
                            Gizmos.DrawLine (path.GetPoint (i), path.GetPoint (nextI));
                        }
                    }
                }
            }
        }
#endif

        #endregion
    }
}