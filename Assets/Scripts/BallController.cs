using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    Rigidbody rb;

    private bool canInteract = false;
    private bool isCharging = false;

    [SerializeField] float maxPower = 100f;  // Maximale Schusskraft
    [SerializeField] float currentPower = 0f;

    public Slider powerSlider;     // Slider-UI für die Schusskraft

    [SerializeField] float minimumSpeed = 0.05f;
    [SerializeField] float stopThreshold = 1f;
    [SerializeField] float shootForce = 5f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] float angularDrag = 1f;
    [SerializeField] private Vector3 collisionImpulse = new Vector3(5, 3, 5);


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;

        powerSlider.minValue = 0f;
        powerSlider.maxValue = maxPower;
        powerSlider.value = 0f;
    }

    void Update()
    {
        if (rb.velocity.magnitude <= minimumSpeed && rb.angularVelocity.magnitude <= minimumSpeed)
        {
            canInteract = true;
        }
        else
        {
            canInteract = false;
        }

        if (rb.velocity.sqrMagnitude < stopThreshold * stopThreshold && rb.angularVelocity.sqrMagnitude < stopThreshold * stopThreshold)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (Input.GetMouseButtonUp(0) && isCharging && canInteract)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(ray.direction * currentPower, hit.point);
                }
            }
            isCharging = false;
            powerSlider.value = 0f;  // Sets Slider back to 0
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

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForceAtPosition(ray.direction * currentPower, hit.point);
                    }
                }
            }
            Debug.Log("Ball clicked!");
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

}

