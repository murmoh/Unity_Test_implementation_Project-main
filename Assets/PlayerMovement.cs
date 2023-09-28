using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSmoothTime = 0.1f;
    public float gravityStrength = 9.8f;
    public float jumpStrength = 2.5f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float climbSpeed = 5f;
    public float wallCheckDistance = 2f;

    [Header("FOV Settings")]
    public float regularFOV = 60f;  // Regular field of view
    public float runFOV = 70f;      // FOV when running
    public float fovSmooth = 5f;    // Smoothing factor for FOV transition

    private Camera playerCamera;    // Player's camera

    private CharacterController controller;
    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;
    private Vector3 currentForceVelocity;

    public GameObject VFX;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main; // Assuming the main camera represents the player's view
    }

    private void Update()
    {
        Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (playerInput.magnitude > 1f)
        {
            playerInput.Normalize();
        }

        Vector3 moveVector = transform.TransformDirection(playerInput);
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        currentMoveVelocity = Vector3.SmoothDamp(currentMoveVelocity, moveVector * currentSpeed, ref moveDampVelocity, moveSmoothTime);
        if (moveVector != Vector3.zero)
            controller.Move(currentMoveVelocity * Time.deltaTime);

        // Modify the FOV when running
        if (Input.GetKey(KeyCode.LeftShift) && playerInput.magnitude > 0.1f)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, runFOV, fovSmooth * Time.deltaTime);
            VFX.SetActive(true);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, regularFOV, fovSmooth * Time.deltaTime);
            VFX.SetActive(false);
        }

        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                currentForceVelocity.y = jumpStrength;
            }
        }
        else
        {
            currentForceVelocity.y -= gravityStrength * Time.deltaTime;
        }

        controller.Move(currentForceVelocity * Time.deltaTime);
    }
}
