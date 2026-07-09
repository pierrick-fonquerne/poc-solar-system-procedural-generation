using NUnit.Framework;
using UnityEngine;

public class IcoSphereGeneratorTests
{
    // A subdivided icosahedron has 10 * 4^n + 2 unique vertices and 20 * 4^n faces.
    [TestCase(0, 12, 20)]
    [TestCase(1, 42, 80)]
    [TestCase(2, 162, 320)]
    [TestCase(3, 642, 1280)]
    public void Generator_ProducesExpectedTopology(int subdivisions, int expectedVertices, int expectedFaces)
    {
        IcoSphereGenerator generator = new IcoSphereGenerator(1f, subdivisions);

        Assert.AreEqual(expectedVertices, generator.Vertices.Count, "Unexpected vertex count.");
        Assert.AreEqual(expectedFaces * 3, generator.Triangles.Count, "Unexpected triangle index count.");
    }

    [TestCase(1f)]
    [TestCase(5f)]
    [TestCase(12.5f)]
    public void Generator_ProjectsAllVerticesOntoSphere(float radius)
    {
        IcoSphereGenerator generator = new IcoSphereGenerator(radius, 2);

        foreach (Vector3 vertex in generator.Vertices)
        {
            Assert.AreEqual(radius, vertex.magnitude, radius * 1e-4f,
                "Every vertex should lie on the sphere of the requested radius.");
        }
    }

    [Test]
    public void Generator_TriangleIndicesAreInRange()
    {
        IcoSphereGenerator generator = new IcoSphereGenerator(1f, 2);

        foreach (int index in generator.Triangles)
        {
            Assert.That(index, Is.InRange(0, generator.Vertices.Count - 1));
        }
    }

    [Test]
    public void BuildMesh_CopiesGeometryAndName()
    {
        IcoSphereGenerator generator = new IcoSphereGenerator(3f, 1);
        Mesh mesh = generator.BuildMesh("TestSphere");

        try
        {
            Assert.AreEqual("TestSphere", mesh.name);
            Assert.AreEqual(generator.Vertices.Count, mesh.vertexCount);
            Assert.AreEqual(generator.Triangles.Count, mesh.triangles.Length);
            Assert.AreEqual(generator.Vertices.Count, mesh.normals.Length, "Normals should be recalculated.");
        }
        finally
        {
            Object.DestroyImmediate(mesh);
        }
    }
}
