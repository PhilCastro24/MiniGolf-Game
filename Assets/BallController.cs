using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    
    [SerializeField] float explosionForce = 10f;
    private float explosionRadius = 10f;

    [SerializeField] float shootForce = 5f;
    [SerializeField]
    float upwardsModifier = 4.0f;

    private Vector3 mouseScreenPosition;

    void Start()
    {
        mouseScreenPosition = Input.mousePosition;
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Barrier") && other.collider != null && other.rigidbody == null)
        {
            ApplyForce();
        }
    }

    void ApplyForce()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddExplosionForce(explosionForce, hit.point, explosionRadius, upwardsModifier);
                }
            }
        }
    }
}
