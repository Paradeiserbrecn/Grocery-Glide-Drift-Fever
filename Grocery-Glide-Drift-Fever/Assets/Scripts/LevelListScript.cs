using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Levels/Level List")]
public class LevelListScript : ScriptableObject
{
    [SerializeField] public List<LevelData> levels = new List<LevelData>();
}
