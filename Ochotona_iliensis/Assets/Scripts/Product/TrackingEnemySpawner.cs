using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEnemySpawner : MonoBehaviour
{
    public GameObject trackingEnemyPrefab;

    [Header("生成间隔")]
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 10f;

    [Header("生成位置（相对于玩家）")]
    public float minRadius = 3f;
    public float maxRadius = 6f;

    [Header("敌人速度设置")]
    public float speedWhenBehind = 5f;
    public float speedWhenAhead = 2.5f;

    private Transform player;

    void Start()
    {
        FindPlayer();
        StartCoroutine(SpawnLoop());
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (trackingEnemyPrefab == null || player == null) return;

        Vector3 spawnPos = GetRandomPositionAroundPlayer();
        GameObject newEnemy = Instantiate(trackingEnemyPrefab, spawnPos, Quaternion.identity);

        TrackingEnemy te = newEnemy.GetComponent<TrackingEnemy>();
        if (te != null)
        {
            te.chaseSpeed = spawnPos.x < player.position.x ? speedWhenBehind : speedWhenAhead;
        }
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minRadius, maxRadius);
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * distance;
        return player.position + offset;
    }
}