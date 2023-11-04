using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerMovement : NetworkBehaviour
{
    private Camera playerCamera;    // Player's camera
    public float moveSmoothTime = 0.1f;
    public float gravityStrength = 9.8f;
    public float jumpStrength = 2.5f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float dashSpeed = 30f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;

    private float lastDashTime;
    private bool isDashing;

    private CharacterController controller;
    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;
    private Vector3 currentForceVelocity;

    public GameObject VFX;
    public GameObject DashVFX;
    public GameObject GrappleVFX; // Visual effect for grapple
    private bool isGrapplingRushing = false; // Flag for rushing state of grapple


    public float grappleSpeed = 100.0f;
    public float grappleRange = 50f;
    public LayerMask grappleMask; // Assign a layer mask in Unity's Inspector to specify what the grapple can attach to.

    private bool isGrappling;

    private Vector3 grappleTarget;
    public float grappleCooldown = 5f; // Time it takes for the grapple to recharge, in seconds
    private float lastGrappleTime; // Time when the grapple was last used
    public float reducedGravityStrength = 3f; // Reduced gravity when grappling
    public float regularFOV = 60f;  // Regular field of view
    public float walkingFOV = 65f;  // FOV when walking forward or backward
    public float runFOV = 75f;  // FOV when walking forward or backward
    public float fovSmooth = 0.1f;  // New variable for FOV smoothness

    public float runningTiltAmount = 5f; // Degree to tilt camera while running
    public float runningTiltSpeed = 7f; // Speed to tilt camera while running

    private float initialCameraRotationX;
    private float targetCameraTilt;
    private float currentCameraTilt;

    public GameObject Rope;
    private List<GameObject> ropeLinks = new List<GameObject>();  // Store the rope links

    // Names of the sound effects
    public string runSound = "Run";
    public string walkSound = "Walk";
    public string jumpSound = "Jump";
    public string dashSound = "Dash";
    public string grappleSound = "Grapple";


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        lastDashTime = Time.time - dashCooldown; // Initialize so that the player can dash immediately
        playerCamera = Camera.main; // Assuming the main camera represents the player's view
        
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (isDashing) return; // Don't do anything else if dashing

        Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if (playerInput.magnitude > 1f)
        {
            playerInput.Normalize();
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
            PlaySound(dashSound);
        }

        Vector3 moveVector = transform.TransformDirection(playerInput);
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        currentMoveVelocity = Vector3.SmoothDamp(currentMoveVelocity, moveVector * currentSpeed, ref moveDampVelocity, moveSmoothTime);

        if (moveVector != Vector3.zero)
            controller.Move(currentMoveVelocity * Time.deltaTime);

        float forwardMovement = Vector3.Dot(controller.velocity, transform.forward);
        
        if (Mathf.Abs(forwardMovement) > 0.1f && !Input.GetKey(KeyCode.LeftShift))
        {
            // If walking forwards or backwards
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, walkingFOV, fovSmooth * Time.deltaTime);
            AudioManager.Instance.Stop(runSound);
            PlaySound(walkSound);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && playerInput.magnitude > 0.1f)
        {
            // If running
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, runFOV, fovSmooth * Time.deltaTime);
            AudioManager.Instance.Stop(walkSound);
            PlaySound(runSound);
        }
        else
        {
            // If not moving or walking sideways
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, regularFOV, fovSmooth * Time.deltaTime);
            AudioManager.Instance.Stop(walkSound);
            AudioManager.Instance.Stop(runSound);
        }

        if (Input.GetKey(KeyCode.LeftShift) && currentMoveVelocity.magnitude > 0.1f)
        {
            // Calculate the target tilt angle
            float sinWave = Mathf.Sin(Time.time * runningTiltSpeed);
            targetCameraTilt = sinWave * runningTiltAmount;
        }
        else
        {
            targetCameraTilt = 0f;
        }

        // Smoothly interpolate towards the target tilt
        currentCameraTilt = Mathf.Lerp(currentCameraTilt, targetCameraTilt, Time.deltaTime * runningTiltSpeed);

        // Apply the tilt to the camera
        Vector3 currentCameraRotation = playerCamera.transform.localEulerAngles;
        currentCameraRotation.x = initialCameraRotationX + currentCameraTilt;
        playerCamera.transform.localEulerAngles = currentCameraRotation;

        float appliedGravity = isGrappling ? reducedGravityStrength : gravityStrength;
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                currentForceVelocity.y = jumpStrength;
                PlaySound(jumpSound);
                AudioManager.Instance.Stop(walkSound);
                AudioManager.Instance.Stop(runSound);
            }
        }
        else
        {
            currentForceVelocity.y -= appliedGravity * Time.deltaTime;
            AudioManager.Instance.Stop(jumpSound);
        }

        controller.Move(currentForceVelocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.V) && Time.time >= lastGrappleTime + grappleCooldown)
        {
            TryGrapple();
            PlaySound(grappleSound);
        }

        if (isGrappling)
        {
            StartCoroutine(GrappleRush());
        }
    }

    IEnumerator Dash()
    {
        if (currentMoveVelocity == Vector3.zero) yield break; // No dashing if not moving

        isDashing = true;
        lastDashTime = Time.time;

        if (DashVFX != null)
        {
            DashVFX.SetActive(true);
        }
        else
        {
            
        }

        Vector3 dashDirection = currentMoveVelocity.normalized; // Normalize to get direction
        float dashStartTime = Time.time;

        while (Time.time < dashStartTime + dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        if (DashVFX != null)
        {
            Debug.Log("Deactivating Dash VFX");
            DashVFX.SetActive(false);
        }
        else
        {
            Debug.Log("Dash VFX not set");
        }

        isDashing = false;
    }

    void TryGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grappleRange, grappleMask))
        {
            isGrappling = true;
            grappleTarget = hit.point;
            lastGrappleTime = Time.time;

            // Create the rope when grappling
            CreateRope(hit.point);
        }
    }

    IEnumerator GrappleRush()
    {
        if (isGrapplingRushing)  // Prevent multiple instances
            yield break;

        isGrapplingRushing = true;

        Vector3 grappleDirection = (grappleTarget - transform.position).normalized; // Direction to grapple in
        float distanceToTarget = Vector3.Distance(transform.position, grappleTarget);  // Distance to grapple target

        while (distanceToTarget > 2f)
        {
            // Move the player towards the grapple target
            controller.Move(grappleDirection * grappleSpeed * Time.deltaTime);
            
            // Update grapple direction and distance after each movement
            grappleDirection = (grappleTarget - transform.position).normalized; 
            distanceToTarget = Vector3.Distance(transform.position, grappleTarget);
            DashVFX.SetActive(true);
            
            
            if(distanceToTarget <= 2f)
            {
                // Terminate the loop when close to the target
                DashVFX.SetActive(false);
                DestroyRope();
                break;
            }

            yield return null;
        }

        isGrappling = false;  // Reset the flag
        isGrapplingRushing = false;  // Reset the flag
        currentForceVelocity = Vector3.zero; // Reset the force velocity so that you can move again
    }

    void Grapple()
    {
        // Define a threshold for how close is "close enough"
        float arrivalThreshold = 0.5f;

        // Debug Line to visualize grapple point from player to target
        Debug.DrawLine(transform.position, grappleTarget, Color.red);

        if (isGrappling)
        {
            float step = grappleSpeed * Time.deltaTime;

            // Log the distance for debugging
            float distanceToTarget = Vector3.Distance(transform.position, grappleTarget);
            Debug.Log("Distance to Target: " + distanceToTarget);

            // Check if the GameObject is close enough to the target point
            if (distanceToTarget <= arrivalThreshold)
            {
                isGrappling = false;
                currentForceVelocity = Vector3.zero; // Reset any residual velocity
                transform.position = grappleTarget; // Snap to the target point
            }
            else
            {
                // Move towards the grapple point
                transform.position = Vector3.MoveTowards(transform.position, grappleTarget, step);
            }
        }
    }

    void CreateRope(Vector3 target)
    {
        DestroyRope();  // Destroy any existing rope
        float linkDistance = 1f;  // Distance between each link
        int numLinks = Mathf.FloorToInt(Vector3.Distance(transform.position, target) / linkDistance);

        Vector3 lastLinkPosition = transform.position;  // Starting from the player position

        for (int i = 0; i < numLinks; i++)
        {
            Vector3 linkPosition = Vector3.Lerp(transform.position, target, (float)(i + 1) / numLinks);

            // Calculate the direction vector between the last link and the new link
            Vector3 directionToNextLink = linkPosition - lastLinkPosition -  new Vector3(i,i,i);

            // Calculate the rotation based on the direction vector
            Quaternion linkRotation = Quaternion.LookRotation(directionToNextLink);

            GameObject newLink = Instantiate(Rope, linkPosition, linkRotation);
            ropeLinks.Add(newLink);

            // You could set newLink as a child of the previous link to make them connected
            if (i > 0)
            {
                newLink.transform.SetParent(ropeLinks[i - 1].transform);
            }

            lastLinkPosition = linkPosition;  // Update last link position
        }
    }


    void DestroyRope()
    {
        foreach (GameObject link in ropeLinks)
        {
            Destroy(link);
        }
        ropeLinks.Clear();
    }

    private void PlaySound(string soundName)
    {
        AudioManager.Instance.Play(soundName);
    }

}