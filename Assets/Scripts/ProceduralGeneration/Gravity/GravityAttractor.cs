using UnityEngine;

/// <summary>
/// Attracts GravityAffectedObjects towards this object using gravitational force.
/// </summary>
public class GravityAttractor : MonoBehaviour
{
    /// <summary>
    /// Attracts the given GravityAffectedObject towards this object.
    /// </summary>
    /// <param name="affectedObject">The GravityAffectedObject to attract.</param>
    public void Attract(GravityAffectedObject affectedObject)
    {
        Vector3 direction = transform.position - affectedObject.transform.position;
        float distance = direction.magnitude;
        float forceMagnitude = Constants.GRAVITATIONAL_CONSTANT * (affectedObject.mass * GetComponent<Rigidbody>().mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;
        affectedObject.AddForce(force);
    }
    
    /// <summary>
    /// Calculates the initial orbital velocity for an object at a given distance.
    /// </summary>
    /// <param name="distance">The distance between the object and the attractor.</param>
    /// <returns>The initial orbital velocity.</returns>
    public float InitialOrbitalVelocity(float distance)
    {
        float starMass = GetComponent<Rigidbody>().mass;
        return Mathf.Sqrt(Constants.GRAVITATIONAL_CONSTANT * starMass / distance);
    }
}
