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
    public BallController ballController;

    void Start()
    {
        cameraTransform = virtualCamera.transform;
        playerTransform = virtualCamera.Follow;

        if (ballController == null)
        {
            ballController = FindObjectOfType<BallController>();
        }
    }

    void Update()
    {
        if (ballController != null && (ballController.IsMoving() || ballController.isCharging))
        {
            return;
        }

        float rotationInput = 0f;

        if (Input.GetMouseButton(2))
        {
            rotationInput += Input.GetAxis("Mouse X");
        }

        float arrowInput = Input.GetAxis("Horizontal");
        rotationInput += arrowInput;

        if (Mathf.Abs(rotationInput) > 0.01f)
        {
            cameraTransform.RotateAround(playerTransform.position, Vector3.up, rotationInput * rotationSpeed * Time.deltaTime);
        }
    }
}
