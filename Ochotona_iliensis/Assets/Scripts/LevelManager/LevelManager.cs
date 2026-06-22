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

    [Header("UI管理器引用")]
    public GameUIManager uiManager; // 拖入场景里的 UI 脚本
    private LevelData loadedLevelData; // 内部记录当前成功生成的关卡数据
    void Start()
    {
        //如果静态变量里有值，加载它；否则加载默认关卡
        LevelData dataToLoad = SelectedLevelData != null ? SelectedLevelData : currentLevelData;
        LoadLevel(dataToLoad);
    }

    public void LoadLevel(LevelData data)
    {
        loadedLevelData = data; //存下来，方便后续读取nextLevel
        Time.timeScale = 1f;    //确保进关卡时间流速正常

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

        //从刚生成的地图物体中，寻找挂有边界的子物体
        //通过名字"LevelBoundary"来找，
        Transform boundaryTransform = map.transform.Find("LevelBoundary");

        if (boundaryTransform != null)
        {
            Collider2D boundaryCollider = boundaryTransform.GetComponent<Collider2D>();

            if (boundaryCollider != null)
            {
                //动态为滑行相机挂载边界
                if (walkCamera != null)
                {
                    var confiner = walkCamera.GetComponent<CinemachineConfiner2D>();
                    if (confiner != null)
                    {
                        confiner.m_BoundingShape2D = boundaryCollider;
                        confiner.InvalidateCache(); //告诉Cinemachine缓存失效，立刻重新计算新边界！
                    }
                }

                //动态为飞行相机挂载边界
                if (flightCamera != null)
                {
                    var confiner = flightCamera.GetComponent<CinemachineConfiner2D>();
                    if (confiner != null)
                    {
                        confiner.m_BoundingShape2D = boundaryCollider;
                        confiner.InvalidateCache(); //同样刷新缓存
                    }
                }

                Debug.Log($"【边界自动化】成功将《{data.levelName}》的空气墙注入相机限制器！");
            }
        }
        else
        {
            Debug.LogWarning($"在预制件 {data.mapPrefab.name} 中没有找到名为 'LevelBoundary' 的子物体！");
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

    //当玩家在 FinishLine 触碰或者满足胜利时调用
    public void OnLevelComplete()
    {
        Debug.Log("恭喜通关！");
        player.enabled = false;

        //检查数据库/当前关卡中是否配置了下一关
        bool hasNextLevel = (loadedLevelData != null && loadedLevelData.nextLevel != null);

        //弹出胜利结算界面，并把“有没有下一关”的生死大权交过去
        if (uiManager != null)
        {
            uiManager.ShowVictoryUI(hasNextLevel);
        }
    }

    //供PlayerHealth死亡时，或者掉落深渊时调用
    public void OnLevelFailed()
    {
        Debug.Log("关卡失败！");
        player.enabled = false;

        //弹出失败结算界面
        if (uiManager != null)
        {
            uiManager.ShowGameOverUI();
        }
    }

    //供胜利界面上的“进入下一关”按钮绑定的公共函数
    public void EnterNextLevel()
    {
        if (loadedLevelData != null && loadedLevelData.nextLevel != null)
        {
            //把下一关塞进口袋里
            SelectedLevelData = loadedLevelData.nextLevel;
            //重新载入当前游戏场景（LevelManager 启动时会自动解包口袋里的下一关）
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}