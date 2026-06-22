using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKeyStart : MonoBehaviour
{
    [Header("关卡数据库")]
    public LevelDatabase levelDatabase; //拖入你的 LevelDatabase (ScriptableObject)

    [Header("跳转的目标游戏场景名字")]
    public string gameSceneName = "GameScene";

    private bool isStarting = false; //防止玩家疯狂连击导致重复加载

    void Update()
    {
        //如果已经触发了加载，就不再接收输入
        if (isStarting) return;

        //改为精确检测键盘空格键按下
        if (Input.GetMouseButtonDown(0))
        {
            TriggerStartGame();
        }
    }

    void TriggerStartGame()
    {
        if (levelDatabase == null || levelDatabase.allLevels.Length == 0)
        {
            Debug.LogError("未关联关卡数据库，或数据库里没有关卡数据！");
            return;
        }

        isStarting = true;
        Debug.Log("检测到空格键输入，正在准备进入第一关...");

        //从数据库里直接掏出第 0 个（第一关）的 LevelData
        LevelData firstLevelData = levelDatabase.allLevels[0];

        if (firstLevelData != null)
        {
            //塞进全局静态口袋，让 LevelManager 进游戏时能自动读到
            LevelManager.SelectedLevelData = firstLevelData;

            //直接跳转到游戏关卡场景
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("第一关的数据为空，请检查 LevelDatabase 配置！");
            isStarting = false;
        }
    }
}