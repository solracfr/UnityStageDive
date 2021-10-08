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
    [SerializeField] InputAction fire;
    [SerializeField] bool isOnGround;


    public Rigidbody playerRB;
    public Transform cam;
    public float speedFactor;
    public float maxSpeed;
    public float turnSmoothTime;
    float turnSmoothVelocity;
    public float jumpForce;
    public float brakeFactor;
    public float distanceToGround;
    public float stopSpeedThreshold;
    float groundDetectionTolerance = 0.1f;

    //TODO: COMMENT EVERYTHINGGGG

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

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessMovement();

    }

    private void ProcessBrake()
    {
        float brakeInput = brake.ReadValue<float>();

        if (brakeInput > 0.5f && IsGrounded())
        {
            if (playerRB.velocity.magnitude > stopSpeedThreshold)
            {
                playerRB.velocity += -playerRB.velocity * brakeInput * brakeFactor * Time.deltaTime; //TODO: Fine tune this 
            }
            else
            {
                playerRB.velocity = Vector3.zero;
            }
        }

    }

    void Update()
    {
        ProcessJump();
        ProcessBrake();
        isOnGround = IsGrounded();
    }

    // jump triggers when button is pressed
    void ProcessJump()
    {
        if (jump.triggered)
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
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
        if (IsGrounded() && playerRB.velocity.magnitude < maxSpeed) playerRB.AddForce(moveDirection * speedFactor * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    // Performs a check to see if the player BoxCollider is touching the ground via Raycast
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + groundDetectionTolerance);
    }
}
