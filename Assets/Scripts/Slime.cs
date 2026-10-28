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
    public int forceRange = 5;
    public float knockBackTime = 0.2f;
    public GameObject damageNum;
    
    private bool isRunning;
    private Animator animator;
    private Direction direction;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 movement;
    private int hp;
    bool isKnockedBack;
    
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        hp = 3;

        StartCoroutine(SetRandomMovement()); 
    }

    // Update is called once per frame
    void Update()
    {
        // 随机生成movement 按一定时间间隔 计时器
        // Debug.Log(Random.Range(-1, 2));
        // 不要在Update里调用协程!!!
        
        // 判断是否在移动
        // movement.sqrMagnitude = movement.x*movement.x + movement.y*movement.y;
        // if (movement.sqrMagnitude > 0)
        // {
        //     isRunning = true;
        // }
        // else isRunning = false;
        isRunning = movement.sqrMagnitude > 0;
        
        // 判断方向
        // var angle = GetAngle(Vector2.up, movement);
        // if ((angle >= 0 && angle < 45) || (angle >= 315 && angle <= 360))
        // {
        //     direction = Direction.Up;
        // }
        // else if (angle >= 45 && angle <= 135)
        // {
        //     direction = Direction.Right;
        //     spriteRenderer.flipX = false;
        // }
        // else if (angle >= 135 && angle <= 225)
        // {
        //     direction = Direction.Down;
        // }
        // else
        // {
        //     direction = Direction.Left;
        //     spriteRenderer.flipX = true;
        // }
        
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
        if (cross.z < 0)
            return Vector2.Angle(from, to);
        else
            return 360 - Vector2.Angle(from, to);
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        KnockBack(attacker);
        hp -= damage;
        GameObject numObj = Instantiate(damageNum, transform.position, Quaternion.identity);
        DamageNum num =  numObj.GetComponentInChildren<DamageNum>();
        num.SetNum(damage);
        
        Debug.Log($"{gameObject.name} HP: {hp}");
        CheckHp();
    }

    void CheckHp()
    {
        if (hp <= 0)
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
}
