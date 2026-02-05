using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    private bool isRunning = false;
    private Stopwatch stopwatch =  new Stopwatch();
    private float inputMargin = 0.1f;
    private TMP_Text timerTMP;
    
    
    private String formatedTimeText;
    private TimeSpan ts;

    private void Start()
    {
        timerTMP = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (!isRunning )
        {
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > inputMargin || 
                Mathf.Abs(Input.GetAxisRaw("Horizontal")) > inputMargin)
            {
                isRunning = true;
                stopwatch.Start();
            }
        }
        ts = stopwatch.Elapsed;
        formatedTimeText = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        timerTMP.SetText(formatedTimeText);
    }

    private void OnLevelFinished()
    {
        stopwatch.Stop();
        Globals.raceTime = stopwatch.Elapsed;
    }
}
