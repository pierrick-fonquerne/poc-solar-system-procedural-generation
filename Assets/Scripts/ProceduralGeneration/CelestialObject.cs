using System;
using UnityEngine;

/// <summary>
/// Base class for celestial object generators (stars, planets, and moons).
/// Serializable so generator settings can be edited in the Inspector.
/// </summary>
[Serializable]
public abstract class CelestialObject
{
    [NonSerialized]
    protected Transform parentTransform;

    /// <summary>
    /// Sets the transform under which generated objects are parented.
    /// Must be called before generating any object.
    /// </summary>
    /// <param name="parent">The parent transform for generated objects.</param>
    public void Initialize(Transform parent)
    {
        parentTransform = parent;
    }

    /// <summary>
    /// Generates an icosphere GameObject with the given radius and subdivisions.
    /// </summary>
    /// <param name="radius">The radius of the generated icosphere.</param>
    /// <param name="subdivisions">The number of subdivisions for the generated icosphere.</param>
    /// <param name="name">The name of the generated GameObject.</param>
    /// <returns>The generated icosphere GameObject, or null if creation failed.</returns>
    protected GameObject GenerateIcosphere(float radius, int subdivisions, string name = "Celestial Object")
    {
        IcoSphereGenerator icoSphereGenerator = new IcoSphereGenerator(radius, subdivisions);
        GameObject icosphere = CelestialObjectFactory.CreateObject(icoSphereGenerator.BuildMesh(name), name);

        if (icosphere != null)
        {
            icosphere.transform.SetParent(parentTransform);
        }

        return icosphere;
    }

    /// <summary>
    /// Calculates the mass of a spherical object with the given radius.
    /// </summary>
    /// <param name="radius">The radius of the object.</param>
    /// <returns>The mass of the object.</returns>
    public static float CalculateMass(float radius)
    {
        float density = 5500f; // approximate rocky-body density
        float volume = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);

        return density * volume;
    }
}
