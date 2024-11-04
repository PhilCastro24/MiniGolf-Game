using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField] float maxPower = 20f;
    [SerializeField] float maxLineLength = 5f;
    [SerializeField] float minimumSpeed = 0.005f;
    [SerializeField] float stopThreshold = 0.005f;
    [SerializeField] float drag = 0.2f;
    [SerializeField] float lowestYPos = 10f;
    [SerializeField] float delayBeforeLoad = 1f;
    [SerializeField] private Vector3 collisionImpulse = new Vector3(5, 3, 5);

    [SerializeField] float jumpForceUp = 5f;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI shotsLeftText;
    [SerializeField] AudioClip hitGolfBallSound;

    public AudioClip holeSound;
    public AudioSource audioSource;

    public Slider powerSlider;
    [HideInInspector]
    public bool isCharging = false;

    TrailRenderer trailRenderer;
    Rigidbody rb;

    private LineRenderer lineRenderer;
    private Vector3 dragStartPos;
    private Vector3 currentMousePos;
    private float currentPower = 0f;
    int totalShots = 2; //Change back to 7

    private bool canInteract = false;
    private bool gameOvertriggered = false;


    SceneController sceneController;
    TimerController timerController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        audioSource = GetComponent<AudioSource>();
        sceneController = FindObjectOfType<SceneController>();
        timerController = FindObjectOfType<TimerController>();


        rb.drag = drag;
        rb.angularDrag = drag;

        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        powerSlider.value = 0f;

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        trailRenderer.enabled = false;

        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        UpdateShots();

        if (rb.velocity.magnitude <= minimumSpeed && rb.angularVelocity.magnitude <= minimumSpeed)
        {
            if (!canInteract) Debug.Log("Ball has come to rest");
            canInteract = true;
            trailRenderer.enabled = false;
        }
        else
        {
            if (canInteract) Debug.Log("Ball is moving");
            canInteract = false;
            trailRenderer.enabled = true;
        }

        if (transform.position.y <= lowestYPos)
        {
            Invoke("RestartScene", delayBeforeLoad);
        }

        /*if (rb.velocity.sqrMagnitude < stopThreshold * stopThreshold && rb.angularVelocity.sqrMagnitude < stopThreshold * stopThreshold)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }*/

        if (isCharging)
        {
            currentMousePos = GetMouseWorldPosition();
            DrawLine(dragStartPos, currentMousePos);

            float distance = Vector3.Distance(dragStartPos, currentMousePos);

            if (distance > maxLineLength)
            {
                distance = maxLineLength;
            }

            currentPower = (distance / maxLineLength) * maxPower;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("JumpObstacle"))
        {
            if (rb != null)
            {
                Vector3 jumpDirection = Vector3.up * jumpForceUp;
                rb.AddForce(jumpDirection, ForceMode.Impulse);
            }
        }
    }
    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && canInteract && totalShots > 0)
        {
            isCharging = true;
            currentPower = 0f;
            dragStartPos = GetMouseWorldPosition();
            lineRenderer.enabled = true;
            Debug.Log("Ball clicked!");
        }
        else
        {
            Debug.Log("Cannot interact with the ball");
        }
    }

    void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        Vector3 linePosition = new Vector3(
            transform.position.x, transform.position.y, transform.position.z);

        lineRenderer.SetPosition(0, linePosition);
        Vector3 direction = endPos - startPos;

        if (direction.magnitude > maxLineLength)
        {
            direction = direction.normalized * maxLineLength;
        }

        lineRenderer.SetPosition(1, transform.position + direction);
    }

    void Shoot(Vector3 startPos, Vector3 endPos)
    {
        Vector3 forceDirection = startPos - endPos;
        forceDirection.Normalize();
        rb.AddForce(forceDirection * currentPower, ForceMode.Impulse);
        trailRenderer.enabled = true;
        audioSource.PlayOneShot(hitGolfBallSound);
        canInteract = false;

        totalShots--;
        Debug.Log("Total Shots: " + totalShots);
        if (totalShots == 1)
        {
            Debug.Log("One Shot left");
        }
        if (totalShots <= 0 && !gameOvertriggered)
        {
            gameOvertriggered = true;
            isCharging = false;
            canInteract = false;

            StartCoroutine(WaitForBallToStop());
        }
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

    public void PlaySound(AudioClip holeSound)
    {
        audioSource.PlayOneShot(holeSound);
    }

    void UpdateShots()
    {
        shotsLeftText.text = "Shots left: " + totalShots;
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        sceneController.Restart();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1;
        sceneController.BackToMenu();
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;

        if (timerController != null)
        {
            timerController.StopTimer();
        }
    }
    IEnumerator WaitForBallToStop()
    {
        float startMovingSpeed = 0.1f;

        Debug.Log("Waiting for ball to stop moving...");
        while (rb.velocity.magnitude < startMovingSpeed && rb.angularVelocity.magnitude < startMovingSpeed)
        {
            yield return null; //Waits for the next frame
        }
        Debug.Log("Ball has started Moving.");
        Debug.Log("Waiting for ball to stop moving...");

        while (rb.velocity.magnitude > minimumSpeed || rb.angularVelocity.magnitude > minimumSpeed)
        {
            yield return null;
        }
        Debug.Log("Ball has stopped moving, showing Game Over Panel");

        ShowGameOverPanel();
    }
}

