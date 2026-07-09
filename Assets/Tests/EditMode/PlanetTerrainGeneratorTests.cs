using NUnit.Framework;
using UnityEngine;

public class PlanetTerrainGeneratorTests
{
    private const float Radius = 2f;
    private const float Amplitude = 0.3f;

    private static Mesh CreateSphereMesh()
    {
        return new IcoSphereGenerator(Radius, 2).BuildMesh("TerrainTestSphere");
    }

    [Test]
    public void ApplyTerrain_DisplacesVerticesWithinAmplitude()
    {
        Mesh mesh = CreateSphereMesh();

        try
        {
            PlanetTerrainGenerator.ApplyTerrain(mesh, seed: 42, frequency: 2f, amplitude: Amplitude);

            bool anyDisplaced = false;
            foreach (Vector3 vertex in mesh.vertices)
            {
                float magnitude = vertex.magnitude;
                Assert.That(magnitude, Is.InRange(Radius - 1e-3f, Radius + Amplitude + 1e-3f),
                    "Vertices should only be displaced outward, up to the amplitude.");
                if (magnitude > Radius + 1e-3f)
                {
                    anyDisplaced = true;
                }
            }

            Assert.IsTrue(anyDisplaced, "The terrain should displace at least some vertices.");
        }
        finally
        {
            Object.DestroyImmediate(mesh);
        }
    }

    [Test]
    public void ApplyTerrain_IsDeterministicForAGivenSeed()
    {
        Mesh meshA = CreateSphereMesh();
        Mesh meshB = CreateSphereMesh();

        try
        {
            PlanetTerrainGenerator.ApplyTerrain(meshA, seed: 1234, frequency: 2f, amplitude: Amplitude);
            PlanetTerrainGenerator.ApplyTerrain(meshB, seed: 1234, frequency: 2f, amplitude: Amplitude);

            Vector3[] verticesA = meshA.vertices;
            Vector3[] verticesB = meshB.vertices;

            Assert.AreEqual(verticesA.Length, verticesB.Length);
            for (int i = 0; i < verticesA.Length; i++)
            {
                Assert.AreEqual(verticesA[i], verticesB[i],
                    "The same seed must produce the same terrain (required for consistent LOD meshes).");
            }
        }
        finally
        {
            Object.DestroyImmediate(meshA);
            Object.DestroyImmediate(meshB);
        }
    }

    [Test]
    public void ApplyTerrain_DifferentSeedsProduceDifferentTerrain()
    {
        Mesh meshA = CreateSphereMesh();
        Mesh meshB = CreateSphereMesh();

        try
        {
            PlanetTerrainGenerator.ApplyTerrain(meshA, seed: 1, frequency: 2f, amplitude: Amplitude);
            PlanetTerrainGenerator.ApplyTerrain(meshB, seed: 99999, frequency: 2f, amplitude: Amplitude);

            Vector3[] verticesA = meshA.vertices;
            Vector3[] verticesB = meshB.vertices;

            bool anyDifferent = false;
            for (int i = 0; i < verticesA.Length; i++)
            {
                if ((verticesA[i] - verticesB[i]).sqrMagnitude > 1e-8f)
                {
                    anyDifferent = true;
                    break;
                }
            }

            Assert.IsTrue(anyDifferent, "Different seeds should produce different terrain.");
        }
        finally
        {
            Object.DestroyImmediate(meshA);
            Object.DestroyImmediate(meshB);
        }
    }
}
