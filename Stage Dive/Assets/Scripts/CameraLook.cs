using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class CameraLook : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook _freeLookComponent;
    [SerializeField] InputAction camLook;
    [SerializeField] PlayerMovement playerMovementScript;
    public float lookSpeed;

    void OnEnable() 
    {
        camLook.Enable();
    }

    void OnDisable()
    {
        camLook.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        _freeLookComponent = GetComponent<CinemachineFreeLook>();
        playerMovementScript = gameObject.transform.parent.gameObject.GetComponentInChildren<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessCameraLook();
    }

    void ProcessCameraLook()
    {
        float xCam = camLook.ReadValue<Vector2>().normalized.x * 180f;
        float yCam = camLook.ReadValue<Vector2>().normalized.y;

        Debug.Log($"X: {xCam} / Y: {yCam}");

        _freeLookComponent.m_XAxis.Value += xCam * lookSpeed * Time.deltaTime;
        _freeLookComponent.m_YAxis.Value += yCam * lookSpeed * Time.deltaTime;
    }
}
