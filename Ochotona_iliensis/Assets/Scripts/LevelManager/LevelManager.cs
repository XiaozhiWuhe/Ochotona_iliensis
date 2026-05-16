using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelData currentLevelData;
    public PlayerController player;
    public static LevelData SelectedLevelData;

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
    }

    public void OnLevelComplete()
    {
        Debug.Log("恭喜通关！");
        //这里可以执行以下操作：
        //停止玩家移动
        player.enabled = false;
        //弹出过关 UI（我们稍后可以做这个）
        //延时 2 秒后加载下一关（或者回到主菜单）
        Invoke("LoadNextLevel", 2f);
    }

    void LoadNextLevel()
    {
        Debug.Log("准备加载下一关...");
    }
}