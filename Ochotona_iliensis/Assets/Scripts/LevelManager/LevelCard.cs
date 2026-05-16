using UnityEngine;
using UnityEngine.EventSystems;

public class LevelCard : MonoBehaviour, IPointerClickHandler
{
    [Header("关卡数据")]
    public LevelData myLevelData;

    //点击时触发函数
    public void OnPointerClick(PointerEventData eventData)
    {
        if (myLevelData != null)
        {
            Debug.Log($"玩家直接点击了卡片，准备加载: {myLevelData.levelName}");

            //直接把这关的数据塞给全局静态变量
            LevelManager.SelectedLevelData = myLevelData;

            //跳转到游戏场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} 上没有分配 LevelData！");
        }
    }
}