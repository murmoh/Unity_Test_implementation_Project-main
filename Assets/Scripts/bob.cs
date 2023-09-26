using System;
using UnityEngine;

public class bob : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] public float smooth;
    [SerializeField] public float multiplier;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 10.0f; // Amount of recoil (adjust as needed)
    [SerializeField] private float recoilRecoveryRate = 80.0f; // Speed of recoil recovery (adjust as needed)

    [Header("Aiming Settings")]
    [SerializeField] private float aimFOV = 40.0f;  // Field of view while aiming
    [SerializeField] private float regularFOV = 60.0f;  // Regular field of view
    [SerializeField] private float aimSmooth = 10.0f;  // Smoothing factor for aiming transition
    [SerializeField] private Vector3 aimPosition;  // Local position of the camera/weapon when aiming
    [SerializeField] private Vector3 hipPosition;  // Local position of the camera/weapon at hip

    private Vector3 currentRecoil = Vector3.zero;
    [SerializeField] private Camera playerCamera; // Reference to the player's camera


    private void Start()
    {
        // Automatically find the main camera in the scene
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found! Ensure there's a camera with the 'MainCamera' tag in the scene.");
            return;
        }

        hipPosition = transform.localPosition;
    }

    private void Update()
    {

        Aim();

        if(Input.GetMouseButton(0))
        {
            recoil();
        }

        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        // apply recoil to mouse input
        mouseX += currentRecoil.x;
        mouseY += currentRecoil.y;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);

        // recover recoil over time
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoveryRate * Time.deltaTime);
    }

    public void recoil()
    {
        currentRecoil += new Vector3(
            UnityEngine.Random.Range(-recoilAmount, recoilAmount),
            UnityEngine.Random.Range(-recoilAmount, recoilAmount),
            0
        );
    }

    public void Aim()
    {
        if (Input.GetMouseButton(1))
        {
            // Interpolate the position and field of view to aim
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, aimSmooth * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, aimFOV, aimSmooth * Time.deltaTime);
            recoilAmount = 2f;
        }
        else
        {
            // Interpolate back to hip position and regular field of view
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipPosition, aimSmooth * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, regularFOV, aimSmooth * Time.deltaTime);
            recoilAmount = 10f;
        }
    }



}
