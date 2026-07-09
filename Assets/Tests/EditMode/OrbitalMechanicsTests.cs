using NUnit.Framework;
using UnityEngine;

public class OrbitalMechanicsTests
{
    [Test]
    public void TitiusBodeDistance_IncreasesWithPlanetIndex()
    {
        for (int i = 0; i < 7; i++)
        {
            Assert.Less(PlanetGenerator.TitiusBodeDistance(i), PlanetGenerator.TitiusBodeDistance(i + 1),
                "Each planet should be farther from the star than the previous one.");
        }
    }

    [Test]
    public void TitiusBodeDistance_MatchesFormulaForFirstPlanet()
    {
        // Index 0: 0.4 + 0.3 * 2^-1 = 0.55 AU
        float expected = 0.55f * Constants.AU_TO_UNITY_UNITS * Constants.SCALE_FACTOR;
        Assert.AreEqual(expected, PlanetGenerator.TitiusBodeDistance(0), 1e-3f);
    }

    [Test]
    public void CalculateMass_ScalesWithRadiusCubed()
    {
        float massSmall = CelestialObject.CalculateMass(1f);
        float massLarge = CelestialObject.CalculateMass(2f);

        Assert.Greater(massSmall, 0f);
        Assert.AreEqual(8f, massLarge / massSmall, 1e-3f, "Mass should scale with the cube of the radius.");
    }

    [Test]
    public void InitialOrbitalVelocity_FollowsKeplerLaw()
    {
        GameObject body = new GameObject("AttractorTest");

        try
        {
            Rigidbody rigidbody = body.AddComponent<Rigidbody>();
            rigidbody.mass = 1e6f;
            GravityAttractor attractor = body.AddComponent<GravityAttractor>();

            float distance = 100f;
            float expected = Mathf.Sqrt(Constants.GRAVITATIONAL_CONSTANT * rigidbody.mass / distance);
            Assert.AreEqual(expected, attractor.InitialOrbitalVelocity(distance), expected * 1e-4f);

            // Kepler: a body twice as far orbits sqrt(2) times slower
            float near = attractor.InitialOrbitalVelocity(distance);
            float far = attractor.InitialOrbitalVelocity(distance * 2f);
            Assert.AreEqual(Mathf.Sqrt(2f), near / far, 1e-3f);
        }
        finally
        {
            Object.DestroyImmediate(body);
        }
    }

    [Test]
    public void InitialOrbitalVelocity_ReturnsZeroAtZeroDistance()
    {
        GameObject body = new GameObject("AttractorTest");

        try
        {
            body.AddComponent<Rigidbody>();
            GravityAttractor attractor = body.AddComponent<GravityAttractor>();

            Assert.AreEqual(0f, attractor.InitialOrbitalVelocity(0f));
        }
        finally
        {
            Object.DestroyImmediate(body);
        }
    }
}
