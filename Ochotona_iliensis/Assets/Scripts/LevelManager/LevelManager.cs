using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData currentLevelData;
    public PlayerController player;

    void Start()
    {
        if (currentLevelData != null)
        {
            LoadLevel(currentLevelData);
        }
        else
        {
            Debug.LogError("未在 LevelManager 面板中指定 Current Level Data！");
        }
    }

    public void LoadLevel(LevelData data)
    {
        // 0. (可选) 清理旧关卡
        GameObject oldMap = GameObject.FindWithTag("LevelMap");
        if (oldMap != null) Destroy(oldMap);

        // 1. 生成地形
        GameObject map = Instantiate(data.mapPrefab, Vector3.zero, Quaternion.identity);

        // 2. 找到地图里的起点并安置玩家
        Transform startPoint = map.transform.Find("StartPoint");
        if (startPoint != null)
        {
            player.transform.position = startPoint.position;
            // 重置玩家速度，防止上一关的速度带到这一关
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void OnLevelComplete()
    {
        Debug.Log("恭喜通关！");
        // 这里可以执行以下操作：
        // 1. 停止玩家移动
        player.enabled = false;
        // 2. 弹出过关 UI（我们稍后可以做这个）
        // 3. 延时 2 秒后加载下一关（或者回到主菜单）
        Invoke("LoadNextLevel", 2f);
    }

    void LoadNextLevel()
    {
        // 这里如果要做自动化，你可能需要在 LevelData 里存一个 "NextLevelData"
        // 或者用一个 List<LevelData> 来按索引加载
        Debug.Log("准备加载下一关...");
    }
}