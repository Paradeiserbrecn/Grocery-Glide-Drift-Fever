using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSum : MonoBehaviour
{
    private int _scoreSum = 0;
    private ScoreTextWrapper _scoreText;
    
    
    private bool _bounce = false;
    private double _bounceTime = 0;
    private double _bounceDuration = 1;

    private void Start()
    {
        _scoreText = GetComponent<ScoreTextWrapper>();
    }

    public void AddScore(int score)
    {
        _scoreSum += score;
        _scoreText.UpdateText(_scoreSum);
        _bounceTime = 0;
        _bounce = true;
    }

    private void Update()
    {
        if (_bounce)
        {
            _scoreText.SetSize((float)(1+ (Math.Pow(3, -3*_bounceTime) * Math.Sin(3* 3.14156 * _bounceTime))));
            _bounceTime += Time.deltaTime;
            if (_bounceTime > _bounceDuration)
            {
                _bounce = false;
            }
        }
    }
}
