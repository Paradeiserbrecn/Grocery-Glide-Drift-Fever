using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Newspaper : MonoBehaviour
{
    [SerializeField] private TMP_Text pollResult, fillText;
    [SerializeField] private GameObject newspaper;
    
    [SerializeField] private List<string> styleTexts, timeTexts, itemsTexts;
    
    public enum Grade{S, A, B, C, D};
    private void Start()
    {
        EventManager.LevelFinished += OnLevelFinished;
        newspaper.gameObject.SetActive(false);
    }

    private void OnLevelFinished()
    {
        Debug.Log((Globals.AirtimeScore+Globals.DriftScore) + "/" + Globals.raceTime.TotalSeconds + "/" + Globals.excessItems);
        
        newspaper.gameObject.SetActive(true);
        EnterAnimation();
        List<Grade> grades = LookUpGrades();
        pollResult.text = PollResultText(grades);
        fillText.text = FillText(grades);
    }

    private List<Grade> LookUpGrades()
    {
        List<Grade> grades = new List<Grade>();
        grades.Add(AssignGrade(Globals.currentLevel.styleGrades, (Globals.AirtimeScore + Globals.DriftScore), false));
        grades.Add(AssignGrade(Globals.currentLevel.timeGrades, (float)Globals.raceTime.TotalSeconds, true));
        grades.Add(AssignGrade(Globals.currentLevel.itemGrades, Globals.excessItems, true));
        return grades;
    }

    private Grade AssignGrade(List<float> gradeValues, float value, bool smallerIsBetter)
    {
        if (smallerIsBetter)
        {
            for (int i = 0; i < gradeValues.Count; i++)
            {
                if(value <= gradeValues[i])
                {
                    return (Grade)i;
                }
            }
        }
        else
        {
            for (int i = 0; i < gradeValues.Count; i++)
            {
                if(value >= gradeValues[i])
                {
                    return (Grade)i;
                }
            }
        }
        return Grade.D;
    }

    private string PollResultText(List<Grade> grades)
    {
        return "Poll result:\n" +
               $"<font=\"georgia SDF\">Style:</font> {grades[0]}\n" +
               $"<font=\"georgia SDF\">Time:</font> {grades[1]}\n" +
               $"<font=\"georgia SDF\">Items:</font> {grades[2]}";
    }

    private string FillText(List<Grade> grades)
    {
        return $"Eye wittnesses described the perpetrator as <font=\"georgiab SDF\">\"{styleTexts[(int)grades[0]]}\"" +
               $"</font> and <font=\"georgiab SDF\">\"{timeTexts[(int)grades[1]]}\"" +
               $"</font>. Some also mentioned they were <font=\"georgiab SDF\">\"{itemsTexts[(int)grades[2]]}\"" +
               "</font>. The authorities are still searching for the culprit. " +
               "The store will likely go bankrupt after this tragic incident.";
    }

    private void EnterAnimation()
    {
        
    }
}
