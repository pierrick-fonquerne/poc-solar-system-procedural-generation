using UnityEngine;

// This script generates a celestial object (planet, moon, or sun) using an icosphere.
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CelestialObjectGenerator : MonoBehaviour
{
    public int numSubdivisions = 4; // Number of subdivisions for the icosphere
    public float radius = 3f; // Radius of the celestial object

    private Mesh mesh; // The mesh of the celestial object
    private IcoSphereGenerator icoSphereGenerator; // The icosphere generator used to create the celestial object

    // Generates the celestial object when the game starts
    private void Awake()
    {
        GenerateCelestialObjec();
    }

    // Generates the celestial object with the specified subdivisions and radius
    public void GenerateCelestialObjec()
    {
        mesh = new Mesh(); // Create a new mesh
        GetComponent<MeshFilter>().mesh = mesh; // Assign the new mesh to the MeshFilter component

        // Create an icosphere generator with the given radius and subdivisions
        icoSphereGenerator = new IcoSphereGenerator(radius, numSubdivisions);
        
        // Assign the icosphere's vertices and triangles to the mesh
        mesh.vertices = icoSphereGenerator.Vertices.ToArray();
        mesh.triangles = icoSphereGenerator.Triangles.ToArray();

        // Recalculate the mesh normals and bounds for proper shading and rendering
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
