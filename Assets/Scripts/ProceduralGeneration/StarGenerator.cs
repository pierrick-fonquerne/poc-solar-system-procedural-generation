using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// Generates stars in the solar system.
/// </summary>
[System.Serializable]
public class StarGenerator : CelestialObject
{
    public float minStarRadius = 10f;
    public float maxStarRadius = 15f;
    public int starSubdivisions = 3;

    [Tooltip("Distance between two consecutive stars, in unscaled units.")]
    public float starSeparation = 20f;

    [Tooltip("Point light intensity in lumens, per unit of star radius.")]
    public float lumensPerRadiusUnit = 2.5e7f;

    [ColorUsage(false, true)]
    public Color emissionColor = new Color(1f, 0.85f, 0.6f);

    [Tooltip("HDR intensity multiplier applied to the emissive color of the star material.")]
    public float emissionIntensity = 20f;

    /// <summary>
    /// Generates a star with a random radius and subdivisions and places it at a given distance.
    /// </summary>
    /// <param name="distance">The distance from the origin along the X axis.</param>
    /// <returns>The generated star GameObject, or null if creation failed.</returns>
    public GameObject GenerateStar(float distance)
    {
        // Generate a random radius for the star within the specified range
        float radius = Random.Range(minStarRadius, maxStarRadius);

        // Generate an icosphere with the given radius and subdivisions
        GameObject star = GenerateIcosphere(radius, starSubdivisions, "Star");
        if (star == null)
        {
            return null;
        }

        // Add a point light to the star to simulate its emission. HDRP uses
        // physical light units, so the intensity is set in lumens through
        // HDAdditionalLightData.
        Light light = star.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = Constants.AU_TO_UNITY_UNITS * Constants.SCALE_FACTOR * 10f;

        // The default HDRP unit for point lights is the lumen
        HDAdditionalLightData hdLightData = star.AddComponent<HDAdditionalLightData>();
        hdLightData.SetIntensity(lumensPerRadiusUnit * radius);

        // Make the star surface glow when the material supports HDRP emission
        MeshRenderer starRenderer = star.GetComponent<MeshRenderer>();
        if (starRenderer != null && starRenderer.material.HasProperty("_EmissiveColor"))
        {
            starRenderer.material.SetColor("_EmissiveColor", emissionColor * emissionIntensity);
        }

        // Set the position of the star and make it a child of the SolarSystemGenerator GameObject
        star.transform.localPosition = new Vector3(distance, 0, 0);

        // Kinematic rigidbody: the star does not move through physics, but its
        // mass drives the Kepler velocity of orbiting planets.
        Rigidbody rigidbody = star.AddComponent<Rigidbody>();
        rigidbody.mass = CalculateMass(radius);
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;

        star.AddComponent<GravityAttractor>();

        return star;
    }
}
