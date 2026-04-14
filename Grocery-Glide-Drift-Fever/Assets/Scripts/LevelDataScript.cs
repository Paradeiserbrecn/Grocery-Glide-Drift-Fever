using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public String scenePath;
    public Sprite previewImage;
    public List<string> dialog;

    public List<float> styleGrades = new List<float>()
    {
        5000f,
        4000f,
        3000f,
        2000f
    };

    public List<float> timeGrades = new List<float>()
    {
        20,
        25,
        30,
        40
    };

    public List<float> itemGrades = new List<float>()
    {
        0,
        1,
        3,
        5
    };
}