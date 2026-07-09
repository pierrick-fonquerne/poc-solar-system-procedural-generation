using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a smooth icosphere of a given radius. Surface relief is applied
/// separately by <see cref="PlanetTerrainGenerator"/> so that every LOD level
/// can share the same deterministic noise field.
/// </summary>
public class IcoSphereGenerator
{
    public List<Vector3> Vertices { get; private set; } // List of vertices for the icosphere
    public List<int> Triangles { get; private set; } // List of triangles for the icosphere

    // Cache for middle points to avoid duplication when subdividing faces
    private Dictionary<long, int> middlePointIndexCache;

    // Constructor for the IcoSphereGenerator
    public IcoSphereGenerator(float radius, int numSubdivisions)
    {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        middlePointIndexCache = new Dictionary<long, int>();

        // Create initial icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        // Add initial vertices for the icosahedron
        AddVertex(new Vector3(-1f, t, 0f));
        AddVertex(new Vector3(1f, t, 0f));
        AddVertex(new Vector3(-1f, -t, 0f));
        AddVertex(new Vector3(1f, -t, 0f));

        AddVertex(new Vector3(0f, -1f, t));
        AddVertex(new Vector3(0f, 1f, t));
        AddVertex(new Vector3(0f, -1f, -t));
        AddVertex(new Vector3(0f, 1f, -t));

        AddVertex(new Vector3(t, 0f, -1f));
        AddVertex(new Vector3(t, 0f, 1f));
        AddVertex(new Vector3(-t, 0f, -1f));
        AddVertex(new Vector3(-t, 0f, 1f));

        // Define initial faces for the icosahedron
        int[][] icoFaces = new int[][]
        {
            new int[] {0, 11, 5},
            new int[] {0, 5, 1},
            new int[] {0, 1, 7},
            new int[] {0, 7, 10},
            new int[] {0, 10, 11},
            new int[] {1, 5, 9},
            new int[] {5, 11, 4},
            new int[] {11, 10, 2},
            new int[] {10, 7, 6},
            new int[] {7, 1, 8},
            new int[] {3, 9, 4},
            new int[] {3, 4, 2},
            new int[] {3, 2, 6},
            new int[] {3, 6, 8},
            new int[] {3, 8, 9},
            new int[] {4, 9, 5},
            new int[] {2, 4, 11},
            new int[] {6, 2, 10},
            new int[] {8, 6, 7},
            new int[] {9, 8, 1}
        };

        // Subdivide each face according to the number of subdivisions
        for (int i = 0; i < icoFaces.Length; i++)
        {
            int[] face = icoFaces[i];
            Subdivide(face[0], face[1], face[2], numSubdivisions);
        }

        // Project every vertex onto the sphere of the desired radius
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vertices[i] = Vertices[i].normalized * radius;
        }
    }

    /// <summary>
    /// Builds a Unity mesh from the generated vertices and triangles.
    /// </summary>
    /// <param name="name">Optional name assigned to the mesh.</param>
    /// <returns>A new mesh with recalculated normals and bounds.</returns>
    public Mesh BuildMesh(string name = "IcoSphere")
    {
        Mesh mesh = new Mesh { name = name };
        mesh.SetVertices(Vertices);
        mesh.SetTriangles(Triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // Add a vertex to the vertices list and return its index
    private int AddVertex(Vector3 vertex)
    {
        int index = Vertices.Count;
        Vertices.Add(vertex);
        return index;
    }

    // Get the index of the middle point between two vertices
    private int GetMiddlePoint(int indexA, int indexB)
    {
        long smallerIndex = Mathf.Min(indexA, indexB);
        long greaterIndex = Mathf.Max(indexA, indexB);
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (middlePointIndexCache.TryGetValue(key, out ret))
        {
            return ret;
        }

        Vector3 point1 = Vertices[indexA];
        Vector3 point2 = Vertices[indexB];
        Vector3 middle = (point1 + point2) / 2f;

        int i = AddVertex(middle);
        middlePointIndexCache.Add(key, i);
        return i;
    }

    // Subdivide a face recursively into smaller faces
    private void Subdivide(int a, int b, int c, int depth)
    {
        if (depth == 0)
        {
            Triangles.Add(a);
            Triangles.Add(b);
            Triangles.Add(c);
        }
        else
        {
            int ab = GetMiddlePoint(a, b);
            int bc = GetMiddlePoint(b, c);
            int ca = GetMiddlePoint(c, a);

            Subdivide(a, ab, ca, depth - 1);
            Subdivide(b, bc, ab, depth - 1);
            Subdivide(c, ca, bc, depth - 1);
            Subdivide(ab, bc, ca, depth - 1);
        }
    }
}
