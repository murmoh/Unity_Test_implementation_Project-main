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
    private CharacterController controller;
    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;
    private Vector3 currentForceVelocity;
    public string Mode = "Survival";

    private void Start()
    {
        controller = GetComponent<CharacterController>();
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
        if(moveVector != Vector3.zero)
            controller.Move(currentMoveVelocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                currentForceVelocity.y = jumpStrength;
            }
        }


        else if(Mode == "Creative")
        {
            if(Input.GetKey(KeyCode.Space) && controller.isGrounded == false)
                {
                    currentForceVelocity.y += 1f;
                }
            if(Input.GetKey(KeyCode.LeftControl) && controller.isGrounded == false)
                {
                    currentForceVelocity.y -= 1f;
                }
        }

        else
        {
            currentForceVelocity.y -= gravityStrength * Time.deltaTime;
        }

        controller.Move(currentForceVelocity * Time.deltaTime);

    }

}