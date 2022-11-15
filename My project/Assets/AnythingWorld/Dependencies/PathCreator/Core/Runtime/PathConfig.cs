using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

/// <summary>
/// Holds data representing a bezier curve to be loaded into SplineCreator.
/// </summary>
[CreateAssetMenu(fileName = "PathConfig", menuName = "AnythingWorld/PathConfig", order = 4)]
public class PathConfig : ScriptableObject
{
    #region Fields
    /// <summary>
    /// Complete list of anchor & control points from path.
    /// </summary>
    public List<Vector3> points;
    /// <summary>
    /// List of anchor points from path.
    /// </summary>
    public List<Vector3> anchorPoints;
    /// <summary>
    /// List of control points from path.
    /// </summary>
    public List<Vector3> controlPoints;
    /// <summary>
    /// Is path a closed loop path.
    /// </summary>
    public bool isClosed;
    /// <summary>
    /// Is path in 2D or 3D space.
    /// </summary>
    public PathSpace pathspace;
    /// <summary>
    /// Global normal for all anchors.
    /// </summary>
    public float globalNormal;
    /// <summary>
    /// List of per anchor normals for path.
    /// </summary>
    public List<float> perAnchorNormals;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initialize path config.
    /// </summary>
    /// <param name="_points">Anchor & control points.</param>
    /// <param name="_anchors">Anchor points.</param>
    /// <param name="_controls">Control points.</param>
    /// <param name="_isclosed">Is path closed loop.</param>
    /// <param name="_pathSpace">2D/3D space of path.</param>
    /// <param name="_globalNormal">Global normal for anchors.</param>
    /// <param name="_perAnchorNormals">Per anchor normal list.</param>
    public void Initialize(List<Vector3> _points, List<Vector3> _anchors, List<Vector3> _controls, bool _isclosed, PathSpace _pathSpace, float _globalNormal, List<float> _perAnchorNormals)
    {
        points = new List<Vector3>(_points);
        anchorPoints = new List<Vector3>(_anchors);
        controlPoints = new List<Vector3>(_controls);
        isClosed = _isclosed;
        pathspace = _pathSpace;
        globalNormal = _globalNormal;
        perAnchorNormals = new List<float>(_perAnchorNormals);
    }
    #endregion

}



