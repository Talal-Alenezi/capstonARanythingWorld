using UnityEngine;
/// <summary>
/// 
/// </summary>
public class ZonePlacement : MonoBehaviour
{
    #region Fields
    private float dimensionX;
    private float dimensionY;
    private float targetScale;
    private static ZonePlacement instance;
    #endregion

    #region Public Methods
    public static ZonePlacement Instance
    {
        get
        {
            if (instance == null)
            {
                var zonePlacementGO = new GameObject();
                zonePlacementGO.name = "Anything Zone Placement";
                var zonePlacement = zonePlacementGO.AddComponent<ZonePlacement>();
                instance = zonePlacement;
            }
            return instance;
        }
    }
    public Vector3 GetZoneObjectPosition(string zoneName)
    {
        var zoneTag = GetZoneTag(zoneName);

        var targTrans = GameObject.FindGameObjectWithTag(zoneTag).transform;

        GetTargetObjectBounds(targTrans);

        var randpos = Vector3.zero;
        randpos.x = Random.Range(-dimensionX / 2f, dimensionX / 2f); //assume mesh of the plane is centered, view mesh.bounds.min.x and mesh.bounds.max.x if not centered
        randpos.y = targetScale;//300f; 
        randpos.z = Random.Range(-dimensionY / 2f, dimensionY / 2f);

        var zoneObjPos = targTrans.position + randpos;
        return zoneObjPos;
    }
    public Vector3 GetZoneObjectScale()
    {
        return new Vector3(targetScale, targetScale, targetScale);
    }
    #endregion

    #region Private Methods
    private void GetTargetObjectBounds(Transform targetTrans)
    {
        var _mesh = targetTrans.GetComponent<MeshFilter>().mesh;
        dimensionX = _mesh.bounds.size.x * targetTrans.localScale.x;
        dimensionY = _mesh.bounds.size.z * targetTrans.localScale.z;
    }
    private string GetZoneTag(string zoneName)
    {
        var zoneTag = "AWForeGround";
        switch (zoneName)
        {
            case "background":
                targetScale = 100f;
                zoneTag = "AWBackGround";
                break;
            case "middleground":
                targetScale = 40f;
                zoneTag = "AWMiddleGround";
                break;
            default:
                targetScale = 20f;
                break;
        }
        return zoneTag;
    }
    #endregion
}