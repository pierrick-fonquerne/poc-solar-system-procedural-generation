using UnityEngine;

public class CameraController : MonoBehaviour
{
  public float moveSpeed = 5.0f; // Movement speed
    public float rotationSpeed = 120.0f; // Rotation speed
    public float yMinLimit = -80f; // Minimum vertical angle
    public float yMaxLimit = 80f; // Maximum vertical angle

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {
        // Rotate the camera using the mouse
        x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        transform.rotation = rotation;

        // Move the camera using the ZQSD keys
        Vector3 moveDirection = new Vector3();

        if (Input.GetKey(KeyCode.Z)) // Move forward
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) // Move backward
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.Q)) // Move left
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
        {
            moveDirection += transform.right;
        }

        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
    }

    // Clamp the angle between the min and max limits
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}