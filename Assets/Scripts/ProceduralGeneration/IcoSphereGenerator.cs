using System.Collections.Generic;
using UnityEngine;

public class IcoSphereGenerator
{
    public List<Vector3> Vertices { get; private set; } // List of vertices for the icosphere
    public List<int> Triangles { get; private set; } // List of triangles for the icosphere


    // Cache for middle points to avoid duplication when subdividing faces
    private Dictionary<long, int> middlePointIndexCache;
    // Scale factor for erosion
    private float erosionScale = 1f;
    // Strength of the erosion
    private float erosionStrength = 5f;
    // Number of layers of Perlin noise to combine for more complex erosion
    private int erosionLayers = 4;

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

        // Scale each vertex by the desired radius
        for (int i = 0; i < Vertices.Count; i++)
        {
            Vertices[i] = Vertices[i].normalized * radius;
        }
    }

    // Add a vertex to the vertices list and return its index
    private int AddVertex(Vector3 vertex)
    {
        int index = Vertices.Count;

        // Calculate the combined elevation from multiple layers of Perlin noise
        float elevation = 0;
        float frequency = 1;
        float amplitude = 1;
        for (int i = 0; i < erosionLayers; i++)
        {
            elevation += Mathf.PerlinNoise(vertex.x * erosionScale * frequency + 0.5f, vertex.y * erosionScale * frequency + 0.5f) * amplitude;
            frequency *= 2;
            amplitude *= erosionStrength;
        }

        // Apply the erosion to the vertex and add it to the list
        float finalElevation = 1f + elevation * 0.1f; // Increase the factor (e.g., 0.1f) to make the features more prominent
        Vertices.Add(vertex.normalized * finalElevation);
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
        Vector3 middle = new Vector3(
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f);

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