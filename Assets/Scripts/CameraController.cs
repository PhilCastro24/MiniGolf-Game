using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float rotationSpeed = 150f;

    private Transform cameraTransform;
    private Transform playerTransform;

    void Start()
    {
        cameraTransform = virtualCamera.transform;
        playerTransform = virtualCamera.Follow;
    }

    void Update()
    {
        float rotationInput = 0f;

        // Mouse drag rotation (Left Mouse Button)
        if (Input.GetMouseButton(0)) // Left mouse button is held down
        {
            rotationInput += Input.GetAxis("Mouse X");
        }

        // Arrow keys rotation
        float arrowInput = Input.GetAxis("Horizontal"); // Left/Right Arrow keys or A/D keys
        rotationInput += arrowInput;

        // Rotate around the Y-axis
        if (Mathf.Abs(rotationInput) > 0.01f)
        {
            cameraTransform.RotateAround(playerTransform.position, Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
        }
    }
}
