using UnityEngine;

/// <summary>
/// Generates planets in the solar system.
/// </summary>
[System.Serializable]
public class PlanetGenerator : CelestialObject
{
    private const int LodLevelCount = 3;

    public int maxNumberOfMoonsPerPlanet = 3;
    public float minPlanetRadius = 2f;
    public float maxPlanetRadius = 4f;
    public int planetSubdivisions = 3;
    public float orbitalSpeedMultiplier = 1f;
    public float rotationSpeedMultiplier = 1f;

    [Tooltip("Base noise frequency of the procedural terrain.")]
    public float terrainFrequency = 2f;

    [Tooltip("Maximum terrain elevation as a fraction of the planet radius.")]
    [Range(0f, 1f)]
    public float terrainRelief = 0.15f;

    /// <summary>
    /// Generates a planet with a random radius, procedural terrain, and LOD
    /// meshes, and places it using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 0).</param>
    /// <param name="star">The GameObject representing the star the planet orbits around.</param>
    /// <returns>The generated planet GameObject, or null if creation failed.</returns>
    public GameObject GeneratePlanet(int planetIndex, GameObject star)
    {
        float radius = Random.Range(minPlanetRadius, maxPlanetRadius);

        // The same seed is used for every LOD mesh so the relief stays
        // consistent when the controller switches between detail levels.
        int terrainSeed = Random.Range(0, 100000);
        float terrainAmplitude = radius * terrainRelief;

        Mesh[] lodMeshes = new Mesh[LodLevelCount];
        for (int lod = 0; lod < LodLevelCount; lod++)
        {
            int subdivisions = Mathf.Max(0, planetSubdivisions - lod);
            IcoSphereGenerator generator = new IcoSphereGenerator(radius, subdivisions);
            lodMeshes[lod] = generator.BuildMesh($"Planet {planetIndex} LOD{lod}");
            PlanetTerrainGenerator.ApplyTerrain(lodMeshes[lod], terrainSeed, terrainFrequency, terrainAmplitude);
        }

        GameObject planet = CelestialObjectFactory.CreateObject(lodMeshes[0], $"Planet {planetIndex}");
        if (planet == null)
        {
            return null;
        }

        planet.transform.SetParent(parentTransform);

        // Configure LOD switching; the controller falls back to built-in
        // distances when the configuration asset is missing.
        PlanetLodConfig lodConfig = Resources.Load<PlanetLodConfig>("Settings/PlanetLodConfig");
        PlanetLodController lodController = planet.AddComponent<PlanetLodController>();
        lodController.Initialize(lodMeshes, lodConfig);
        if (lodConfig != null && lodConfig.enableBenchmarking)
        {
            PlanetLodBenchmark benchmark = planet.AddComponent<PlanetLodBenchmark>();
            benchmark.ApplyConfig(lodConfig);
        }

        // Collider for surface interactions, kept in sync with the active LOD.
        // The rigidbody is kinematic, which allows a non-convex mesh collider.
        MeshCollider meshCollider = planet.AddComponent<MeshCollider>();
        lodController.SetMeshCollider(meshCollider);

        // Calculate the planet's position using the Titius-Bode formula and a random angle
        float planetDistance = TitiusBodeDistance(planetIndex);
        float angle = Random.Range(0, 2 * Mathf.PI);
        float x = planetDistance * Mathf.Cos(angle);
        float z = planetDistance * Mathf.Sin(angle);
        planet.transform.localPosition = new Vector3(x, 0, z);

        Rigidbody rigidbody = planet.AddComponent<Rigidbody>();
        rigidbody.mass = CalculateMass(radius);
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        // Attractor used by moons to derive their orbital velocity
        planet.AddComponent<GravityAttractor>();

        // Kepler-based orbital speed around the star, driven kinematically
        float distanceToStar = Vector3.Distance(planet.transform.position, star.transform.position);
        float orbitalSpeed = star.GetComponent<GravityAttractor>().InitialOrbitalVelocity(distanceToStar);
        Orbiter orbiter = planet.AddComponent<Orbiter>();
        orbiter.SetOrbit(star.transform, orbitalSpeed * orbitalSpeedMultiplier);

        // Self rotation
        float rotationSpeed = Random.Range(0.1f, 1f) * rotationSpeedMultiplier;
        Rotator rotator = planet.AddComponent<Rotator>();
        rotator.angularVelocity = rotationSpeed;

        return planet;
    }

    /// <summary>
    /// Calculates the distance of a planet from its star using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 0).</param>
    /// <returns>The distance of the planet from the star in Unity units.</returns>
    public static float TitiusBodeDistance(int planetIndex)
    {
        float distanceAU = 0.4f + 0.3f * Mathf.Pow(2, planetIndex - 1);

        return distanceAU * Constants.AU_TO_UNITY_UNITS * Constants.SCALE_FACTOR;
    }
}
