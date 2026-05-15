using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞的是不是玩家
        if (collision.CompareTag("Player"))
        {
            // 通知 LevelManager 玩家过关了
            FindObjectOfType<LevelManager>().OnLevelComplete();
            // 禁用自身防止多次触发
            gameObject.SetActive(false);
        }
    }
}