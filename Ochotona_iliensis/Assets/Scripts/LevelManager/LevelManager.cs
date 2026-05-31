using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    public LevelData currentLevelData;
    public PlayerController player;
    public static LevelData SelectedLevelData;

    [Header("Cinemachine虚拟相机引用")]
    public CinemachineVirtualCamera walkCamera;   
    public CinemachineVirtualCamera flightCamera; 

    void Start()
    {
        //如果静态变量里有值，加载它；否则加载默认关卡
        LevelData dataToLoad = SelectedLevelData != null ? SelectedLevelData : currentLevelData;
        LoadLevel(dataToLoad);
    }

    public void LoadLevel(LevelData data)
    {
        //清理旧关卡
        GameObject oldMap = GameObject.FindWithTag("LevelMap");
        if (oldMap != null) Destroy(oldMap);

        //生成地形
        GameObject map = Instantiate(data.mapPrefab, Vector3.zero, Quaternion.identity);

        //找到地图里的起点并安置玩家
        Transform startPoint = map.transform.Find("StartPoint");
        if (startPoint != null)
        {
            player.transform.position = startPoint.position;
            //重置玩家速度，防止上一关的速度带到这一关
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        //根据关卡类型切换玩家运动模式
        PlayerController walkScript = player.GetComponent<PlayerController>();
        PlayerFlightController flyScript = player.GetComponent<PlayerFlightController>();

        if (data.isFlightLevel)
        {
            //飞行关
            if (flyScript != null) flyScript.enabled = true;
            if (walkScript != null) walkScript.enabled = false;

            if (flightCamera != null) flightCamera.gameObject.SetActive(true);
            if (walkCamera != null) walkCamera.gameObject.SetActive(false);

            Debug.Log($"【模式切换】已激活《{data.levelName}》的飞行模式");
        }
        else
        {
            //普通关
            if (walkScript != null) walkScript.enabled = true;
            if (flyScript != null) flyScript.enabled = false;

            if (walkCamera != null) walkCamera.gameObject.SetActive(true);
            if (flightCamera != null) flightCamera.gameObject.SetActive(false);

            Debug.Log($"【模式切换】已激活《{data.levelName}》的滑行模式");
        }
    }

    public void OnLevelComplete()
    {
        Debug.Log("恭喜通关！");

        //停止玩家移动和物理模拟
        player.enabled = false;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; //让玩家定格在终点，不再受重力影响
        }

        //延时执行ReturnToMainMenu
        Invoke("ReturnToMainMenu", 2f);
    }

    void LoadNextLevel()
    {
        Debug.Log("准备加载下一关...");
    }

    void ReturnToMainMenu()
    {
        Debug.Log("正在返回选关界面...");
        SceneManager.LoadScene("MainMenu");
    }
}