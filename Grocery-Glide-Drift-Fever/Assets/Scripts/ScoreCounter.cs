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

    private Vector3 _targetPosition, _mousePos =  Vector3.zero;
    [SerializeField] Camera _camera;
    private List<ScoreTextWrapper> _movingTextWrappers = new List<ScoreTextWrapper>();
    
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
            _movingTextWrappers.Add(_scoreTextWrapper);
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
        if (_movingTextWrappers.Count > 0)
        {
            MoveCounters();
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ScoreUpdated(0);
            ScoreToSum(0,ScoreType.AirTime);
            _movingTextWrappers.Add(_scoreTextWrapper);
        }
    }

    private void MoveCounters()
    {
        _mousePos = Input.mousePosition;
        _targetPosition = _camera.ScreenToWorldPoint(new Vector3(0, 600, 5));
        Debug.Log(_mousePos);
        foreach (ScoreTextWrapper wrapper in _movingTextWrappers)
        {
            wrapper.moveTime += Time.deltaTime;
            wrapper.SetPos(Vector3.Lerp(wrapper.transform.position, _targetPosition, wrapper.moveTime));
        }
    }

        

    
    
    
}
