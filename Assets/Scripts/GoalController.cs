using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [SerializeField] float loadDelay = 3f;

    public TimerController timerController;

    BallController ballController;
    SceneController sceneController;

    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        sceneController = FindObjectOfType<SceneController>();
    }

    IEnumerator DelayBeforeLoadingNextScene()
    {
        yield return new WaitForSeconds(loadDelay);
        sceneController.LoadNextScene();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DelayBeforeLoadingNextScene());
            ballController.PlaySound(ballController.holeSound);
            timerController.StopTimer();
            ballController.hasReachedGoalTrigger = true;
        }
    }
}
