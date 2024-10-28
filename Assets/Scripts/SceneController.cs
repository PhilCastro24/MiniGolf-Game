using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        Debug.Log("Current Scene: " + currentSceneIndex + ", Next Scene: " + nextSceneIndex);

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading next scene: " + nextSceneIndex);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more Scenes to Load, return to Main Menu. ");
            BackToMenu();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
