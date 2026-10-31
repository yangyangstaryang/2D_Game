using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUp : MonoBehaviour
{
    public enum DropType
    {
        Coin,
        Blood
    }
    
    public DropType type;
    
    Rigidbody2D rb;
    private Vector2 moveDir;
    public float pickupRadius = 5f;
    public float moveSpeed = 3f;
    public float acceleratedSpeed = 1.0f;
    public float dropRadius = 1.0f;
    public float dropTime = 0.2f;
    public float dropHeight = 1f;
    public AnimationCurve curve;
    private bool isDropping;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DropAnim());
    }

    private void Update()
    {
        if(isDropping) return;
        
        Vector3 playerPos = PlayerController.Instance.transform.position;

        if (Vector2.Distance(playerPos, transform.position) < pickupRadius)
        {
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += acceleratedSpeed;
        }
        else
        {
            moveDir = Vector2.zero;
            moveSpeed = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (type)
            {
                case DropType.Coin:
                    Debug.Log("捡到了硬币");
                    break;
                case DropType.Blood:
                    Debug.Log("捡到了血药");
                    break;
            }
            Destroy(gameObject);
        }
    }

    // 抛物线动画
    IEnumerator DropAnim()
    {
        isDropping = true;
        // 设定起始点和结束点
        Vector2 startPoint = transform.position;
        Vector2 endPoint = new Vector2(startPoint.x + Random.Range(-dropRadius, dropRadius),
            startPoint.y + Random.Range(-dropRadius, dropRadius));
        
        float timer = 0.0f;
        while (timer < dropTime)
        {
            timer += Time.deltaTime;
            float delta = timer / dropTime;
            
            // 计算抛物线轨迹
            // float x = startPoint.x + (endPoint.x - startPoint.x) * timer / dropTime;
            // 插值函数
            float y = Mathf.Lerp(0f, dropHeight,curve.Evaluate(delta));
            
            transform.position = Vector2.Lerp(startPoint, endPoint, delta) + new Vector2(0, y);
            
            yield return null;
        }
        isDropping = false;
    }
}
