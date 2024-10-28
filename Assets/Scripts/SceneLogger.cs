using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLogger : MonoBehaviour
{
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("Loaded Scene: " + currentScene.name + ", BuildIndex: " + currentScene.buildIndex);
    }
}
