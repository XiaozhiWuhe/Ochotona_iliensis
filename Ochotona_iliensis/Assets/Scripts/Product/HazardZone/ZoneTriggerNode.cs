using UnityEngine;

public class ZoneTriggerNode : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject hazardZonePrefab;     // 危机区域的预制体
    public Transform spawnLocation;         // 危机区域生成的目标位置（一个空的 GameObject）

    private bool hasTriggered = false;       // 确保每个关卡节点只触发一次

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;

            if (hazardZonePrefab != null && spawnLocation != null)
            {
                // 在设定的位置生成矩形区域
                Instantiate(hazardZonePrefab, spawnLocation.position, spawnLocation.rotation);
                Debug.Log($"玩家到达关卡节点：{gameObject.name}，危机区域已生成！");
            }

            // 触发后，可以把这个隐形的节点物体销毁，节省性能
            Destroy(gameObject, 0.1f);
        }
    }
}