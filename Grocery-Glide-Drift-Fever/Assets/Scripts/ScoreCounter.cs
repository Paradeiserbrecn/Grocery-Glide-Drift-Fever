using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{

    private bool _driftCounterActive = false;
    private bool _airtimeCounterActive = false;
    private ScoreTextWrapper _driftScoreTextWrapper;
    private ScoreTextWrapper _airtimeTextWrapper;
    private ScoreSum _scoreSum;
    [SerializeField] private GameObject driftScoreTextPrefab;



    private void Start()
    {
        _scoreSum = FindObjectOfType<ScoreSum>();
        if (_scoreSum == null)
        {
            Debug.LogError("ScoreSum not found");
        }
    }

    public void DriftScoreUpdated(int score)
    {
        if (!_driftCounterActive)
        {
            _driftScoreTextWrapper = Instantiate(driftScoreTextPrefab, transform).GetComponent<ScoreTextWrapper>();
            _driftCounterActive = true;
        }
        _driftScoreTextWrapper.UpdateText(score);
    }
    
    public void AirtimeScoreUpdated(int airtime)
    {
        if (!_airtimeCounterActive)
        {
            _airtimeTextWrapper = Instantiate(driftScoreTextPrefab, transform).GetComponent<ScoreTextWrapper>(); 
            _airtimeCounterActive = true;
        }
        _airtimeTextWrapper.UpdateText(airtime);
    }

    public void AddDriftScoreToSum(int score)
    {
        if (_driftCounterActive)
        {
            Debug.Log("score added");
            _scoreSum.AddScore(score);
            Destroy(_driftScoreTextWrapper.gameObject);
            _driftCounterActive = false;
        }
    }

    public void AddAirtimeScoreToSum(int airtime)
    {
        if (_airtimeCounterActive)
        {
            Debug.Log("airtime added");
            _scoreSum.AddScore(airtime);
            Destroy(_airtimeTextWrapper.gameObject);
            _airtimeCounterActive = false;
        }
    }

    public void RemoveAirtimeCounter(int score)
    {
        if (_airtimeCounterActive)
        {
            Destroy(_airtimeTextWrapper.gameObject);
            _airtimeCounterActive = false;
        }
    }
}
