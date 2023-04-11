using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float angularVelocity;

    private void Update()
    {
        transform.Rotate(Vector3.up, angularVelocity * Time.deltaTime);
    }
}
