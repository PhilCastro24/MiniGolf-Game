using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]float ShootForce =5f;

    public float drag = 0.5f;

    bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&!isMoving)
        {
            StartCoroutine(StopBall());
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(ray.direction * ShootForce, hit.point);
                }
            }
        }
    }

    IEnumerator StopBall()
    {
        isMoving = true;
        yield return new WaitForSeconds(3);
        rb.angularVelocity = Vector3.zero;
        isMoving = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            rb.AddForce(new Vector3(5,3,5), ForceMode.Impulse);
        }
    }
}
