using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSum : MonoBehaviour
{
    private int scoreSum = 0;
    private TMP_Text itemName;
    private bool scoreCounterActive  = false;
    private ScoreCounter activeScoreCounter;

    private void InitializeScoreCounter()
    {
        scoreCounterActive = true;
        
    }

    public void DriftScore(int score)
    {
        
    }
}
