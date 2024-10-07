using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private bool timerIsRunning = true;

    //private int minutes;
    //private int seconds;

    /* 
    public int GetMinutes
    {
        get
        {
            return minutes;
        }
    }

    public int GetSeconds
    {
        get
        {
            return seconds;
        }
    }
    */

    void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimer();
        }
    }

    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }


    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimer();
    }

    void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        timerText.text = string.Format("Zeit: {0:00}:{1:00}", minutes, seconds);
    }
}
