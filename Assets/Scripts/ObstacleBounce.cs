using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    [SerializeField] private Vector3 collisionImpulse = new Vector3(5, 3, 5);

    BallController ballController;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BallController ballController = collision.gameObject.GetComponent<BallController>();

            ballController.rb.AddForce(collisionImpulse, ForceMode.Impulse);
        }
    }
}
