using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public Transform centerOfMass;
    public float orbitalVelocity;

    private void Update()
    {
        transform.RotateAround(centerOfMass.position, Vector3.up, orbitalVelocity * Time.deltaTime);
    }
}
