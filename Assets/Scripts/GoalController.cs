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
    LevelCompleteUI levelCompleteUI;


    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        levelManager = FindObjectOfType<LevelManager>();
        levelCompleteUI = FindObjectOfType<LevelCompleteUI>();

        if (levelCompleteUI == null)
        {
            Debug.LogError("LevelCompleteUI not found in the Scene!");
        }
    }

    IEnumerator DelayBeforeShowingLevelCompleteUI()
    {
        yield return new WaitForSeconds(loadDelay);

        if (levelCompleteUI != null)
        {
            levelCompleteUI.ShowLevelCompleteUI();
        }
        else
        {
            Debug.LogError("LevelCompleteUI is null");
            levelManager.LoadNextLevel();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DelayBeforeShowingLevelCompleteUI());
            ballController.PlaySound(ballController.holeSound);
            timerController.StopTimer();
        }
    }
}
