using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script generates a solar system with stars, planets, and moons.
/// </summary>
public class SolarSystemGenerator : MonoBehaviour
{
    // Customizable properties
    public int numberOfStars = 1;
    public int numberOfPlanets = 3;
    public int maxNumberOfPlanets = 8;

    private void Start()
    {
        StarGenerator starGenerator = new StarGenerator(transform);
        PlanetGenerator planetGenerator = new PlanetGenerator(transform);

        // Create a list to store the GameObjects of each generated star
        List<GameObject> stars = new List<GameObject>();

        // Generate stars
        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject star = starGenerator.GenerateStar(i);
            stars.Add(star);
        }

        // Generate planets
        for (int i = 0; i < numberOfPlanets; i++)
        {
            // Pass the GameObject of the first star as an argument
            // You can modify this to pass the GameObject of the star you want the planet to orbit
            GameObject planet = planetGenerator.GeneratePlanet(i, stars[0]);
        }
    }
}
