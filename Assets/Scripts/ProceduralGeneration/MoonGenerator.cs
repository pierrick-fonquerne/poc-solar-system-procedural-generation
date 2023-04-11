using UnityEngine;

/// <summary>
/// Generates moons around planets in the solar system.
/// </summary>
public class MoonGenerator : CelestialObject
{
    public float minMoonRadius = 0.5f;
    public float maxMoonRadius = 1f;
    public int moonSubdivisions = 2;
    public float moonOrbitDistance = 1f;
    public float rotationSpeedMultiplier = 1f;
    public float orbitalSpeedMultiplier = 5f;
    public float minRotationSpeed = 3f;
    public float maxRotationSpeed = 5f;

    public MoonGenerator(Transform parentTransform) : base(parentTransform)
    {
    }

    /// <summary>
    /// Generates a moon with a random radius and subdivisions and places it around a given planet at a specified index.
    /// </summary>
    /// <param name="planet">The planet GameObject around which the moon will be placed.</param>
    /// <param name="moonIndex">The index of the moon (starting from 0).</param>
    public void GenerateMoon(GameObject planet, int moonIndex)
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

        // Set the moon's rotation speed
        float rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * rotationSpeedMultiplier;
        moon.AddComponent<Rotator>();
        moon.GetComponent<Rotator>().angularVelocity = rotationSpeed;

        // Set the moon's orbital speed
        float distanceToPlanet = Vector3.Distance(moon.transform.position, planet.transform.position);
        float orbitalSpeed = planet.GetComponent<GravityAttractor>().InitialOrbitalVelocity(distanceToPlanet * Constants.SCALE_FACTOR);
        moon.AddComponent<Orbiter>();
        moon.GetComponent<Orbiter>().orbitalVelocity = orbitalSpeed;
        moon.GetComponent<Orbiter>().centerOfMass = planet.transform;
    }
}
