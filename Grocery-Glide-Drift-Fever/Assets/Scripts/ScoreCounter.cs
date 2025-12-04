using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{

    private bool _counterActive = false;
    private ScoreTextWrapper _scoreTextWrapper;
    private ScoreSum _scoreSum;
    [SerializeField] private GameObject driftScoreTextPrefab;

    private Vector3 _targetPosition;
    
    public enum ScoreType{AirTime, Drift}



    private void Start()
    {
        _scoreSum = FindObjectOfType<ScoreSum>();
        if (_scoreSum == null)
        {
            Debug.LogError("ScoreSum not found");
        }
    }

    public void ScoreUpdated(int score)
    {
        if (!_counterActive)
        {
            _scoreTextWrapper = Instantiate(driftScoreTextPrefab, transform).GetComponent<ScoreTextWrapper>();
            _counterActive = true;
        }
        _scoreTextWrapper.UpdateText(score);
    }

    public void ScoreToSum(int score, ScoreType type)
    {
        if (_counterActive)
        {
            Debug.Log(type + " score added");
            _scoreSum.AddScore(score);
            Destroy(_scoreTextWrapper.gameObject);
            _counterActive = false;
        }
    }

    public void RemoveCounter(int score)
    {
        if (_counterActive)
        {
            Destroy(_scoreTextWrapper.gameObject);
            _counterActive = false;
        }
    }

    private void Update()
    {
        
    }

    private void MoveCounter()
    {
        
    }
}
