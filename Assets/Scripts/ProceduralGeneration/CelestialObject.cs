using UnityEngine;

/// <summary>
/// Base class for celestial objects (stars, planets, and moons).
/// </summary>
public abstract class CelestialObject
{
    protected Transform parentTransform;

    public CelestialObject(Transform parentTransform)
    {
        this.parentTransform = parentTransform;
    }

    /// <summary>
    /// Generates an icosphere with the given radius and subdivisions.
    /// </summary>
    /// <param name="radius">The radius of the generated icosphere.</param>
    /// <param name="subdivisions">The number of subdivisions for the generated icosphere.</param>
    /// <returns>The generated icosphere GameObject.</returns>
    protected GameObject GenerateIcosphere(float radius, int subdivisions)
    {
        IcoSphereGenerator icoSphereGenerator = new IcoSphereGenerator(radius, subdivisions);
        CelestialObjectGenerator celestialObjectGenerator = new CelestialObjectGenerator(icoSphereGenerator.Vertices, icoSphereGenerator.Triangles);

        GameObject icosphere = celestialObjectGenerator.GenerateObject();
        icosphere.transform.SetParent(parentTransform);

        return icosphere;
    }

    /// <summary>
    /// Calculates the mass of an object with the given radius and density.
    /// </summary>
    /// <param name="radius">The radius of the object.</param>
    /// <returns>The mass of the object in kilograms.</returns>
    protected float CalculateMass(float radius)
    {
        float density = 5500f; // kg/m^3 (approximate value for celestial objects)
        float volume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);

        return density * volume;
    }
}
