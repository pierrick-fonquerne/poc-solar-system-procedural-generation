using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    // Customizable properties
    public int numberOfStars = 1;
    public int numberOfPlanets = 3;
    public int maxNumberOfPlanets = 8;
    public float minStarRadius = 5f;
    public float maxStarRadius = 10f;
    public float minPlanetRadius = 1f;
    public float maxPlanetRadius = 3f;
    public int starSubdivisions = 3;
    public int planetSubdivisions = 2;
    public float starOrbitDistance = 20f;
    public float planetOrbitDistance = 10f;
    public int numberOfMoonsPerPlanet = 1;
    public int maxNumberOfMoonsPerPlanet = 3;
    public float minMoonRadius = 0.5f;
    public float maxMoonRadius = 1.5f;
    public int moonSubdivisions = 2;
    public float moonOrbitDistance = 5f;

    private void Start()
    {
        // Generate star
        for (int i = 0; i < numberOfStars; i++)
        {
            GenerateStar(i * starOrbitDistance);
        }

        Debug.Log($"Number of generated stars : {numberOfStars}");

        // Generate planets
        int totalMoons = 0;
        for (int i = 0; i < numberOfPlanets; i++)
        {
            numberOfMoonsPerPlanet = Random.Range(0, maxNumberOfMoonsPerPlanet + 1);
            totalMoons += numberOfMoonsPerPlanet;
            GeneratePlanet(i + 1);
        }

        Debug.Log($"Number of generated planets : {numberOfPlanets}");
        Debug.Log($"Number of generated moons : {totalMoons}");
    }

    /// <summary>
    /// Generates a star with a random radius and subdivisions and places it at a given distance.
    /// </summary>
    /// <param name="distance">The distance from the origin.</param>
    private void GenerateStar(float distance)
    {
        // Generate a random radius for the star within the specified range
        float radius = Random.Range(minStarRadius, maxStarRadius);

        // Generate an icosphere with the given radius and subdivisions
        GameObject star = GenerateIcosphere(radius, starSubdivisions);

        // Set the material of the star
        MeshRenderer starRenderer = star.GetComponent<MeshRenderer>();

        // Add a point light to the star to simulate its emission
        star.AddComponent<Light>();
        star.GetComponent<Light>().type = LightType.Point;
        star.GetComponent<Light>().range = starOrbitDistance * 2f;
        star.GetComponent<Light>().intensity = 1.5f * radius; // Set intensity based on the star's radius
        star.GetComponent<Light>().range = starOrbitDistance * Constants.SCALE_FACTOR * 2f;

        // Set the position of the star and make it a child of the SolarSystemGenerator GameObject
        star.transform.parent = transform;
        star.transform.localPosition = new Vector3(distance, 0, 0);
        Debug.Log($"Star generated at position : {star.transform.position}");

        // Add a rigidbody to the star to enable gravity calculations
        star.AddComponent<Rigidbody>();
        star.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        star.GetComponent<Rigidbody>().useGravity = false;

        // Add a GravityAttractor component to the star to make it attract other objects
        star.AddComponent<GravityAttractor>();

        // Add a GravityAffectedObject component to the star to make it affected by gravity
        star.AddComponent<GravityAffectedObject>();
    }

    /// <summary>
    /// Generates a planet with a random radius and subdivisions, and places it at an index using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 1).</param>
    private void GeneratePlanet(int planetIndex)
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
        planet.transform.parent = transform;
        planet.transform.localPosition = new Vector3(x, 0, z);
        Debug.Log($"Planet generated at position : {planet.transform.position}");

        // Add necessary components to the planet
        planet.AddComponent<Rigidbody>();
        planet.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        planet.GetComponent<Rigidbody>().useGravity = false;
        planet.AddComponent<GravityAttractor>();
        planet.AddComponent<GravityAffectedObject>();

        // Generate moons for the planet
        for (int j = 0; j < numberOfMoonsPerPlanet; j++)
        {
            GenerateMoon(planet, j);
        }
    }

    /// <summary>
    /// Generates a moon with a random radius and subdivisions and places it around a given planet at a specified index.
    /// </summary>
    /// <param name="planet">The planet GameObject around which the moon will be placed.</param>
    /// <param name="moonIndex">The index of the moon (starting from 0).</param>
    private void GenerateMoon(GameObject planet, int moonIndex)
    {
        float radius = Random.Range(minMoonRadius, maxMoonRadius);
        GameObject moon = GenerateIcosphere(radius, moonSubdivisions);

        // Calculate the distance of the moon from the planet
        float moonDistance = (moonOrbitDistance + moonOrbitDistance * moonIndex) * Constants.SCALE_FACTOR;
        // Calculate a random angle for the moon's position
        float angle = Random.Range(0, 2 * Mathf.PI);
        // Calculate the x and z positions of the moon based on the distance and angle
        float x = moonDistance * Mathf.Cos(angle);
        float z = moonDistance * Mathf.Sin(angle);

        // Set the moon's parent to the planet and position it
        moon.transform.parent = planet.transform;
        moon.transform.localPosition = new Vector3(x, 0, z);
        Debug.Log($"Moon generated at position : {moon.transform.position}");

        // Add a Rigidbody component to the moon and set its mass and useGravity properties
        moon.AddComponent<Rigidbody>();
        moon.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        moon.GetComponent<Rigidbody>().useGravity = false;
        moon.AddComponent<GravityAffectedObject>();
    }

    /// <summary>
    /// Generates an icosphere with the given radius and subdivisions.
    /// </summary>
    /// <param name="radius">The radius of the generated icosphere.</param>
    /// <param name="subdivisions">The number of subdivisions for the generated icosphere.</param>
    /// <returns>The generated icosphere GameObject.</returns>
    private GameObject GenerateIcosphere(float radius, int subdivisions)
    {
        IcoSphereGenerator icoSphereGenerator = new IcoSphereGenerator(radius, subdivisions);
        CelestialObjectGenerator celestialObjectGenerator = new CelestialObjectGenerator(icoSphereGenerator.Vertices, icoSphereGenerator.Triangles);

        GameObject icosphere = celestialObjectGenerator.GenerateObject();
        icosphere.transform.SetParent(transform);

        return icosphere;
    }

    /// <summary>
    /// Calculates the mass of an object with the given radius and density.
    /// </summary>
    /// <param name="radius">The radius of the object.</param>
    /// <returns>The mass of the object in kilograms.</returns>
    private float CalculateMass(float radius)
    {
        float density = 5500f; // kg/m^3 (approximate value for celestial objects)
        float volume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);

        return density * volume;
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
