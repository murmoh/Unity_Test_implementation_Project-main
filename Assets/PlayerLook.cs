using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform PlayerCamera;
    public Transform weapon;
    public Vector2 Sensitivities;
    public float tiltAmount = 10f;
    public float tiltSpeed = 2f;

    private Vector2 XYRotation;
    private float currentTilt = 0f;
    private float targetTilt = 0f;
    private float velocityTilt = 0f;  // For SmoothDamp
    private PlayerMovement playerMovement;


    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement = GetComponentInParent<PlayerMovement>();  // Get the PlayerMovement script from the parent object
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

    }

    void Update()
    {
        // Mouse Look
        Vector2 MouseInput = new Vector2
        {
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y")
        };

        XYRotation.x -= MouseInput.y * Sensitivities.y;
        XYRotation.y += MouseInput.x * Sensitivities.x;

        XYRotation.x = Mathf.Clamp(XYRotation.x, -50f, 50f);

        transform.eulerAngles = new Vector3(0f, XYRotation.y, 0f);
        PlayerCamera.localEulerAngles = new Vector3(XYRotation.x, 0f, 0f);
        weapon.localEulerAngles = new Vector3(XYRotation.x, 0f, 0f);

        // Tilt based on movement
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? playerMovement.runSpeed : playerMovement.walkSpeed;
        float speedFactor = currentSpeed / playerMovement.runSpeed;

        if (moveDirection.magnitude > 0.1f)
        {
            targetTilt = Mathf.Clamp(moveDirection.x, -1f, 1f) * tiltAmount * speedFactor;
        }
        else
        {
            targetTilt = 0f;
        }

        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref velocityTilt, tiltSpeed * Time.deltaTime);

        PlayerCamera.localEulerAngles = new Vector3(PlayerCamera.localEulerAngles.x, PlayerCamera.localEulerAngles.y, -currentTilt);
        weapon.localEulerAngles = new Vector3(weapon.localEulerAngles.x, weapon.localEulerAngles.y, -currentTilt);
    }
}