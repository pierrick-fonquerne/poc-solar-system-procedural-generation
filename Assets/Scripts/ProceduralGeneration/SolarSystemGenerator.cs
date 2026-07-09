using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a solar system with stars, planets, and moons.
/// </summary>
public class SolarSystemGenerator : MonoBehaviour
{
    [Header("Generation")]
    [Tooltip("When enabled, a new random seed is picked on every run.")]
    public bool useRandomSeed = true;

    [Tooltip("Seed used for the generation. The same seed reproduces the same solar system.")]
    public int seed;

    [Min(0)]
    public int numberOfStars = 1;

    [Min(0)]
    public int numberOfPlanets = 3;

    [Min(1)]
    public int maxNumberOfPlanets = 8;

    [Header("Generators")]
    public StarGenerator starGenerator = new StarGenerator();
    public PlanetGenerator planetGenerator = new PlanetGenerator();
    public MoonGenerator moonGenerator = new MoonGenerator();

    private void Start()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        Random.InitState(seed);
        Debug.Log($"Generating solar system with seed {seed}.");

        starGenerator.Initialize(transform);
        planetGenerator.Initialize(transform);
        moonGenerator.Initialize(transform);

        // Generate stars, spaced out along the X axis
        List<GameObject> stars = new List<GameObject>();
        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject star = starGenerator.GenerateStar(i * starGenerator.starSeparation * Constants.SCALE_FACTOR);
            if (star != null)
            {
                stars.Add(star);
            }
        }

        if (stars.Count == 0)
        {
            Debug.LogWarning("No star was generated; skipping planet generation.", this);
            return;
        }

        // Generate planets orbiting the first star, each with its own moons
        int planetCount = Mathf.Min(numberOfPlanets, maxNumberOfPlanets);
        for (int i = 0; i < planetCount; i++)
        {
            GameObject planet = planetGenerator.GeneratePlanet(i, stars[0]);
            if (planet == null)
            {
                continue;
            }

            int numberOfMoons = Random.Range(0, planetGenerator.maxNumberOfMoonsPerPlanet + 1);
            for (int j = 0; j < numberOfMoons; j++)
            {
                moonGenerator.GenerateMoon(planet, j);
            }
        }
    }
}
