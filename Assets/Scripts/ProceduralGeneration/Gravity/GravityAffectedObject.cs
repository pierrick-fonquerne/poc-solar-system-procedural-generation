using UnityEngine;

public class GravityAffectedObject : MonoBehaviour
{
    /// <summary>
    /// The mass of the object.
    /// </summary>
    public float mass;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mass = rb.mass;
    }

    /// <summary>
    /// Adds a force to the object.
    /// </summary>
    /// <param name="force">The force to add.</param>
    public void AddForce(Vector3 force)
    {
        rb.AddForce(force);
    }
}
