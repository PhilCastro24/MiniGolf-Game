using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] float maxPower = 100f;
    [SerializeField] float powerMultiplier = 0.1f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] float stopThreshold = 0.01f;
    [SerializeField] Slider powerSlider;

    private LineRenderer aimLine;
    private Vector3 forceDirection;
    [HideInInspector] public bool isCharging = false;
    private bool canInteract = false;

    private Rigidbody rb;

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
            forceDirection = Vector3.zero;
        }
        else if (Input.GetMouseButton(0) && isCharging)
        {
            Vector3 ballScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mouseScreenPos = Input.mousePosition;

            Vector3 direction = (ballScreenPos - mouseScreenPos).normalized;
            direction.z = 0;
            forceDirection = direction;

            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, transform.position + forceDirection);
        }
        else if (Input.GetMouseButtonUp(0) && isCharging)
        {
            rb.AddForce(forceDirection * maxPower * powerMultiplier, ForceMode.Impulse);
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

    public void CancelShot()
    {
        isCharging = false;
        aimLine.enabled = false;
        powerSlider.value = 0f;
    }

    public bool IsMoving()
    {
        return rb.velocity.sqrMagnitude > stopThreshold;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
