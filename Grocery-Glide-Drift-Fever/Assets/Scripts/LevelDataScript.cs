using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public String ScenePath;
    public Sprite previewImage;
    public List<string> dialog;
}