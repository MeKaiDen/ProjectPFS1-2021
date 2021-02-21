using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private CharacterController cc;
    [SerializeField] private Transform cam;
    [SerializeField] private Camera thirdPersonCam;
    [SerializeField] private Transform itemGrabOffSet;
    
    
    [Header("Character Settings")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float grabRange = 10;


    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 0.5f;
    [SerializeField] private float dashSpeed = 12;
    [SerializeField] private float maxDashDistance = 100;
    [SerializeField] private float maxDashSpeed = 100;
    [SerializeField] private float additionalDashSpeed = 0.01f;
    [SerializeField] private float additionalDashDistance = 0.001f;
    
    
    [Header("JumpSettings")]
    [SerializeField] private float jumpHeight = 8;
    [SerializeField] private float maxJumpHeight = 8;
    [SerializeField]private float jumpSpeed =0;
    [SerializeField] private float additionalJumpHeight;

    private float initialDashDistance;
    private float initialDashSpeed;
    
    private float initialJumpHeight;

    private float gravity = 9.8f;
    private Vector3 direction;
    private float turnSmoothVelocity;
    [SerializeField]private bool firstJump;
    [SerializeField]private bool secondaryJump;
    private bool groundedPlayer;
    private bool dashing;
    private Vector3 moveDir;
    [SerializeField]private bool grab;
    [SerializeField]private GameObject itemGrabed;


    private void Awake()
    {
        grab = false;
        dashing = false;
        initialDashDistance = dashDistance;
        initialDashSpeed = dashSpeed;
        initialJumpHeight = jumpHeight;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        PlayerMovement();
        PrepareJump();
        Jump();
        GravitySetUp();
        PrepareDash();
        Grab();
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        direction = new Vector3(horizontal,0, vertical).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)* Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle,0);

            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            if (!dashing)
            {
                cc.Move(moveDir * speed * Time.deltaTime); 
            }
        }
    }

    void PrepareJump()
    {
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            initialJumpHeight = jumpHeight;
        }
        
        if (Input.GetButton("Jump") && groundedPlayer)
        {
            if (jumpHeight <= maxJumpHeight)
            {
                jumpHeight += additionalJumpHeight;
            }
        }
    }
    
    void Jump()
    {
        if (Input.GetButtonUp("Jump"))
        {
            if (firstJump)
            {
                jumpSpeed += Mathf.Sqrt(jumpHeight * 3 * gravity);
                firstJump = false;
            }
            else if (secondaryJump)
            {
                jumpSpeed += Mathf.Sqrt(initialJumpHeight * 3 * gravity);
                secondaryJump = false;
            }
        }

        if (!groundedPlayer)
        {
            jumpSpeed -= gravity * Time.deltaTime;
        }
        
        cc.Move(new Vector3(0, jumpSpeed * Time.deltaTime, 0));
    }

    private void PrepareDash()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !dashing)
        {
            if (dashDistance <= maxDashDistance)
            {
                dashDistance += additionalDashDistance;
            }
            if (dashSpeed <= maxDashSpeed)
            {
                dashSpeed += additionalDashSpeed;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift) && !dashing)
        {
            StartCoroutine("Dash");
        }
        
        if (dashing)
        {
            cc.Move(moveDir * speed * Time.deltaTime); 
        }
    }
    
    IEnumerator Dash()
    {
        dashing = true;
        float initialSpeed = speed;
        speed = dashSpeed;
        yield return new WaitForSeconds(dashDistance);
        speed = initialSpeed;
        dashDistance = initialDashDistance;
        dashSpeed = initialDashSpeed;
        dashing = false;
    }

    void GravitySetUp()
    {
        RaycastHit groundDetector;
        if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.down),out groundDetector,1))
        {
            Debug.Log("ground detected");
            groundedPlayer = true;
            jumpSpeed = 0;
            if (!firstJump)
            {
                jumpHeight = initialJumpHeight;  
            }
            firstJump = true;
            secondaryJump = true;
        }
        else
        {
            groundedPlayer = false;
        }
    }

    void Grab()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            if (!grab)
            {
                RaycastHit hitPoint;
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(mouseRay.origin,mouseRay.direction);
                if (Physics.Raycast(mouseRay, out hitPoint, grabRange))
                {
                    if (hitPoint.collider.GetComponent<InteractableObject>())
                    {
                        hitPoint.collider.gameObject.GetComponent<InteractableObject>().InitGrab(itemGrabOffSet);
                        itemGrabed = hitPoint.collider.gameObject;
                    } Debug.Log("hit :" + hitPoint.collider);
                }

                if (itemGrabed != null)
                {
                    grab = true;
                }
            }
            else if (grab)
            {
                itemGrabed.GetComponent<InteractableObject>().StopGrab();
                grab = false;
            }
        }
    }

}
