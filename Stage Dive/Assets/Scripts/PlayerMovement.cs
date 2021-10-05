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

    public Rigidbody playerRB;
    public float speedFactor;
    public float jumpForce;


    void OnEnable() 
    {
        movement.Enable();   
        brake.Enable(); 
        jump.Enable();
    }

    void OnDisable() 
    {
        movement.Disable();
        brake.Disable();
        jump.Disable();
    }
  
    // Start is called before the first frame update
    void Start()
    {   
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessMovement();
        
    }

    private void ProcessBrake()
    {
        float brakeFactor = brake.ReadValue<float>();   
        playerRB.velocity += -playerRB.velocity * brakeFactor * Time.deltaTime; //TODO: Fine tune this
    }

    void Update()
    {
        ProcessJump();
        ProcessBrake();
    }

    // jump triggers when button is pressed
    void ProcessJump()
    {
        if (jump.triggered) 
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }


    void ProcessMovement()
    {
        float xMove = movement.ReadValue<Vector2>().x;
        float yMove = movement.ReadValue<Vector2>().y;

        playerRB.AddForce(Vector3.forward * speedFactor);
    }
}
