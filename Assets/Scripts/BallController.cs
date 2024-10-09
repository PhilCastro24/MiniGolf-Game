using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    Rigidbody rb;

    private bool canInteract = false;
    private bool isCharging = false;

    [SerializeField] float maxPower = 100f;
    [SerializeField] float currentPower = 0f;
    [SerializeField] float powerMultiplier = 0.1f;
    [SerializeField] float forceMultiplier = 1f;

    public Slider powerSlider;

    [SerializeField] float minimumSpeed = 0.05f;
    [SerializeField] float stopThreshold = 1f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] float lowestYPos = 10f;
    [SerializeField] private Vector3 collisionImpulse = new Vector3(5, 3, 5);
    [SerializeField] Slider powerSlider;

    private bool canInteract = false;
    private bool isCharging = false;

    private LineRenderer lineRenderer;

    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;

        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        powerSlider.value = 0f;

        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        //if velocity and angular drag is under or equal to minSpeed then...
        if (rb.velocity.magnitude <= minimumSpeed && rb.angularVelocity.magnitude <= minimumSpeed)
        {
            //you can interact with the ball
            canInteract = true;
        }
        else
        {
            //othwerwise, if its higher then minSpeed. you cannot interact with the ball
            canInteract = false;
        }

        //if the ball reaches a certain speed and angular drag then...
        if (rb.velocity.sqrMagnitude < stopThreshold * stopThreshold && rb.angularVelocity.sqrMagnitude < stopThreshold * stopThreshold)
        {
            //velocity equals 0. Means it comes to a full stop
            rb.velocity = Vector3.zero;
            //same goes for the angular drag
            rb.angularVelocity = Vector3.zero;
        }

        if (isCharging && canInteract)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 dragVector = currentMousePos - mousePressDownPos;

            currentPower = dragVector.magnitude * powerMultiplier;
            currentPower = Mathf.Clamp(currentPower, 0, maxPower);

            powerSlider.value = currentPower;

            Vector3 ballPosition = transform.position;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y - ballPosition.y));

            Vector3 direction = mouseWorldPosition - ballPosition;
            direction.y = 0;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, ballPosition);
            lineRenderer.SetPosition(1, ballPosition + direction);
        }
        else
        {
            lineRenderer.enabled = false;
        }

        if (Input.GetMouseButtonUp(0) && isCharging && canInteract)
        {
            mouseReleasePos = Input.mousePosition;

            Vector3 mousePressWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(mousePressDownPos.x, mousePressDownPos.y, Camera.main.nearClipPlane));

            Vector3 mouseReleaseWorldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(mouseReleasePos.x, mouseReleasePos.y, Camera.main.nearClipPlane));


            Vector3 forceVector = mousePressDownPos - mouseReleasePos;
            forceVector.y = 0f;

            Vector3 force = forceVector.normalized * currentPower * forceMultiplier;
            
            rb.AddForce(force, ForceMode.Impulse);

            //if nothing of the things above happend, ischaring equals false
            isCharging = false;
            // and sets Slider back to 0
            powerSlider.value = 0f;
        }
        //If you click on the left Mouse Button and isCharging is true...
        if (Input.GetMouseButton(0) && isCharging)
        {
            //...the longer right Mouse Button is clicked, the more power will be added
            currentPower += Time.deltaTime * maxPower;
            //...Limits the current Power to min.0 and max.(the amount of maxPower)
            currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
            //and the slider equals the current Power
            powerSlider.value = currentPower;

            //if you click on the right Mouse Button while left Mouse Button is clicked and isCharging is true...
            if (Input.GetMouseButtonDown(1))
            {
                //then is charging will be false again...
                isCharging = false;
                //Current Power will be 0 again...
                currentPower = 0f;
                //same as the Slider Value...
                powerSlider.value = 0f;
                //and this Debug.log pops up
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
        //if canInteract is true...
        if (canInteract)
        {
            //...following by if Left Mouse Button is clicked and is NOT charging
            if (Input.GetMouseButtonDown(0) && !isCharging)
            {
                //...isCharging gets true
                isCharging = true;
                //current Power equals 0 at the beginning
                currentPower = 0f;

                mousePressDownPos = Input.mousePosition;

                Debug.Log("Ball clicked!");
            }
        }
        else
        {
            Debug.Log("Ball still moving");
        }

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

}

