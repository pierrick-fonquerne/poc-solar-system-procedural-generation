using UnityEngine;

/// <summary>
/// Generates moons around planets in the solar system.
/// </summary>
[System.Serializable]
public class MoonGenerator : CelestialObject
{
    public float minMoonRadius = 0.5f;
    public float maxMoonRadius = 1f;
    public int moonSubdivisions = 2;

    [Tooltip("Spacing between consecutive moon orbits, in unscaled units.")]
    public float moonOrbitDistance = 1f;

    public float rotationSpeedMultiplier = 1f;
    public float orbitalSpeedMultiplier = 1f;
    public float minRotationSpeed = 3f;
    public float maxRotationSpeed = 5f;

    [Tooltip("Base noise frequency of the moon terrain.")]
    public float terrainFrequency = 3f;

    [Tooltip("Maximum terrain elevation as a fraction of the moon radius.")]
    [Range(0f, 1f)]
    public float terrainRelief = 0.1f;

    /// <summary>
    /// Generates a moon with a random radius and places it in orbit around the given planet.
    /// </summary>
    /// <param name="planet">The planet GameObject around which the moon will be placed.</param>
    /// <param name="moonIndex">The index of the moon (starting from 0).</param>
    /// <returns>The generated moon GameObject, or null if creation failed.</returns>
    public GameObject GenerateMoon(GameObject planet, int moonIndex)
    {
        float radius = Random.Range(minMoonRadius, maxMoonRadius);
        GameObject moon = GenerateIcosphere(radius, moonSubdivisions, $"Moon {moonIndex}");
        if (moon == null)
        {
            return null;
        }

        PlanetTerrainGenerator.ApplyTerrain(moon, Random.Range(0, 100000), terrainFrequency, radius * terrainRelief);

        // Orbit radius: clear the planet surface, then space each moon out
        float planetRadius = planet.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x;
        float moonDistance = planetRadius + (moonOrbitDistance * (moonIndex + 1)) * Constants.SCALE_FACTOR;

        // Place the moon at a random angle around the planet
        float angle = Random.Range(0, 2 * Mathf.PI);
        float x = moonDistance * Mathf.Cos(angle);
        float z = moonDistance * Mathf.Sin(angle);

        // Parent the moon to the planet so it follows the planet's orbit
        moon.transform.SetParent(planet.transform);
        moon.transform.localPosition = new Vector3(x, 0, z);

        Rigidbody rigidbody = moon.AddComponent<Rigidbody>();
        rigidbody.mass = CalculateMass(radius);
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        // Self rotation
        float rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * rotationSpeedMultiplier;
        Rotator rotator = moon.AddComponent<Rotator>();
        rotator.angularVelocity = rotationSpeed;

        // Kepler-based orbital speed around the planet, driven kinematically
        float orbitalSpeed = planet.GetComponent<GravityAttractor>().InitialOrbitalVelocity(moonDistance);
        Orbiter orbiter = moon.AddComponent<Orbiter>();
        orbiter.SetOrbit(planet.transform, orbitalSpeed * orbitalSpeedMultiplier);

        return moon;
    }
}
