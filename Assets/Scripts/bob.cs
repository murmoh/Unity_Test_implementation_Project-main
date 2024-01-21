using System;
using UnityEngine;
using Photon.Pun;

public class bob : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] public float smooth;
    [SerializeField] public float multiplier;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 10.0f;
    [SerializeField] private float recoilRecoveryRate = 80.0f;

    [Header("Aiming Settings")]
    [SerializeField] private float aimFOV = 40.0f;
    [SerializeField] private float regularFOV = 60.0f;
    [SerializeField] private float aimSmooth = 10.0f;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private Vector3 hipPosition;

    private Vector3 currentRecoil = Vector3.zero;
    private Camera playerCamera; // Reference to the player's camera
    private PhotonView view;

    private void Start()
    {
        view = GetComponentInParent<PhotonView>();

        if (view != null && view.IsMine)
        {
            playerCamera = Camera.main;
        }
        else
        {
            // Disable the camera component for non-local players
            Camera cameraComponent = GetComponent<Camera>();
            if (cameraComponent != null)
            {
               
            }
        }

        hipPosition = transform.localPosition;
    }

    void Update()
    {
        if (view != null && view.IsMine)
        {
            Aim();

            if (Input.GetMouseButton(0))
            {
                recoil();
            }

            float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

            mouseX += currentRecoil.x;
            mouseY += currentRecoil.y;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);

            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoveryRate * Time.deltaTime);
        }
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
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, aimSmooth * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, aimFOV, aimSmooth * Time.deltaTime);
            recoilAmount = 2f;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipPosition, aimSmooth * Time.deltaTime);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, regularFOV, aimSmooth * Time.deltaTime);
            recoilAmount = 10f;
        }
    }
}
