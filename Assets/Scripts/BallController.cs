using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] float maxPower = 20f;
    [SerializeField] float minimumSpeed = 0.05f;
    [SerializeField] float stopThreshold = 1f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] float lowestYPos = 10f;
    [SerializeField] private Vector3 collisionImpulse = new Vector3(5, 3, 5);

    [SerializeField] AudioClip hitGolfBallSound;

    public AudioClip holeSound;

    public Slider powerSlider;

    public AudioSource audioSource;

    TrailRenderer trailRenderer;
    Rigidbody rb;

    private LineRenderer lineRenderer;
    private Vector3 dragStartPos;
    private Vector3 currentMousePos;
    private float currentPower = 0f;

    private bool canInteract = false;
    private bool isCharging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();

        rb.drag = drag;
        rb.angularDrag = drag;

        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        powerSlider.value = 0f;

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        trailRenderer.enabled = false;

    }

    void Update()
    {
        if (rb.velocity.magnitude <= minimumSpeed && rb.angularVelocity.magnitude <= minimumSpeed)
        {
            canInteract = true;
            trailRenderer.enabled = false;

        }
        else
        {
            canInteract = false;
            trailRenderer.enabled = true;

        }

        if (rb.velocity.sqrMagnitude < stopThreshold * stopThreshold && rb.angularVelocity.sqrMagnitude < stopThreshold * stopThreshold)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (isCharging)
        {
            currentMousePos = GetMouseWorldPosition();
            DrawLine(dragStartPos, currentMousePos);

            currentPower = Vector3.Distance(dragStartPos, currentMousePos) * maxPower;
            currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
            powerSlider.value = currentPower;


            if (Input.GetMouseButtonUp(0))
            {
                Shoot(dragStartPos, currentMousePos);
                isCharging = false;
                powerSlider.value = 0f;
                lineRenderer.enabled = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                isCharging = false;
                currentPower = 0f;
                powerSlider.value = 0f;
                lineRenderer.enabled = false;
                Debug.Log("Shot canceled");
            }
        }

        if (transform.position.y < lowestYPos)
        {
            Restart();
        }
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && canInteract)
        {
            isCharging = true;
            currentPower = 0f;
            dragStartPos = GetMouseWorldPosition();
            lineRenderer.enabled = true;
            Debug.Log("Ball clicked!");
        }
        else
        {
            Debug.Log("Ball still moving");
        }
    }

    void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        lineRenderer.SetPosition(0, transform.position);
        Vector3 direction = endPos - startPos;
        lineRenderer.SetPosition(1, transform.position + direction);
    }

    void Shoot(Vector3 startPos, Vector3 endPos)
    {
        Vector3 forceDirection = startPos - endPos;
        forceDirection.Normalize();
        rb.AddForce(forceDirection * currentPower, ForceMode.Impulse);
        trailRenderer.enabled = true;
        audioSource.PlayOneShot(hitGolfBallSound);
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            rb.AddForce(collisionImpulse, ForceMode.Impulse);
        }
    }
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlaySound(AudioClip holeSound)
    {
        audioSource.PlayOneShot(holeSound);
    }
}

