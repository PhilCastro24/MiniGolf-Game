using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public TimerController timerController;
    public BallController ballController;

    void Start()
    {
        ballController = FindObjectOfType<BallController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ballController.PlaySound(ballController.holeSound);
            timerController.StopTimer();
            //SceneManager.LoadScene(+1);
        }
    }
}
