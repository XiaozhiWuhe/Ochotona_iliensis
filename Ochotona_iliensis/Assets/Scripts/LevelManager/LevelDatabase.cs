using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Level System/Level Database")]
public class LevelDatabase : ScriptableObject
{
    //按界面上显示的顺序，把所有LevelData放这里
    public LevelData[] allLevels;
}
