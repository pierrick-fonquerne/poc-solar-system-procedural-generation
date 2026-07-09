using UnityEngine;

/// <summary>
/// Marks a body as a gravitational center and computes Kepler-based orbital
/// velocities for objects orbiting it. Requires a Rigidbody for its mass.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GravityAttractor : MonoBehaviour
{
    /// <summary>
    /// Calculates the circular orbital velocity for an object at a given distance,
    /// using v = sqrt(G * M / d).
    /// </summary>
    /// <param name="distance">The distance between the object and this attractor.</param>
    /// <returns>The orbital velocity in units per second.</returns>
    public float InitialOrbitalVelocity(float distance)
    {
        if (distance <= Mathf.Epsilon)
        {
            return 0f;
        }

        Rigidbody body = GetComponent<Rigidbody>();
        if (body == null)
        {
            Debug.LogWarning("GravityAttractor requires a Rigidbody to provide its mass.", this);
            return 0f;
        }

        return Mathf.Sqrt(Constants.GRAVITATIONAL_CONSTANT * body.mass / distance);
    }
}
