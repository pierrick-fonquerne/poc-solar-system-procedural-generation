using UnityEngine;

/// <summary>
/// Generates procedural terrain for a planet using Perlin noise.
/// </summary>
public static class PlanetTerrainGenerator
{
    /// <summary>
    /// Applies a Perlin noise based heightmap to the given planet mesh.
    /// </summary>
    /// <param name="planet">The planet GameObject whose terrain will be modified.</param>
    /// <param name="frequency">Noise frequency controlling feature size.</param>
    /// <param name="amplitude">Noise amplitude controlling height variation.</param>
    public static void ApplyTerrain(GameObject planet, float frequency = 2f, float amplitude = 0.5f)
    {
        if (planet == null)
        {
            Debug.LogError("PlanetTerrainGenerator.ApplyTerrain called with null planet.");
            return;
        }

        MeshFilter meshFilter = planet.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("PlanetTerrainGenerator requires the planet to have a MeshFilter component.");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        int seed = Random.Range(0, 100000);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normal = normals[i];
            float noise = Mathf.PerlinNoise(normal.x * frequency + seed, normal.y * frequency + seed);
            vertices[i] += normal * noise * amplitude;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }
}
