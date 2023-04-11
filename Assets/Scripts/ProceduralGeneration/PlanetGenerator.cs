using UnityEngine;

/// <summary>
/// Generates planets in the solar system.
/// </summary>
public class PlanetGenerator : CelestialObject
{
    public int maxNumberOfMoonsPerPlanet = 3;
    public float minPlanetRadius = 1f;
    public float maxPlanetRadius = 3f;
    public int planetSubdivisions = 2;

    private MoonGenerator moonGenerator;

    public PlanetGenerator(Transform parentTransform) : base(parentTransform)
    {
        moonGenerator = new MoonGenerator(parentTransform);
    }

    /// <summary>
    /// Generates a planet with a random radius and subdivisions, and places it at an index using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 0).</param>
    /// <returns>The generated planet GameObject.</returns>
    public GameObject GeneratePlanet(int planetIndex)
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
        planet.AddComponent<GravityAffectedObject>();

        // Generate moons for the planet
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
