using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [SerializeField] float loadDelay = 3f;

    public TimerController timerController;

    BallController ballController;
    LevelManager levelManager;


    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    IEnumerator DelayBeforeLoadingNextScene()
    {
        yield return new WaitForSeconds(loadDelay);
        levelManager.LoadNextLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DelayBeforeLoadingNextScene());
            ballController.PlaySound(ballController.holeSound);
            timerController.StopTimer();
        }
    }
}
