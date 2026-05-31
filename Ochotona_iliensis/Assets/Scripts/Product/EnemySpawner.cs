using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;   // 要生成的敌人预制体
    public float spawnInterval = 2f; // 生成间隔（秒）
    public float spawnX = 12f;       // 生成时的 X 坐标（屏幕右侧外）
    public float spawnY = 0f;        // 生成时的 Y 坐标（根据敌人类型手动设置）

    void Start()
    {
        InvokeRepeating("Spawn", 1f, spawnInterval);
    }

    void Spawn()
    {
        Vector3 pos = new Vector3(spawnX, spawnY, 0);
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}