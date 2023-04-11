using UnityEngine;

/// <summary>
/// Controls the camera movement and rotation.
/// </summary>
public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float baseMoveSpeed = 5.0f;
    public float moveSpeedMultiplier = 1.0f;
    public float rotationSpeed = 120.0f;
    public float rollSpeed = 60.0f;
    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;
    public float mouseSensitivity = 1.0f; // Mouse sensitivity
    public float zoomSpeed = 5.0f; // Zoom speed

    private float x = 0.0f;
    private float y = 0.0f;
    private bool isCameraLocked = false; // Camera lock

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {
        // Toggle camera lock with the L key
        if (Input.GetKeyDown(KeyCode.L))
        {
            isCameraLocked = !isCameraLocked;
        }

        if (!isCameraLocked) // Only update camera rotation if it's not locked
        {
            // Rotate the camera using the mouse
            x += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime * mouseSensitivity;
            y -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime * mouseSensitivity;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;
        }

        // Calculate move speed based on distance from origin
        float distance = transform.position.magnitude;
        moveSpeed = baseMoveSpeed * (1.0f + distance * moveSpeedMultiplier);

        // Move the camera using the arrow keys
        Vector3 moveDirection = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow)) // Move forward
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Move backward
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) // Move left
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.RightArrow)) // Move right
        {
            moveDirection += transform.right;
        }

        // Move the camera vertically using the Space and Ctrl keys
        if (Input.GetKey(KeyCode.Space)) // Move up
        {
            moveDirection += transform.up;
        }
        if (Input.GetKey(KeyCode.LeftControl)) // Move down
        {
            moveDirection -= transform.up;
        }

        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

        // Roll the camera using the A and E keys
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward, rollSpeed * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.forward, -rollSpeed * Time.deltaTime, Space.Self);
        }

        // Zoom in and out with the mouse wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0.0f)
        {
            float zoomAmount = scrollInput * zoomSpeed * Time.deltaTime;
            transform.position += transform.forward * zoomAmount;
        }
    }

    /// <summary>
    /// Clamp the angle between the min and max limits.
    /// </summary>
    /// <param name="angle">The angle to be clamped.</param>
    /// <param name="min">The minimum limit of the angle.</param>
    /// <param name="max">The maximum limit of the angle.</param>
    /// <returns>Returns the clamped angle value.</returns>
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
