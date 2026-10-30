using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public int forceRange = 5;
    public float knockBackTime = 0.2f;
    public GameObject damageNum;
    public HPBar hpBar;
    public int fullHP = 3;
    
    private bool isRunning;
    private Animator animator;
    private Direction direction;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 movement;
    private int currentHp;
    bool isKnockedBack;
    IEnumerator autoMove;
    
    
    // Start is called before the first frame update
    void Start()  
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        currentHp = fullHP;
        // 类型匹配
        autoMove = SetRandomMovement();
        StartCoroutine(autoMove); 
        
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
        
        // 随机生成movement 按一定时间间隔 计时器
        // Debug.Log(Random.Range(-1, 2));
        // 不要在Update里调用协程!!!
        
        // 判断是否在移动
        isRunning = movement.sqrMagnitude > 0;
        
        // 更改动画状态机参数 (参数名 参数值)
        animator.SetBool("IsRunning", isRunning);
        // animator.SetInteger("Direction", (int)direction);
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
    }
    
    private void FixedUpdate()
    {
        // 物理移动 方向x位移
        // 位移=速度x时间
        if (!isKnockedBack)
        {
            rb.MovePosition(rb.position + movement*speed*Time.fixedDeltaTime);
        }
        
    }

    IEnumerator SetRandomMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            movement = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
        }
    }

    // 向量的点乘Dot 叉乘Cross 以及它们的几何意义
    float GetAngle(Vector2 from, Vector2 to)
    {
        var cross = Vector3.Cross(from, to);
        if (cross.z > 0)
            return Vector2.Angle(from, to);
        else
            return 360 - Vector2.Angle(from, to);
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        KnockBack(attacker);
        currentHp -= damage;
        
        // 显示伤害数字
        GameObject numObj = Instantiate(damageNum, transform.position, Quaternion.identity);
        DamageNum num =  numObj.GetComponentInChildren<DamageNum>();
        num.SetNum(damage);
        
        // 更新血条
        hpBar.ShowHPBar((float)currentHp / fullHP);
        
        Debug.Log($"{gameObject.name} HP: {currentHp}");
        CheckHp();
    }

    void CheckHp()
    {
        if (currentHp <= 0)
        {
            animator.SetTrigger("Dead");
        }
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);   // 删除自身
    }

    void KnockBack(Transform attacker)
    {
        isKnockedBack = true;
        // (自己的位置 - 攻击者的位置) 归一化 获得攻击方向 * 攻击力度
        var force = (transform.position - attacker.position).normalized * forceRange;
        rb.AddForce(force, ForceMode2D.Impulse);     // 给物体添加一个冲击力 
        StartCoroutine(KnockBackTime());
    }

    IEnumerator KnockBackTime()
    {
        yield return new WaitForSeconds(knockBackTime);
        isKnockedBack = false;
    }

    void Follow()
    {
        // 获取面向玩家的方向 = 玩家位置 - 自身位置
        // 单例模式用途：1、保证实例在全局中只存在一个   2、能全局调用到实例
        // 不能让方向的长度影响我们的速度 所以需要“归一化”
        movement = (PlayerController.Instance.transform.position - transform.position).normalized;
    }

    [Header("view")] 
    public Transform viewCenter;
    [Range(0, 3)]public float viewDistance = 3;
    [Range(0, 360)]public float viewAngle = 90;
    
    private void OnDrawGizmos()
    {
        if (InViewField()) Handles.color = Color.green;
        else Handles.color = Color.yellow;
        Handles.DrawWireArc(viewCenter.position, Vector3.back, Vector3.up, 360, viewDistance);
        
        Gizmos.color = Color.red;
        float rot = GetAngle(Vector2.up, movement);
        float leftAngle = -viewAngle / 2 + 90 + rot;
        float rightAngle = viewAngle / 2 + 90 + rot;
        
        Vector3 left = new Vector3(Mathf.Cos(leftAngle * Mathf.Deg2Rad), Mathf.Sin(leftAngle * Mathf.Deg2Rad), 0);
        Vector3 right = new Vector3(Mathf.Cos(rightAngle * Mathf.Deg2Rad), Mathf.Sin(rightAngle * Mathf.Deg2Rad), 0);
        Gizmos.DrawLine(viewCenter.position, viewCenter.position + left.normalized * viewDistance);
        Gizmos.DrawLine(viewCenter.position, viewCenter.position + right.normalized * viewDistance);
    }

    // 判断玩家是否在视线范围内
    bool InViewField()
    {
        if(!Application.isPlaying) return false;
        Vector3 playerPos = PlayerController.Instance.transform.position;
        if (Vector3.Distance(playerPos, viewCenter.position) < viewDistance)
        {
            // 求夹角
            Vector3 dir = playerPos - viewCenter.position;
            float angle = Vector2.Angle(movement, dir);
            if (angle < viewAngle / 2)
                return true;
        }
        return false;
    }

    enum State
    {
        Idle,
        Patrol,
        Trace,
        Return,
        Dead
    }
    
    State state = State.Patrol;

    // 状态机只管切换状态
    // 具体逻辑要在对应的状态里执行
    void StateMachine()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Patrol:
                if (InViewField())
                {
                    StopCoroutine(autoMove);
                    state = State.Trace;
                }
                break;
            case State.Trace:
                Follow();
                if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > 5)
                {
                    StartCoroutine(autoMove);
                    state = State.Patrol;
                }
                break;
            case State.Return:
                break;
            case State.Dead:
                break;
        }
    }
}
