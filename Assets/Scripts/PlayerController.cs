using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }  
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    [Header("Move")]
    private bool isRunning;
    private Direction direction;
    public float speed = 1.0f;
    public Animator animator;
    private Vector2 movement;
    
    [Header("Attack")]
    public GameObject upAtk;
    public GameObject downAtk;
    public GameObject leftAtk;
    public GameObject rightAtk;
    public float attackCD;
    private bool isAttackOver;
    
    [Header("Dash")]
    public float dashForce;
    public float dashTime;
    public float dashResumeTime;
    bool isDash, isDashOver;
    
    [Header("Ghost")]
    // public int ghostNum = 5;
    public GameObject ghostPrefab;
    public float ghostIntervalTime = 0.1f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        direction = Direction.Down;
        isDashOver = true;
        isAttackOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        isRunning = false;
        movement = Vector2.zero;
        
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(inputX, inputY);
        
        if(movement.y > 0) direction = Direction.Up;
        if(movement.y < 0) direction = Direction.Down;
        if(movement.x < 0) direction = Direction.Left;
        if(movement.x > 0) direction = Direction.Right;
        
        isRunning = movement.sqrMagnitude > 0;
        
        // 更改动画状态机参数 (参数名 参数值)
        animator.SetBool("IsRunning", isRunning);
        animator.SetFloat("Direction", (float)direction);

        if (Input.GetMouseButtonDown(0) && isAttackOver)
        {
            Attack();
        }

        if (Input.GetMouseButtonDown(1) && isDashOver)
        {
            Dash();
        }
    }

    private void FixedUpdate()
    {
        // 物理移动 方向x位移
        // 位移=速度x时间
        if (!isDash)
        {
            // rb.MovePosition(rb.position + movement*speed*Time.fixedDeltaTime);
            rb.velocity  = movement.normalized * speed;
        }
    }

    // 攻击动画进入事件（开启对应方向碰撞检测）
    public void AtkAnimEnterEvent(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                upAtk.SetActive(true);
                break;  
            case Direction.Down:
                downAtk.SetActive(true);
                break;
            case Direction.Left:
                leftAtk.SetActive(true);
                break;
            case Direction.Right:
                rightAtk.SetActive(true);
                break;
        }
    }

    // 攻击动画退出事件（关闭所有碰撞检测）
    public void AtkAnimExitEvent()
    {
        upAtk.SetActive(false);
        downAtk.SetActive(false);
        leftAtk.SetActive(false);
        rightAtk.SetActive(false);
    }

    void Dash()
    {
        isDash = true;
        isDashOver = false;
        StartCoroutine(GenerateGhost());
        rb.AddForce(movement.normalized * dashForce,  ForceMode2D.Impulse);
        StartCoroutine(OnDashEnd());
    }

    IEnumerator OnDashEnd()
    {
        yield return new WaitForSeconds(dashTime);
        isDash = false;
        yield return new WaitForSeconds(dashResumeTime);
        isDashOver = true; 
    }

    IEnumerator GenerateGhost()
    {
        // for (int i = 0; i < ghostNum; i++)
        while(isDash)
        {
            GameObject ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
            SpriteRenderer ghostSr = ghost.GetComponent<SpriteRenderer>();
            ghostSr.sprite = spriteRenderer.sprite;
            if (direction == Direction.Left)
            {
                ghostSr.flipX = true;
            }
            yield return new WaitForSeconds(ghostIntervalTime);
        }
    }

    private void Attack()
    {
        isAttackOver = false;
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCD());
    }

    IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(attackCD);
        isAttackOver = true;
    }

    public void Transfer(string targetName)
    {
        GameObject target =  GameObject.Find(targetName);
        if(target == null) return;  // 熔断
        transform.position = target.transform.position;
    }
}
