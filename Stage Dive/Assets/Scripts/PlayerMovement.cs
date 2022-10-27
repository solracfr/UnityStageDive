using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputAction movement;
    [SerializeField] InputAction brake;
    [SerializeField] InputAction jump;
    [SerializeField] bool isJumping;
    [SerializeField] bool isBraking;
    [SerializeField] InputAction fire;
    [SerializeField] bool isFiring;
    [SerializeField] bool isOnGround;


    public Rigidbody playerRB;
    public Transform cam;
    public float speedFactor;
    public float maxSpeed;
    public float turnSmoothTime;
    float turnSmoothVelocity;
    public float jumpForce;
    public float brakeValue;
    public float brakeFactor;
    public float distanceToGround;
    public float stopSpeedThreshold;
    float groundDetectionTolerance = 0.1f;

    // 
    void OnEnable()
    {
        movement.Enable();
        brake.Enable();
        jump.Enable();
        fire.Enable();
    }

    void OnDisable()
    {
        movement.Disable();
        brake.Disable();
        jump.Disable();
        fire.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        distanceToGround = GetComponent<BoxCollider>().bounds.extents.y;
    }

    void Update()
    {
        ProcessInput();
        isOnGround = IsGrounded();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessMovement();
        ProcessJump();
        ProcessBrake();
    }

    // Check for inputs using new Input System
    void ProcessInput()
    {
        if (jump.triggered && IsGrounded()) { isJumping = true; }
        if (fire.triggered) { isFiring = true; }
        brakeValue = brake.ReadValue<float>();
        if (brakeValue > 0.5f) { isBraking = true; } 
    }

    
    // Brake if grounded and not jumping
    private void ProcessBrake()
    {

        if (isBraking && IsGrounded() && !isJumping) 
        {
            if (playerRB.velocity.magnitude > stopSpeedThreshold) // braking while moving
            {
                playerRB.AddForce(-brakeValue * brakeFactor * playerRB.velocity);
            }
            else // if below a certain speed
            {
                playerRB.Sleep();
            }
        }

        
        isBraking = false;
    }

    // jump triggers when button is pressed
    void ProcessJump()
    {
        if (isJumping) 
        {
            isBraking = false;
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isJumping = false;
        } 
    }


    // Takes in player input to influnce lateral movement. Player prefab will look ahead and accelerate in the direction of the camera
    void ProcessMovement()
    {
        float xDir = movement.ReadValue<Vector2>().normalized.x;
        float yDir = movement.ReadValue<Vector2>().normalized.y;

        float targetAngle = Mathf.Atan2(xDir, yDir) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        if (IsGrounded() && playerRB.velocity.magnitude < maxSpeed && !isBraking) 
        {
            playerRB.AddForce(moveDirection * speedFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    // Performs a check to see if the player BoxCollider is touching the ground via Raycast
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + groundDetectionTolerance);
    }
}
