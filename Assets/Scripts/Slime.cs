using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }  
    public float speed = 1.0f;
    
    private bool isRunning;
    private Animator animator;
    private Direction direction;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 随机生成movement
        movement = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        
        // 更改动画状态机参数 (参数名 参数值)
        animator.SetBool("IsRunning", isRunning);
        animator.SetInteger("Direction", (int)direction);
    }
    
    private void FixedUpdate()
    {
        // 物理移动 方向x位移
        // 位移=速度x时间
        rb.MovePosition(rb.position + movement*speed*Time.fixedDeltaTime);
    }

    IEnumerator SetRandomMovement()
    {
        yield return 
    }
}
