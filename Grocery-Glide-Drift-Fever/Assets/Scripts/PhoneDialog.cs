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
    
    private double time;
    private int charCount = 0;
    private int dialogNr = 0;
    private LevelData levelData;
    private bool fulltextVisible = false;

    public void StartDialog(LevelData data)
    {
        levelData = data;
    }


    void Update()
    {
        if(!fulltextVisible){
            time += Time.deltaTime;
            if (time > secPerChar)
            {
                time = 0;
                text.maxVisibleCharacters++;
                if (text.maxVisibleCharacters >= charCount)
                {
                    fulltextVisible = true;
                    //text is fully visible
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (fulltextVisible)
            {
                AdvanceText();
            }
            else
            {
                text.maxVisibleCharacters = charCount;
            }
        }
    }

    private void AdvanceText()
    {
        phoneAnimator.SetTrigger(0); //0 == Talk
        text.maxVisibleCharacters = 0;
        dialogNr++;
        if (dialogNr >= levelData.dialog.Count)
        {
            //close dialog
            //event dialog done
        }
        text.SetText(levelData.dialog[dialogNr]);
        charCount = levelData.dialog[dialogNr].Length;
    }
}
