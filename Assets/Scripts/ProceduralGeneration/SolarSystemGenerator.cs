using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script generates a solar system with stars, planets, and moons.
/// </summary>
public class SolarSystemGenerator : MonoBehaviour
{
    /// <summary>
    /// The number of stars to generate.
    /// </summary>
    public int numberOfStars = 1;

    /// <summary>
    /// The number of planets to generate.
    /// </summary>
    public int numberOfPlanets = 3;

    /// <summary>
    /// The maximum number of planets that can be generated.
    /// </summary>
    public int maxNumberOfPlanets = 8;

    /// <summary>
    /// The minimum radius for stars.
    /// </summary>
    public float minStarRadius = 10f;

    /// <summary>
    /// The maximum radius for stars.
    /// </summary>
    public float maxStarRadius = 15f;

    /// <summary>
    /// The minimum radius for planets.
    /// </summary>
    public float minPlanetRadius = 2f;

    /// <summary>
    /// The maximum radius for planets.
    /// </summary>
    public float maxPlanetRadius = 4f;

    /// <summary>
    /// The number of subdivisions for stars and planets.
    /// </summary>
    public int subdivisions = 2;

    private List<GameObject> stars;
    private List<GameObject> planets;

    /// <summary>
    /// Starts the solar system generation.
    /// </summary>
    private void Start()
    {
    }

    /// <summary>
    /// Generates the solar system with the specified number of stars and planets.
    /// </summary>
    public void GenerateSolarSystem()
    {
        ClearExistingSolarSystem();

        StarGenerator starGenerator = new StarGenerator(transform)
        {
            minStarRadius = minStarRadius,
            maxStarRadius = maxStarRadius,
            starSubdivisions = subdivisions
        };

        PlanetGenerator planetGenerator = new PlanetGenerator(transform)
        {
            minPlanetRadius = minPlanetRadius,
            maxPlanetRadius = maxPlanetRadius,
            planetSubdivisions = subdivisions
        };

        stars = new List<GameObject>();
        planets = new List<GameObject>();

        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject star = starGenerator.GenerateStar(i);
            stars.Add(star);
        }

        for (int i = 0; i < numberOfPlanets; i++)
        {
            GameObject planet = planetGenerator.GeneratePlanet(i, stars[0]);
            planets.Add(planet);
        }
    }

    /// <summary>
    /// Clears the existing solar system by destroying all generated stars and planets.
    /// </summary>
    public void ClearExistingSolarSystem()
    {
        if (stars != null)
        {
            foreach (GameObject star in stars)
            {
                Destroy(star);
            }
            stars.Clear();
        }

        if (planets != null)
        {
            foreach (GameObject planet in planets)
            {
                Destroy(planet);
            }
            planets.Clear();
        }
    }
}
