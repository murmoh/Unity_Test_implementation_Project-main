using System;
using UnityEngine;

public class bob : MonoBehaviour {

    [Header("Sway Settings")]
    [SerializeField] public float smooth;
    [SerializeField] public float multiplier;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAmount = 10.0f; // Amount of recoil (adjust as needed)
    [SerializeField] private float recoilRecoveryRate = 80.0f; // Speed of recoil recovery (adjust as needed)

    private Vector3 currentRecoil = Vector3.zero;

    private void Update()
    {
        // Check for left click
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            recoilAmount = 2f;
            recoilRecoveryRate = 70f;
            recoil();
        }
        else if(Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            recoilAmount = 10f;
            recoilRecoveryRate = 80f;
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
}
