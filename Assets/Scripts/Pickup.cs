using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    Rigidbody2D rb;
    private Vector2 moveDir;
    public float pickupRadius = 5f;
    public float moveSpeed = 3f;
    public float acceleratedSpeed = 1.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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
            Destroy(gameObject);
        }
    }
}
