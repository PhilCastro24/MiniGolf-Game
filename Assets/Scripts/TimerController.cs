using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private bool timerIsRunning = true;

    void Update()
    {
        if (timerIsRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimer();

        }
    }

    public void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        timerText.text = string.Format("Zeit: {0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }
    public void StartTimer()
    {
        timerIsRunning = true;
    }
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimer();
    }
}
