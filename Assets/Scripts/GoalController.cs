using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public TimerController timerController;

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            timerController.StopTimer();
            SceneManager.LoadScene(+1);
        }
    }
}
