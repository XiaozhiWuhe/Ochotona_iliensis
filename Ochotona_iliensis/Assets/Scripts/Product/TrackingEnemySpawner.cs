using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEnemySpawner : MonoBehaviour
{
    public GameObject trackingEnemyPrefab;
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 10f;
    public float minRadius = 3f;
    public float maxRadius = 6f;

    [Header("敌人速度设置")]
    public float speedWhenBehind = 5f;   // 敌人在玩家后方（X < 玩家X）
    public float speedWhenAhead = 2.5f;  // 敌人在玩家前方（X > 玩家X）

    private Transform player;

    void Start()
    {
        FindPlayer();
        ScheduleSpawn();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void ScheduleSpawn()
    {
        float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
        Invoke(nameof(SpawnEnemy), interval);
    }

    void SpawnEnemy()
    {
        if (trackingEnemyPrefab == null || player == null)
        {
            ScheduleSpawn();
            return;
        }

        Vector3 spawnPos = GetRandomPositionAroundPlayer();
        GameObject newEnemy = Instantiate(trackingEnemyPrefab, spawnPos, Quaternion.identity);

        TrackingEnemy te = newEnemy.GetComponent<TrackingEnemy>();
        if (te != null)
        {
            // 根据生成位置相对于玩家的前后设置追踪速度
            if (spawnPos.x < player.position.x)
                te.chaseSpeed = speedWhenBehind;
            else
                te.chaseSpeed = speedWhenAhead;
        }

        ScheduleSpawn();
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minRadius, maxRadius);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;
        return player.position + offset;
    }
}