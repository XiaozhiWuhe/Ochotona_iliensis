using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 告诉玩家获得护盾
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.AddShield();
            }
            // 碎片消失
            Destroy(gameObject);
        }
    }
}