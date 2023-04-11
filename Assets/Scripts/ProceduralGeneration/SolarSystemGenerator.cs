using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    // Customizable properties
    public int numberOfSuns = 1;
    public int numberOfPlanets = 3;
    public int maxNumberOfPlanets = 8;
    public float minSunRadius = 5f;
    public float maxSunRadius = 10f;
    public float minPlanetRadius = 1f;
    public float maxPlanetRadius = 3f;
    public int sunSubdivisions = 3;
    public int planetSubdivisions = 2;
    public float sunOrbitDistance = 20f;
    public float planetOrbitDistance = 10f;
    public int numberOfMoonsPerPlanet = 1;
    public int maxNumberOfMoonsPerPlanet = 3;
    public float minMoonRadius = 0.5f;
    public float maxMoonRadius = 1.5f;
    public int moonSubdivisions = 2;
    public float moonOrbitDistance = 5f;
    public List<GameObject> celestialObjects = new List<GameObject>();
    public GameObject distanceTextPrefab;
    public GameObject sunDistanceTextPrefab;

    private void Start()
    {
        // Generate suns
        for (int i = 0; i < numberOfSuns; i++)
        {
            GenerateSunAtDistance(i * sunOrbitDistance);
        }

        // Generate planets
        for (int i = 0; i < numberOfPlanets; i++)
        {
            numberOfMoonsPerPlanet = Random.Range(0, maxNumberOfMoonsPerPlanet + 1);
            GeneratePlanetAtIndex(i + 1);
        }
    }

    /// <summary>
    /// Generates a sun with a random radius and subdivisions and places it at a given distance.
    /// </summary>
    /// <param name="distance">The distance from the origin.</param>
    private void GenerateSunAtDistance(float distance)
    {
        // Generate a random radius for the sun within the specified range
        float radius = Random.Range(minSunRadius, maxSunRadius);

        // Generate an icosphere with the given radius and subdivisions
        GameObject sun = GenerateIcosphere(radius, sunSubdivisions);

        // Set the material of the sun
        MeshRenderer sunRenderer = sun.GetComponent<MeshRenderer>();
        sunRenderer.material = new Material(Resources.Load<Shader>("Shaders/SunShader"));
        sunRenderer.material.SetColor("_Color", Color.yellow);
        sunRenderer.material.SetFloat("_Emission", 2f);

        // Add a point light to the sun to simulate its emission
        sun.AddComponent<Light>();
        sun.GetComponent<Light>().type = LightType.Point;
        sun.GetComponent<Light>().range = sunOrbitDistance * 2f;
        sun.GetComponent<Light>().intensity = 1.5f;
        sun.GetComponent<Light>().range = sunOrbitDistance * Constants.SCALE_FACTOR * 2f;

        // Set the position of the sun and make it a child of the SolarSystemGenerator GameObject
        sun.transform.parent = transform;
        sun.transform.localPosition = new Vector3(distance, 0, 0);

        // Add a rigidbody to the sun to enable gravity calculations
        sun.AddComponent<Rigidbody>();
        sun.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        sun.GetComponent<Rigidbody>().useGravity = false;

        // Add a GravityAttractor component to the sun to make it attract other objects
        sun.AddComponent<GravityAttractor>();

        // Add a GravityAffectedObject component to the sun to make it affected by gravity
        sun.AddComponent<GravityAffectedObject>();

        // Generate the distance text object and set its parent to the sun
        GameObject sunDistanceTextObject = Instantiate(sunDistanceTextPrefab, sun.transform);
    }

    /// <summary>
    /// Generates a planet with a random radius and subdivisions, and places it at an index using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 1).</param>
    private void GeneratePlanetAtIndex(int planetIndex)
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

        // Add necessary components to the planet
        planet.AddComponent<Rigidbody>();
        planet.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        planet.GetComponent<Rigidbody>().useGravity = false;
        planet.AddComponent<GravityAttractor>();
        planet.AddComponent<GravityAffectedObject>();

        // Generate moons for the planet
        for (int j = 0; j < numberOfMoonsPerPlanet; j++)
        {
            GenerateMoonForPlanet(planet, j);
        }

        // Create a TextMeshPro text object for the planet's distance
        GameObject textObject = Instantiate(distanceTextPrefab, planet.transform.position + new Vector3(1, 0, 0), Quaternion.identity, planet.transform); TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();

        // Set the text value to the planet's distance in astronomical units
        text.SetText($"{planetDistance / Constants.AU_TO_UNITY_UNITS} AU");

        // Set the position of the text object relative to the planet
        textObject.transform.localPosition = new Vector3(0, 2 * radius, 0);
    }


    /// <summary>
    /// Generates a moon with a random radius and subdivisions and places it around a given planet at a specified index.
    /// </summary>
    /// <param name="planet">The planet GameObject around which the moon will be placed.</param>
    /// <param name="moonIndex">The index of the moon (starting from 0).</param>
    private void GenerateMoonForPlanet(GameObject planet, int moonIndex)
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
    /// Calculates the distance of a planet from the sun using the Titius-Bode formula.
    /// </summary>
    /// <param name="planetIndex">The index of the planet (starting from 1).</param>
    /// <returns>The estimated distance of the planet from the sun.</returns>
    private float TitiusBodeFormula(int planetIndex)
    {
        float distanceAU = 0.4f + 0.3f * Mathf.Pow(2, planetIndex - 1);
        float distance = distanceAU * Constants.AU_TO_UNITY_UNITS * Constants.SCALE_FACTOR;

        return distance;
    }
}
