using UnityEngine;

/// <summary>
/// Applies procedural terrain to celestial body meshes using layered 3D noise.
/// The noise field is a function of the unit-sphere direction and a seed, so
/// meshes of different resolutions (LOD levels) built with the same seed get
/// a consistent relief.
/// </summary>
public static class PlanetTerrainGenerator
{
    /// <summary>
    /// Displaces the vertices of a sphere mesh along their radial direction
    /// using fractal noise.
    /// </summary>
    /// <param name="mesh">The sphere mesh to displace.</param>
    /// <param name="seed">Seed controlling the noise field; identical seeds produce identical relief.</param>
    /// <param name="frequency">Base noise frequency controlling feature size.</param>
    /// <param name="amplitude">Maximum radial displacement in world units.</param>
    /// <param name="octaves">Number of noise layers combined for fractal detail.</param>
    public static void ApplyTerrain(Mesh mesh, int seed, float frequency = 2f, float amplitude = 0.5f, int octaves = 3)
    {
        if (mesh == null)
        {
            Debug.LogError("PlanetTerrainGenerator.ApplyTerrain called with a null mesh.");
            return;
        }

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 direction = vertices[i].normalized;
            float elevation = FractalNoise(direction, seed, frequency, octaves);
            vertices[i] += direction * elevation * amplitude;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    /// <summary>
    /// Convenience overload that displaces the mesh currently assigned to the
    /// GameObject's MeshFilter.
    /// </summary>
    /// <param name="body">The celestial body whose mesh will be modified.</param>
    /// <param name="seed">Seed controlling the noise field.</param>
    /// <param name="frequency">Base noise frequency controlling feature size.</param>
    /// <param name="amplitude">Maximum radial displacement in world units.</param>
    public static void ApplyTerrain(GameObject body, int seed, float frequency = 2f, float amplitude = 0.5f)
    {
        if (body == null)
        {
            Debug.LogError("PlanetTerrainGenerator.ApplyTerrain called with a null GameObject.");
            return;
        }

        MeshFilter meshFilter = body.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("PlanetTerrainGenerator requires the body to have a MeshFilter component.");
            return;
        }

        ApplyTerrain(meshFilter.sharedMesh, seed, frequency, amplitude);
    }

    // Standard fBm: each octave doubles the frequency and halves the amplitude.
    // The result is normalized to [0, 1].
    private static float FractalNoise(Vector3 direction, int seed, float baseFrequency, int octaves)
    {
        float total = 0f;
        float amplitude = 1f;
        float frequency = baseFrequency;
        float maxValue = 0f;

        for (int octave = 0; octave < octaves; octave++)
        {
            total += Noise3D(direction * frequency, seed + octave * 131) * amplitude;
            maxValue += amplitude;
            amplitude *= 0.5f;
            frequency *= 2f;
        }

        return maxValue > 0f ? total / maxValue : 0f;
    }

    // Unity only ships 2D Perlin noise; averaging the three axis-aligned
    // slices gives an inexpensive seamless 3D approximation.
    private static float Noise3D(Vector3 point, int seed)
    {
        // Offset the sampling position by the seed to select a noise variant;
        // the fractional multiplier avoids Perlin lattice alignment. The base
        // offset keeps the sampling domain positive, where PerlinNoise behaves
        // consistently (negative coordinates mirror around zero).
        float offset = 1000f + (seed % 65536) * 0.7317f;

        float xy = Mathf.PerlinNoise(point.x + offset, point.y + offset);
        float yz = Mathf.PerlinNoise(point.y + offset, point.z + offset);
        float zx = Mathf.PerlinNoise(point.z + offset, point.x + offset);

        return (xy + yz + zx) / 3f;
    }
}
