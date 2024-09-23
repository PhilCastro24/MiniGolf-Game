using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float shootForce = 5f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] private Vector3 collisionImpulse = new Vector3(5, 3, 5);


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(ray.direction * shootForce, hit.point);
                }
            }
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
