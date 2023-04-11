using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script generates a celestial object (planet, moon, or star) using an icosphere.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CelestialObjectGenerator : MonoBehaviour
{
    private Mesh mesh; // The mesh of the celestial object

    /// <summary>
    /// Generates a celestial object with the given vertices and triangles.
    /// </summary>
    /// <param name="vertices">The vertices of the icosphere.</param>
    /// <param name="triangles">The triangles of the icosphere.</param>
    public CelestialObjectGenerator(List<Vector3> vertices, List<int> triangles)
    {
        mesh = new Mesh(); // Create a new mesh

        // Assign the icosphere's vertices and triangles to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        // Recalculate the mesh normals and bounds for proper shading and rendering
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    /// <summary>
    /// Generates a celestial object GameObject.
    /// </summary>
    /// <returns>The generated celestial object GameObject.</returns>
    public GameObject GenerateObject()
    {
        GameObject icoSpherePrefab = Resources.Load<GameObject>("Prefabs/IcoSphere");

        if (icoSpherePrefab == null)
        {
            Debug.LogError("Failed to load IcoSphere prefab from Resources/Prefabs directory!");
        }
        else
        {
            GameObject obj = Instantiate(icoSpherePrefab);
            obj.name = "Celestial Object";

            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            return obj;
        }

        return null;
    }
}
