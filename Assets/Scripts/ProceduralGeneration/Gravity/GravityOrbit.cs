using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class applies gravitational forces to all <see cref="GravityAffectedObject"/> in the scene, causing them to orbit around each other.
/// </summary>
public class GravityOrbit : MonoBehaviour
{
    /// <summary>
    /// The list of all <see cref="GravityAffectedObject"/> in the scene.
    /// </summary>
    public List<GravityAffectedObject> affectedObjects = new List<GravityAffectedObject>();

    /// <summary>
    /// The time step used for the simulation.
    /// </summary>
    public float timeStep = 0.01f;

    /// <summary>
    /// Adds all <see cref="GravityAffectedObject"/> in the scene to the <see cref="affectedObjects"/> list.
    /// </summary>
    private void Start()
    {
        foreach (GravityAffectedObject affectedObject in FindObjectsOfType<GravityAffectedObject>())
        {
            affectedObjects.Add(affectedObject);
        }
    }

    /// <summary>
    /// Applies gravitational forces to all <see cref="GravityAffectedObject"/> in the <see cref="affectedObjects"/> list, causing them to orbit around each other.
    /// </summary>
    private void FixedUpdate()
    {
        for (int i = 0; i < affectedObjects.Count - 1; i++)
        {
            for (int j = i + 1; j < affectedObjects.Count; j++)
            {
                GravityAffectedObject obj1 = affectedObjects[i];
                GravityAffectedObject obj2 = affectedObjects[j];

                Vector3 direction = obj2.transform.position - obj1.transform.position;
                float distance = direction.magnitude;

                // Avoid extreme gravitational forces when objects are too close
                float minDistance = Mathf.Max(distance, 1f);

                float forceMagnitude = Constants.GRAVITATIONAL_CONSTANT * (obj1.mass * obj2.mass) / Mathf.Pow(minDistance, 2);
                Vector3 force = direction.normalized * forceMagnitude;

                obj1.AddForce(force);
                obj2.AddForce(-force);
            }
        }
    }
}