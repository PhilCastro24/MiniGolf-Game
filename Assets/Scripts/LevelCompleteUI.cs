using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    public GameObject levelComplete;
    public Button continueButton;

    LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();


        levelComplete.SetActive(false);
    }

    public void ShowLevelCompleteUI()
    {
        levelComplete.SetActive(true);
        Time.timeScale = 0;
    }

    public void ContinueToNextLevel()
    {
        Time.timeScale = 1;

        levelComplete.SetActive(false);

        if (levelManager != null)
        {
            levelManager.LoadNextLevel();
        }
        else
        {
            Debug.Log("LevelManager not found");
        }
    }
}
