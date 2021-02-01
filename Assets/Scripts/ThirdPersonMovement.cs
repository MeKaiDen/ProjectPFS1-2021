using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private CharacterController cc;
    [SerializeField] private Transform cam;
    
    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private bool groundedPlayer;
    [SerializeField] private float jumpHeight = 8;


    [SerializeField]private float gravity = 9.8f;
    private float turnSmoothVelocity;
    [SerializeField]private float verticalSpeed =0;
    
    
    private void Update()
    {
        PlayerMovement();

        
    }

    private void FixedUpdate()
    {
        //Gestion de la gravité
        RaycastHit groundDetector;
        if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.down),out groundDetector,1))
        {
            Debug.Log("ground detected");
            groundedPlayer = true;
            verticalSpeed = 0;
        }
        else
        {
            groundedPlayer = false;
        }

        if (Input.GetButton("Jump") && groundedPlayer)
        {
            verticalSpeed += Mathf.Sqrt(jumpHeight * 3 * gravity);
            Debug.Log("jump " + verticalSpeed);
        }

        if (!groundedPlayer)
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }
        
        cc.Move(new Vector3(0, verticalSpeed * Time.deltaTime, 0));
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 direction = new Vector3(horizontal,0, vertical).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)* Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle,0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cc.Move(moveDir * speed * Time.deltaTime); 
        }
        
        
    }
}
