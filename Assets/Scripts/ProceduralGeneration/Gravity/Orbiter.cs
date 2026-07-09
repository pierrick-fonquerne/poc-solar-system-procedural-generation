using UnityEngine;

/// <summary>
/// Drives a kinematic circular orbit around a center of mass.
/// </summary>
public class Orbiter : MonoBehaviour
{
    public Transform centerOfMass;

    /// <summary>
    /// Angular velocity of the orbit in degrees per second.
    /// </summary>
    public float angularVelocity;

    /// <summary>
    /// Configures the orbit from a linear (tangential) velocity, converting it
    /// to an angular velocity based on the current distance to the center.
    /// </summary>
    /// <param name="center">The transform to orbit around.</param>
    /// <param name="linearVelocity">The tangential speed in units per second.</param>
    public void SetOrbit(Transform center, float linearVelocity)
    {
        centerOfMass = center;
        float distance = Vector3.Distance(transform.position, center.position);
        angularVelocity = distance > Mathf.Epsilon ? (linearVelocity / distance) * Mathf.Rad2Deg : 0f;
    }

    private void Update()
    {
        if (centerOfMass == null)
        {
            return;
        }

        transform.RotateAround(centerOfMass.position, Vector3.up, angularVelocity * Time.deltaTime);
    }
}
