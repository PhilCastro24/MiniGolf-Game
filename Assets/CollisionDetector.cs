using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier") && collision.collider != null && collision.rigidbody == null)
        {
            Debug.Log("Collided with object that has only a Collider: " + collision.gameObject.name);
        }
    }
}

