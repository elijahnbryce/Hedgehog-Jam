using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool timerActive;
    public float time;

    public void TimerStart()
    {
        timerActive = true;
        time = 0;
    }

    private void Update()
    {
        if (timerActive)
        {
            time += Time.deltaTime;
        }
    }

    public void StopTime()
    {
        timerActive = false;
    }

    public void StartTime()
    {
        timerActive = true;
    }

    public void DisplayTime(TMPro.TextMeshProUGUI timerText)
    {
        string timeToDisplay;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timeToDisplay = string.Format("{0:00} : {1:00}", minutes, seconds);
        timerText.text = timeToDisplay;
    }

    public float GetTime()
    {
        return time;
    }
}
