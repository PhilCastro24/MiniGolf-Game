using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] float maxPower = 100f;
    [SerializeField] float powerMultiplier = 1f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] Slider powerSlider;

    private LineRenderer aimLine;
    private Vector3 forceDirection;
    private float currentPower = 0f;
    private bool isCharging = false;
    private bool canInteract = false;
    private Plane aimPlane;
    private Rigidbody rb;

    public bool IsCharging => isCharging;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;

        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        powerSlider.value = 0f;

        aimLine = GetComponentInChildren<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.enabled = false;
    }

    void Update()
    {
        if (IsMoving())
        {
            canInteract = false;
            aimLine.enabled = false;
            isCharging = false;
            return;
        }
        else
        {
            canInteract = true;
        }

        if (Input.GetMouseButtonDown(0) && canInteract)
        {
            isCharging = true;
            aimLine.enabled = true;
            aimPlane = new Plane(Vector3.up, transform.position);
        }
        else if (Input.GetMouseButton(0) && isCharging)
        {
            UpdateAim();
        }
        else if (Input.GetMouseButtonUp(0) && isCharging)
        {
            ApplyForce();
            isCharging = false;
            aimLine.enabled = false;
            powerSlider.value = 0f;
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelShot();
        }

        if (transform.position.y < -10f)
        {
            Restart();
        }
    }

    private void UpdateAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (aimPlane.Raycast(ray, out float distance))
        {
            Vector3 aimPoint = ray.GetPoint(distance);
            forceDirection = (transform.position - aimPoint).normalized;

            float distanceToAim = Vector3.Distance(transform.position, aimPoint);
            currentPower = Mathf.Clamp(distanceToAim * powerMultiplier, 0, maxPower);
            powerSlider.value = currentPower;

            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, transform.position + forceDirection * currentPower / maxPower);
        }
    }

    private void ApplyForce()
    {
        rb.AddForce(forceDirection * currentPower, ForceMode.Impulse);
    }

    public void CancelShot()
    {
        isCharging = false;
        aimLine.enabled = false;
        powerSlider.value = 0f;
    }

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > 0.01f;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
