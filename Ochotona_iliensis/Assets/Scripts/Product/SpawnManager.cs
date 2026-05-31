using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("预制体引用")]
    public GameObject cliffCrackPrefab;
    public GameObject foodPrefab;
    public GameObject flyingPredatorPrefab;
    public GameObject groundPredatorPrefab;

    [Header("生成间隔")]
    public float cliffSpawnInterval = 3f;   // 崖缝每3秒尝试生成一次
    public float foodSpawnInterval = 5f;
    public float flyingSpawnInterval = 4f;
    public float groundSpawnInterval = 6f;

    [Header("生成位置范围")]
    public float groundY = -3f;      // 地面Y坐标，确保崖缝和狐鼬贴地
    public float flyingMinY = 1f;    // 飞禽飞行高度范围
    public float flyingMaxY = 3f;
    public float spawnX = 12f;       // 在屏幕右侧外生成

    // 计时器
    private float cliffTimer;
    private float foodTimer;
    private float flyingTimer;
    private float groundTimer;

    void Start()
    {
        // 初始化计时器，可以随机一个偏移，避免一开始所有东西一起出
        cliffTimer = Random.Range(1f, cliffSpawnInterval);
        foodTimer = Random.Range(2f, foodSpawnInterval);
        flyingTimer = Random.Range(1f, flyingSpawnInterval);
        groundTimer = Random.Range(2f, groundSpawnInterval);
    }

    void Update()
    {
        // 分别计时生成
        cliffTimer -= Time.deltaTime;
        if (cliffTimer <= 0)
        {
            SpawnCliff();
            cliffTimer = cliffSpawnInterval;
        }

        foodTimer -= Time.deltaTime;
        if (foodTimer <= 0)
        {
            SpawnFood();
            foodTimer = foodSpawnInterval;
        }

        flyingTimer -= Time.deltaTime;
        if (flyingTimer <= 0)
        {
            SpawnFlyingPredator();
            flyingTimer = flyingSpawnInterval;
        }

        groundTimer -= Time.deltaTime;
        if (groundTimer <= 0)
        {
            SpawnGroundPredator();
            groundTimer = groundSpawnInterval;
        }
    }

    void SpawnCliff()
    {
        // 崖缝生成在地面Y坐标上，X在右侧
        Vector3 spawnPos = new Vector3(spawnX, groundY, 0);
        Instantiate(cliffCrackPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnFood()
    {
        // 食物可能在空中（崖缝上方）或地面随机
        float randomY = groundY + Random.Range(0f, 2f); // 调整高度
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0);
        Instantiate(foodPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnFlyingPredator()
    {
        float randomY = Random.Range(flyingMinY, flyingMaxY);
        Vector3 spawnPos = new Vector3(spawnX, randomY, 0);
        Instantiate(flyingPredatorPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnGroundPredator()
    {
        Vector3 spawnPos = new Vector3(spawnX, groundY, 0);
        Instantiate(groundPredatorPrefab, spawnPos, Quaternion.identity);
    }
}