using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    [SerializeField] float shootForce = 5f;
    [SerializeField] float drag = 0.5f;
    [SerializeField] LineRenderer aimLine;

    float forceFactor;
    //bool shootingMode;

    void Start()
    {
        aimLine.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.angularDrag = drag;
    }

    void Update()
    {
        ApplyForce();
        BallLaunch();
    }

    void ApplyForce()
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

    void BallLaunch()
    {
        //if (shootingMode)
        //{
        if (Input.GetMouseButtonDown(0))
        {
            aimLine.gameObject.SetActive(true);

            var ballScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            var mouseScreenPos = Input.mousePosition;
            ballScreenPos.z = 1f;
            mouseScreenPos.z = 1f;
            var positions = new Vector3[] {
                    Camera.main.ScreenToWorldPoint(ballScreenPos),
                    Camera.main.ScreenToWorldPoint(mouseScreenPos)
                };
            aimLine.SetPositions(positions);
            aimLine.endColor = Color.Lerp(Color.green, Color.red, forceFactor);
            //}
            /*else if (Input.GetMouseButtonUp(0))
            {
                shootingMode = false;
                aimLine.gameObject.SetActive(false);
            }*/
        }
    }
}
