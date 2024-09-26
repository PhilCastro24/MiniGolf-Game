using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI elements

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float force = 5f;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] Slider powerSlider;

    private int shootCount;
    private bool shoot;
    private bool shootingMode;
    private float forceFactor;
    private Vector3 forceDirection;
    private Ray ray;
    private Plane plane;
    private bool wasMoving = false;

    public bool ShootingMode => shootingMode;
    public int ShootCount => shootCount;

    void Start()
    {
        // Initialize UI elements
        aimLine.gameObject.SetActive(false);

        // Initialize Rigidbody and physics properties
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;
    }

    void Update()
    {
        if (IsMoving())
        {
            // Disable aiming mechanics
            aimLine.gameObject.SetActive(false);
            shootingMode = false; // Ensure shootingMode is false when ball is moving
            return; // Prevent further input handling
        }

        if (Input.GetMouseButtonDown(0))
        {
            shootingMode = true; // Start aiming
            aimLine.gameObject.SetActive(true);
            plane = new Plane(Vector3.up, transform.position);
        }
        else if (Input.GetMouseButton(0) && shootingMode)
        {
            // Calculate force direction
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
            {
                forceDirection = (transform.position - ray.GetPoint(distance)).normalized;
            }

            // Calculate force factor
            Vector3 mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 ballViewportPos = Camera.main.WorldToViewportPoint(transform.position);
            Vector3 pointerDirection = ballViewportPos - mouseViewportPos;
            pointerDirection.z = 0;
            forceFactor = pointerDirection.magnitude * 2;

            // Update aim visuals
            Vector3 ballScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mouseScreenPos = Input.mousePosition;
            ballScreenPos.z = 1f;
            mouseScreenPos.z = 1f;
            Vector3[] positions = {
                Camera.main.ScreenToWorldPoint(ballScreenPos),
                Camera.main.ScreenToWorldPoint(mouseScreenPos)
            };
            aimLine.SetPositions(positions);
            aimLine.endColor = Color.Lerp(Color.green, Color.red, forceFactor);
            powerSlider.value = forceFactor;
        }
        else if (Input.GetMouseButtonUp(0) && shootingMode)
        {
            shoot = true;
            shootingMode = false; // Stop aiming
            aimLine.gameObject.SetActive(false);
        }

        // Cancel shot if right-clicked
        if (Input.GetMouseButtonDown(1))
        {
            CancelShot();
        }
    }

    void FixedUpdate()
    {
        if (shoot)
        {
            shoot = false;
            rb.AddForce(forceDirection * force * forceFactor, ForceMode.Impulse);
            shootCount++;
        }

        // Stop the ball if it's nearly stationary
        if (rb.velocity.sqrMagnitude < 0.01f && rb.velocity.sqrMagnitude != 0)
        {
            rb.velocity = Vector3.zero;
        }

        // Detect when the ball has just stopped moving
        if (wasMoving && !IsMoving())
        {
            // Re-set the value of the slider back to 0
            powerSlider.value = 0;
        }

        // Update wasMoving for the next frame
        wasMoving = IsMoving();
    }

    public void Penalty()
    {
        shootCount++;
        //
    }

    public void CancelShot()
    {
        shootingMode = false;
        aimLine.gameObject.SetActive(false);
        powerSlider.value = 0; // Reset the power slider
    }

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > 0.01f;
    }
}
