using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreCounter : MonoBehaviour
{

    private bool _counterActive = false;
    private ScoreTextWrapper _scoreTextWrapper;
    private ScoreSum _scoreSum;
    [SerializeField] private GameObject driftScoreTextPrefab;
    [SerializeField] private Transform scoreSpawnPoint;
    [SerializeField] private Transform scoreSumTransform;
    private Vector3  _scoreSumPosition;
    private Camera _camera;

    private Vector3 _targetPosition;
    private List<ScoreTextWrapper> _movingTextWrappers = new List<ScoreTextWrapper>();
    private bool _moving;
    
    public enum ScoreType{AirTime, Drift}



    private void Start()
    {
        _scoreSum = FindObjectOfType<ScoreSum>();
        if (_scoreSum == null)
        {
            Debug.LogError("ScoreSum not found");
        }
        
        _camera = Camera.main;
        _scoreSumPosition = scoreSumTransform.position;
    }

    public void ScoreUpdated(int score)
    {
        if (!_counterActive)
        {
            _scoreTextWrapper = Instantiate(driftScoreTextPrefab, scoreSpawnPoint).GetComponent<ScoreTextWrapper>();
            _counterActive = true;
        }
        _scoreTextWrapper.UpdateText(score);
    }

    public void StopCounter(int score, ScoreType type)
    {
        if (_counterActive)
        {
            _scoreTextWrapper.score = score;
            _scoreTextWrapper.scoreType = type;
            _scoreTextWrapper.transform.SetParent(_camera.transform);
            _movingTextWrappers.Add(_scoreTextWrapper);
            _scoreTextWrapper = null;
            _counterActive = false;
            _moving = true;
        }
    }

    private void ScoreToSum(ScoreTextWrapper scoreText)
    {
        Debug.Log(scoreText.scoreType + " score added");
        _scoreSum.AddScore(scoreText.score);
        _movingTextWrappers.Remove(scoreText);
        Destroy(scoreText.gameObject);
        if(_movingTextWrappers.Count == 0) _moving = false;
    }
    
    
    private void FixedUpdate()
    {
        if (_moving)
        {
            MoveCounters();
        }
    }

    private void MoveCounters()
    {
        _targetPosition = _camera.ScreenToWorldPoint(new Vector3(_scoreSumPosition.x, _scoreSumPosition.y ,1f));
        
        Debug.Log(_camera.transform.position +",  " + _targetPosition);
        
        int idx = 0;
        //foreach (ScoreTextWrapper wrapper in _movingTextWrappers)
        while(idx <  _movingTextWrappers.Count)
        {
            ScoreTextWrapper wrapper = _movingTextWrappers[idx];
            
            wrapper.SetPos(Vector3.MoveTowards(wrapper.transform.position, _targetPosition, 0.2f));
            wrapper.transform.localScale = wrapper.transform.localScale * 0.95f;
            
            if (Vector3.Distance(wrapper.transform.position, _targetPosition) < 0.15f)
            {
                ScoreToSum(wrapper);
                continue;
            }

            idx++;
        }
    }
}
