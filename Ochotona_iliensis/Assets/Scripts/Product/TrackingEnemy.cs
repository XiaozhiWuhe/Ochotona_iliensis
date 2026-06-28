using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingEnemy : MonoBehaviour
{
    [Header("移动速度（由生成器设置）")]
    public float chaseSpeed = 3f;

    [Header("伤害与销毁")]
    public int damage = 1;
    public float destroyX = -15f;
    public float destroyY = 20f;
    public float screenOffset = 2f;

    private Transform player;
    private bool isAlive = true;
    private bool isActivated = false;
    private Camera mainCamera;
    private Transform myTransform;

    void Start()
    {
        mainCamera = Camera.main;
        myTransform = transform;
        InvokeRepeating("FindPlayer", 0f, 0.5f);
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (!isAlive) return;
        if (player == null) return;

        if (!isActivated)
        {
            if (mainCamera == null) return;
            float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            if (myTransform.position.x < rightEdge + screenOffset)
                isActivated = true;
            else return;
        }

        if (myTransform.position.x < destroyX || Mathf.Abs(myTransform.position.y) > destroyY)
        {
            Destroy(gameObject);
            return;
        }

        float step = chaseSpeed * Time.deltaTime;
        myTransform.position = Vector2.MoveTowards(myTransform.position, player.position, step);

        float dirX = player.position.x - myTransform.position.x;
        if (Mathf.Abs(dirX) > 0.001f)
        {
            Vector3 scale = myTransform.localScale;
            scale.x = dirX < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            myTransform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) return;
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }
    }

    public void InstantKill()
    {
        if (!isAlive) return;
        isAlive = false;
        Destroy(gameObject);
    }
}