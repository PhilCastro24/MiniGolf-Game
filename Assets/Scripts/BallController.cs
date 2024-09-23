using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Added for IPointerDownHandler

public class BallController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public Rigidbody rb;
    [SerializeField] private float force = 5f;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private LineRenderer aimLine;

    private int shootCount;
    private bool shoot;
    private bool shootingMode;
    private float forceFactor;
    private Vector3 forceDirection;
    private Ray ray;
    private Plane plane;

    public bool ShootingMode => shootingMode;
    public int ShootCount => shootCount;

    void Start()
    {
        aimLine.gameObject.SetActive(false);

        // Initialize Rigidbody and physics properties
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;
    }

    void Update()
    {
        if (shootingMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                aimLine.gameObject.SetActive(true);
                plane = new Plane(Vector3.up, transform.position);
            }
            else if (Input.GetMouseButton(0))
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
            }
            else if (Input.GetMouseButtonUp(0))
            {
                shoot = true;
                shootingMode = false;
                aimLine.gameObject.SetActive(false);
            }

            // Cancel shot if right-clicked
            if (Input.GetMouseButtonDown(1))
            {
                shootingMode = false;
                aimLine.gameObject.SetActive(false);
            }
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
    }

    public void Penalty()
    {
        shootCount++;
    }

    public void CancelShot()
    {
        shootingMode = false;
        aimLine.gameObject.SetActive(false);
    }

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > 0.01f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsMoving())
            return;

        shootingMode = true;
    }
}
