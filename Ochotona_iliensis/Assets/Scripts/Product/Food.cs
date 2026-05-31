using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public int healAmount = 1; // 回复1点生命

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                //我看改完后好像不用回血道具了，所以我把你Heal方法删了，然后这里先注释掉，以后要用再写。
                //playerHealth.Heal(healAmount);
                Debug.Log("伊犁鼠兔吃了食物，生命回复1点。");
            }
            // 吃完后销毁食物
            Destroy(gameObject);
        }
    }
}