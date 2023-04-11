using System.Collections.Generic;
using UnityEngine;

public class SolarSystemGenerator : MonoBehaviour
{
    // Customizable properties
    public int numberOfSuns = 1;
    public int numberOfPlanets = 3;
    public float minSunRadius = 5f;
    public float maxSunRadius = 10f;
    public float minPlanetRadius = 1f;
    public float maxPlanetRadius = 3f;
    public int sunSubdivisions = 3;
    public int planetSubdivisions = 2;
    public float sunOrbitDistance = 20f;
    public float planetOrbitDistance = 10f;
    public int numberOfMoonsPerPlanet = 1;
    public float minMoonRadius = 0.5f;
    public float maxMoonRadius = 1.5f;
    public int moonSubdivisions = 2;
    public float moonOrbitDistance = 5f;

    void Start()
    {
        // Generate suns
        for (int i = 0; i < numberOfSuns; i++)
        {
            GameObject sun = GenerateSun();
            sun.transform.parent = transform;
            float sunDistance = sunOrbitDistance * i;
            sun.transform.localPosition = new Vector3(sunDistance, 0, 0);
        }

        // Generate planets
        for (int i = 0; i < numberOfPlanets; i++)
        {
            GameObject planet = GeneratePlanet();
            planet.transform.parent = transform;
            float planetDistance = 50f + planetOrbitDistance * i;
            float angle = Random.Range(0, 2 * Mathf.PI);
            float x = planetDistance * Mathf.Cos(angle);
            float z = planetDistance * Mathf.Sin(angle);
            planet.transform.localPosition = new Vector3(x, 0, z);

            // Generate moons
            for (int j = 0; j < numberOfMoonsPerPlanet; j++)
            {
                GameObject moon = GenerateMoon();
                moon.transform.parent = planet.transform;
                float moonDistance = moonOrbitDistance + moonOrbitDistance * j;
                angle = Random.Range(0, 2 * Mathf.PI);
                x = moonDistance * Mathf.Cos(angle);
                z = moonDistance * Mathf.Sin(angle);
                moon.transform.localPosition = new Vector3(x, 0, z);
            }
        }
    }

    /// <summary>
    /// Generates a sun with a random radius and subdivisions.
    /// </summary>
    /// <returns>The generated sun GameObject.</returns>
    private GameObject GenerateSun()
    {
        float radius = Random.Range(minSunRadius, maxSunRadius);
        GameObject sun = GenerateIcosphere(radius, sunSubdivisions);
        MeshRenderer sunRenderer = sun.GetComponent<MeshRenderer>();

        sunRenderer.material = new Material(Resources.Load<Shader>("Shaders/SunShader"));
        sunRenderer.material.SetColor("_Color", Color.yellow);
        sunRenderer.material.SetFloat("_Emission", 2f);

        sun.AddComponent<Light>();
        sun.GetComponent<Light>().type = LightType.Point;
        sun.GetComponent<Light>().range = sunOrbitDistance * 2f;
        sun.GetComponent<Light>().intensity = 1.5f;

        return sun;
    }

    /// <summary>
    /// Generates a planet with a random radius and subdivisions.
    /// </summary>
    /// <returns>The generated planet GameObject.</returns>
    private GameObject GeneratePlanet()
    {
        float radius = Random.Range(minPlanetRadius, maxPlanetRadius);
        return GenerateIcosphere(radius, planetSubdivisions);
    }

    /// <summary>
    /// Generates a moon with a random radius and subdivisions.
    /// </summary>
    /// <returns>The generated moon GameObject.</returns>
    private GameObject GenerateMoon()
    {
        float radius = Random.Range(minMoonRadius, maxMoonRadius);
        return GenerateIcosphere(radius, moonSubdivisions);
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
}
