using UnityEngine;

/// <summary>
/// Generates stars in the solar system.
/// </summary>
public class StarGenerator : CelestialObject
{
    /// <summary>
    /// The minimum radius for stars.
    /// </summary>
    public float minStarRadius = 10f;

    /// <summary>
    /// The maximum radius for stars.
    /// </summary>
    public float maxStarRadius = 15f;

    /// <summary>
    /// The number of subdivisions for stars.
    /// </summary>
    public int starSubdivisions = 3;

    /// <summary>
    /// The distance for star orbits.
    /// </summary>
    public float starOrbitDistance = 20f;

    /// <summary>
    /// Initializes a new instance of the <see cref="StarGenerator"/> class.
    /// </summary>
    /// <param name="parentTransform">The parent transform for the star.</param>
    public StarGenerator(Transform parentTransform) : base(parentTransform)
    {
    }

    /// <summary>
    /// Generates a star with a random radius and subdivisions and places it at a given distance.
    /// </summary>
    /// <param name="distance">The distance from the origin.</param>
    /// <returns>The generated star GameObject.</returns>
    public GameObject GenerateStar(float distance)
    {
        float radius = Random.Range(minStarRadius, maxStarRadius);
        GameObject star = GenerateIcosphere(radius, starSubdivisions);

        MeshRenderer starRenderer = star.GetComponent<MeshRenderer>();
        star.AddComponent<Light>().type = LightType.Point;
        star.GetComponent<Light>().range = starOrbitDistance * 2f;
        star.GetComponent<Light>().intensity = 1.5f * radius;
        star.GetComponent<Light>().range = starOrbitDistance * Constants.SCALE_FACTOR * 2f;

        star.transform.parent = parentTransform;
        star.transform.localPosition = new Vector3(distance, 0, 0);
        Debug.Log($"Star generated at position : {star.transform.position}");

        star.AddComponent<Rigidbody>().mass = CalculateMass(radius);
        star.GetComponent<Rigidbody>().useGravity = false;
        star.AddComponent<GravityAttractor>();
        star.AddComponent<GravityAffectedObject>();

        return star;
    }
}
