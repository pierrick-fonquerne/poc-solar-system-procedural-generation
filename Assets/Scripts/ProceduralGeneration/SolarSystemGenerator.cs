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

        // Generate stars
        for (int i = 0; i < numberOfStars; i++)
        {
            starGenerator.GenerateStar(i);
        }

        // Generate planets
        for (int i = 0; i < numberOfPlanets; i++)
        {
            GameObject planet = planetGenerator.GeneratePlanet(i);
        }
    }
}
