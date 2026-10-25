using System.Collections;
using System.Collections.Generic;
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
    
    bool isRunning;
    public Animator animator;
    public float speed = 1.0f;
    Direction direction;
    SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        // 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        isRunning = false;
        
        // 获取按键
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, speed, 0);
            isRunning = true;
            direction = Direction.Up;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(speed, 0, 0);
            isRunning = true;
            direction = Direction.Left;
            //transform.localScale = new Vector3(-1, 1, 1);
            spriteRenderer.flipX = true;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0, speed, 0);
            isRunning = true;
            direction = Direction.Down;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed, 0, 0);
            isRunning = true;
            direction = Direction.Right;
            //transform.localScale = new Vector3(1, 1, 1);
            spriteRenderer.flipX = false;
        }
        
        // 更改动画状态机参数 (参数名 参数值)
        animator.SetBool("IsRunning", isRunning);
        animator.SetInteger("Direction", (int)direction);
    }
}
