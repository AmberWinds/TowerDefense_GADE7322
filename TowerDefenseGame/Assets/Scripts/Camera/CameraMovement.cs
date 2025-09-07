using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float sprintMultiplier = 2f;   // hold LeftShift to sprint
    public bool moveInLocalSpace = true;  // if false, WASD uses world axes

    [Header("Look (RMB drag)")]
    public float lookSensitivity = 2.0f;  // degrees per mouse delta unit
    public float maxPitch = 85f;          // clamp vertical look
    public bool invertY = false;

    private float yaw;
    private float pitch;

    void OnEnable()
    {
        // Mouse is always Visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Initialize yaw/pitch from current rotation
        var e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = NormalizePitch(e.x);
    }

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        if (!Input.GetMouseButton(1)) return; // look only while RMB held

        // Using classic Input Manager axes ("Mouse X/Y")
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * lookSensitivity;
        float ySign = invertY ? 1f : -1f;
        pitch += my * lookSensitivity * ySign;

        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMove()
    {
        // WASD from the default Input Manager axes
        float x = Input.GetAxisRaw("Horizontal"); // A/D
        float z = Input.GetAxisRaw("Vertical");   // W/S

        // Q/E for vertical
        float y = 0f;
        if (Input.GetKey(KeyCode.E)) y += 1f;
        if (Input.GetKey(KeyCode.Q)) y -= 1f;

        Vector3 dir = new Vector3(x, y, z);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector3 delta = dir * speed * Time.deltaTime;

        if (moveInLocalSpace)
            transform.Translate(delta, Space.Self);
        else
            transform.Translate(delta, Space.World);
    }

    static float NormalizePitch(float xEuler)
    {
        // Convert 0..360 to -180..180, then clamp later
        if (xEuler > 180f) xEuler -= 360f;
        return xEuler;
    }
}
