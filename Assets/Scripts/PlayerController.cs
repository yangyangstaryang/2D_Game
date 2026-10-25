using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }  
    public Animator animator;
    public float speed = 1.0f;
    
    private bool isRunning;
    private Direction direction;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isRunning = false;
        movement = Vector2.zero;
        
        // 获取按键
        if (Input.GetKey(KeyCode.W))
        {
            // 更改坐标位置
            //transform.position += new Vector3(0, speed, 0);
            isRunning = true;
            direction = Direction.Up;
            movement = new Vector2(0, 1);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            //transform.position -= new Vector3(speed, 0, 0);
            isRunning = true;
            direction = Direction.Left;
            //transform.localScale = new Vector3(-1, 1, 1);
            //spriteRenderer.flipX = true;
            movement = new Vector2(-1, 0);
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            //transform.position -= new Vector3(0, speed, 0);
            isRunning = true;
            direction = Direction.Down;
            movement = new Vector2(0, -1);
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            //transform.position += new Vector3(speed, 0, 0);
            isRunning = true;
            direction = Direction.Right;
            //transform.localScale = new Vector3(1, 1, 1);
            //spriteRenderer.flipX = false;
            movement = new Vector2(1, 0);
        }
        
        // 更改动画状态机参数 (参数名 参数值)
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("Direction", (float)direction);
    }

    private void FixedUpdate()
    {
        // 物理移动 方向x位移
        // 位移=速度x时间
        rb.MovePosition(rb.position + movement*speed*Time.fixedDeltaTime);
    }
}
