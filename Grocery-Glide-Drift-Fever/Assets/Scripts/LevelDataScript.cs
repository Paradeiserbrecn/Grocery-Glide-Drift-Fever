using Unity.VectorGraphics;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Scene Scene;
    public Sprite previewImage;
}