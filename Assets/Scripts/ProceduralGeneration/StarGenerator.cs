using UnityEngine;

/// <summary>
/// Generates stars in the solar system.
/// </summary>
public class StarGenerator : CelestialObject
{
    public float minStarRadius = 5f;
    public float maxStarRadius = 10f;
    public int starSubdivisions = 3;
    public float starOrbitDistance = 20f;

    public StarGenerator(Transform parentTransform) : base(parentTransform)
    {
    }

    /// <summary>
    /// Generates a star with a random radius and subdivisions and places it at a given distance.
    /// </summary>
    /// <param name="distance">The distance from the origin.</param>
    public void GenerateStar(float distance)
    {
        // Generate a random radius for the star within the specified range
        float radius = Random.Range(minStarRadius, maxStarRadius);

        // Generate an icosphere with the given radius and subdivisions
        GameObject star = GenerateIcosphere(radius, starSubdivisions);

        // Set the material of the star
        MeshRenderer starRenderer = star.GetComponent<MeshRenderer>();

        // Add a point light to the star to simulate its emission
        star.AddComponent<Light>();
        star.GetComponent<Light>().type = LightType.Point;
        star.GetComponent<Light>().range = starOrbitDistance * 2f;
        star.GetComponent<Light>().intensity = 1.5f * radius; // Set intensity based on the star's radius
        star.GetComponent<Light>().range = starOrbitDistance * Constants.SCALE_FACTOR * 2f;

        // Set the position of the star and make it a child of the SolarSystemGenerator GameObject
        star.transform.parent = parentTransform;
        star.transform.localPosition = new Vector3(distance, 0, 0);
        Debug.Log($"Star generated at position : {star.transform.position}");

        // Add a rigidbody to the star to enable gravity calculations
        star.AddComponent<Rigidbody>();
        star.GetComponent<Rigidbody>().mass = CalculateMass(radius);
        star.GetComponent<Rigidbody>().useGravity = false;

        // Add a GravityAttractor component to the star to make it attract other objects
        star.AddComponent<GravityAttractor>();

        // Add a GravityAffectedObject component to the star to make it affected by gravity
        star.AddComponent<GravityAffectedObject>();
    }
}