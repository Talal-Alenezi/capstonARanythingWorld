using System.Collections.Generic;
using UnityEngine;

public class NavMeshSourceTagParent : MonoBehaviour
{
    [SerializeField]
    public List<NavMeshSourceTag> sourceTag;

    void OnEnable()
    {
        var meshes = GetComponentsInChildren<MeshFilter>();

        foreach (var mesh in meshes)
        {
            sourceTag.Add(mesh.gameObject.AddComponent<NavMeshSourceTag>());
        }


        /*
         *  
        if (meshes != null)
        {
            foreach (var mesh in meshes)
            {
                sourceTag.Add(mesh.gameObject.AddComponent<NavMeshSourceTag>());
            }
        }

        var terrains = GetComponents<Terrain>();
        if (terrains != null)
        {
            foreach (var terrain in terrains)
            {
                sourceTag.Add(terrain.gameObject.AddComponent<NavMeshSourceTag>());
            }
        }
         */

    }
    void OnDisable()
    {




        /*var meshes = GetComponentsInChildren<MeshFilter>();
        if (meshes != null)
        {
            foreach (var mesh in meshes)
            {
                m_Meshes.Remove(mesh);
            }
        }

        var terrains = GetComponents<Terrain>();
        if (terrains != null)
        {
            foreach (var terrain in terrains)
            {
                m_Terrains.Remove(terrain);
            }
        }*/
    }

    public void RemoveSourceTags()
    {
        foreach (var tag in sourceTag)
        {
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(tag);
                };
#endif
            }
            else
            {
                Destroy(tag);

            }
            sourceTag.Remove(tag);
        }

    }
}
