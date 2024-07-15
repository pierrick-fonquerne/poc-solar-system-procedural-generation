using UnityEngine;

/// <summary>
/// Generates planets in the solar system.
/// </summary>
public class PlanetGenerator : CelestialObject
{
    /// <summary>
    /// The maximum number of moons per planet.
    /// </summary>
    public int maxNumberOfMoonsPerPlanet = 3;

    /// <summary>
    /// The minimum radius for planets.
    /// </summary>
    public float minPlanetRadius = 2f;

    /// <summary>
    /// The maximum radius for planets.
    /// </summary>
    public float maxPlanetRadius = 4f;

    /// <summary>
    /// The number of subdivisions for planets.
    /// </summary>
    public int planetSubdivisions = 2;

    /// <summary>
    /// The multiplier for orbital speed.
    /// </summary>
    public float orbitalSpeedMultiplier = 2f;

    /// <summary>
    /// The multiplier for rotation speed.
    /// </summary>
    public float rotationSpeedMultiplier = 1f;

    private MoonGenerator moonGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetGenerator"/> class.
    /// </summary>
    /// <param name="parentTransform">The parent transform for the planet.</param>
    public PlanetGenerator(Transform parentTransform) : base(parentTransform)
    {
        moonGenerator = new MoonGenerator(parentTransform);
    }

    /// <summary>
    /// Generates a planet with a random radius and subdivisions, and places it at an index using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 0).</param>
    /// <param name="star">The GameObject representing the star the planet orbits around.</param>
    /// <returns>The generated planet GameObject.</returns>
    public GameObject GeneratePlanet(int planetIndex, GameObject star)
    {
        float radius = Random.Range(minPlanetRadius, maxPlanetRadius);
        GameObject planet = GenerateIcosphere(radius, planetSubdivisions);

        float planetDistance = TitiusBodeFormula(planetIndex);
        float angle = Random.Range(0, 2 * Mathf.PI);
        float x = planetDistance * Mathf.Cos(angle);
        float z = planetDistance * Mathf.Sin(angle);

        planet.transform.parent = parentTransform;
        planet.transform.localPosition = new Vector3(x, 0, z);
        Debug.Log($"Planet generated at position : {planet.transform.position}");

        planet.AddComponent<Rigidbody>().mass = CalculateMass(radius);
        planet.GetComponent<Rigidbody>().useGravity = false;
        planet.AddComponent<GravityAttractor>();

        float orbitalSpeed = star.GetComponent<GravityAttractor>().InitialOrbitalVelocity(planetDistance * Constants.SCALE_FACTOR);

        GravityAffectedObject gravityAffectedObject = planet.AddComponent<GravityAffectedObject>();
        Vector3 initialVelocity = Quaternion.Euler(0, 0, 90) * (star.transform.position - planet.transform.position).normalized * orbitalSpeed;
        gravityAffectedObject.GetComponent<Rigidbody>().velocity = initialVelocity;

        planet.AddComponent<Orbiter>().orbitalVelocity = orbitalSpeed;
        planet.GetComponent<Orbiter>().centerOfMass = star.transform;

        float rotationSpeed = Random.Range(0.1f, 1f) * rotationSpeedMultiplier;
        planet.AddComponent<Rotator>().angularVelocity = rotationSpeed;

        int numberOfMoons = Random.Range(0, maxNumberOfMoonsPerPlanet + 1);
        for (int j = 0; j < numberOfMoons; j++)
        {
            moonGenerator.GenerateMoon(planet, j);
        }

        return planet;
    }

    /// <summary>
    /// Calculates the distance of a planet from the star using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 1).</param>
    /// <returns>The estimated distance of the planet from the star.</returns>
    private float TitiusBodeFormula(int planetIndex)
    {
        float distanceAU = 0.4f + 0.3f * Mathf.Pow(2, planetIndex - 1);
        float distance = distanceAU * Constants.AU_TO_UNITY_UNITS * Constants.SCALE_FACTOR;
        return distance;
    }
}
