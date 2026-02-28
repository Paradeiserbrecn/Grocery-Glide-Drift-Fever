using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhoneDialog : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Animator phoneAnimator;
    [SerializeField] private double secPerChar;
    private TextAsset jsonFile;
    
    private double time;
    private int charCount = 0;

    private void Start()
    {
        
    }


    void Update()
    {
        time += Time.deltaTime;
        if (time > secPerChar)
        {
            time = 0;
            text.maxVisibleCharacters++;
        }
    }

    private void AdvanceText()
    {
        phoneAnimator.SetTrigger(0); //0 == Talk
        text.maxVisibleCharacters = 0;
        text.SetText();
    }
}
