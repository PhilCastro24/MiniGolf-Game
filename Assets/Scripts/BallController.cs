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

    public Slider powerSlider;

    [SerializeField] float minimumSpeed = 0.05f;
    [SerializeField] float stopThreshold = 1f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] float lowestYPos = 10f;
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

        if (Input.GetMouseButtonUp(0) && isCharging && canInteract)
        {
            //Creates a ray starting from the Camera to wherever you click with your mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //a RaycastHit function will be named hit
            RaycastHit hit;

            //activates the Raycast and through the ray, and saves it inside the hit variable
            if (Physics.Raycast(ray, out hit))
            {
                //if hit.rigidbody has a value then...
                if (hit.rigidbody != null)
                {
                    //it will ad a force to exactly the position you click on the object, requires the direction
                    //multiplied with the current power and the exact position where it got hit
                    hit.rigidbody.AddForceAtPosition(ray.direction * currentPower, hit.point);
                }
            }
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
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}

