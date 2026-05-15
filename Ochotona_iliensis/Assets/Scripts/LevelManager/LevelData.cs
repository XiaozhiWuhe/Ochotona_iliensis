using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level System/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelID;
    public string levelName;
    public LevelData nextLevel;

    [Header("物理配置")]
    public float baseSpeed;
    public float maxSpeed;
    public float gravityScale;

    [Header("关卡预制件")]
    public GameObject mapPrefab; //存有 Sprite Shape 坡道的预制体
}