using UnityEngine;

/// <summary>
/// Generates planets in the solar system.
/// </summary>
public class PlanetGenerator : CelestialObject
{
    public int maxNumberOfMoonsPerPlanet = 3;
    public float minPlanetRadius = 2f;
    public float maxPlanetRadius = 4f;
    public int planetSubdivisions = 2;
    public float orbitalSpeedMultiplier = 2f;
    public float rotationSpeedMultiplier = 1f;

    private MoonGenerator moonGenerator;

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
        // Generate a planet with a random radius and subdivisions
        float radius = Random.Range(minPlanetRadius, maxPlanetRadius);
        GameObject planet = GenerateIcosphere(radius, planetSubdivisions);

        // Calculate the planet's position using the Titius-Bode formula and a random angle
        float planetDistance = TitiusBodeFormula(planetIndex);
        float angle = Random.Range(0, 2 * Mathf.PI);
        float x = planetDistance * Mathf.Cos(angle);
        float z = planetDistance * Mathf.Sin(angle);

        // Set the planet's position and parent it to the Solar System's game object
        planet.transform.parent = parentTransform;
        planet.transform.localPosition = new Vector3(x, 0, z);
        Debug.Log($"Planet generated at position : {planet.transform.position}");

        // Add necessary components to the planet
        planet.AddComponent<Rigidbody>();
        planet.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        planet.GetComponent<Rigidbody>().useGravity = false;
        planet.AddComponent<GravityAttractor>();

        // Calculate the orbital speed of the planet
        float orbitalSpeed = star.GetComponent<GravityAttractor>().InitialOrbitalVelocity(planetDistance * Constants.SCALE_FACTOR);

        // Add a GravityAffectedObject component to the planet to make it affected by gravity
        GravityAffectedObject gravityAffectedObject = planet.AddComponent<GravityAffectedObject>();

        // Set the initial velocity of the planet
        Vector3 initialVelocity = Quaternion.Euler(0, 0, 90) * (star.transform.position - planet.transform.position).normalized * orbitalSpeed;
        gravityAffectedObject.GetComponent<Rigidbody>().velocity = initialVelocity;

        // Set the planet's orbital speed
        planet.AddComponent<Orbiter>();
        planet.GetComponent<Orbiter>().orbitalVelocity = orbitalSpeed;
        planet.GetComponent<Orbiter>().centerOfMass = star.transform;

        // Set the planet's rotation speed
        float rotationSpeed = Random.Range(0.1f, 1f) * rotationSpeedMultiplier;
        planet.AddComponent<Rotator>();
        planet.GetComponent<Rotator>().angularVelocity = rotationSpeed;

        // Generate moons for the planet
        int numberOfMoons = Random.Range(0, maxNumberOfMoonsPerPlanet + 1);
        for (int j = 0; j < numberOfMoons; j++)
        {
            moonGenerator.GenerateMoon(planet, j);
        }

        return planet;
    }

    /// <summary>
    /// Calculates the orbital speed of a planet using Kepler's law.
    /// </summary>
    /// <param name="starMass">The mass of the central star in kilograms.</param>
    /// <param name="distance">The distance between the planet and the star in meters.</param>
    /// <returns>The orbital speed of the planet in meters per second.</returns>
    private float CalculateOrbitalSpeed(float starMass, float distance)
    {
        float orbitalSpeed = Mathf.Sqrt(Constants.GRAVITATIONAL_CONSTANT * starMass / distance);

        return orbitalSpeed;
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
