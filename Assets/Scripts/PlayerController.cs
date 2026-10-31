using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }  
    
    static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }
    
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
    float dashCooldownEndTime;    // 冲刺解锁的时刻（给冷却UI读取）
    float attackCooldownEndTime;  // 攻击解锁的时刻（给冷却UI读取）

    // ---- 供冷却UI查询：剩余冷却时间 / 总冷却时长 ----
    public float DashCooldownRemaining { get { return Mathf.Max(0f, dashCooldownEndTime - Time.time); } }
    public float DashCooldownTotal { get { return dashTime + dashResumeTime; } }
    public float AttackCooldownRemaining { get { return Mathf.Max(0f, attackCooldownEndTime - Time.time); } }
    public float AttackCooldownTotal { get { return attackCD; } }
    
    [Header("Ghost")]
    // public int ghostNum = 5;
    public GameObject ghostPrefab;
    public float ghostIntervalTime = 0.1f;
    
    [Header("Gate")]
    public string targetGate;
    public bool inGate;
    public string targetScene;
    public GameObject KeyE;

    [Header("Stamina")]
    public float fullStamina = 100f;
    public float staminaRegenSpeed = 20f;    // 每秒回复多少点
    float currentStamina;

    private void Awake()
    {
        // 将本实例存入公共区域
        // unity单例写法
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
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
        
        // 初始化
        currentHp = fullHP;
        currentStamina = fullStamina;                 // 开局精力为空
        if (UIController.Instance != null)   // 立刻刷一次UI，不要等下一帧
            UIController.Instance.SetStaminaFillAmount(1f);
    }

    // Update is called once per frame
    void Update()
    {
        // 精力回复：没满就一直回，消耗后立刻开始，没有停顿
        if (currentStamina < fullStamina)
        {
            currentStamina = Mathf.Min(currentStamina + staminaRegenSpeed * Time.deltaTime, fullStamina);
            if (UIController.Instance != null)
                UIController.Instance.SetStaminaFillAmount(currentStamina / fullStamina);
        }
        
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

        if (inGate && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    private void FixedUpdate()
    {
        // 物理移动 方向x位移
        // 位移=速度x时间
        if (!isDash && !isKnockedBack)
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
        if (!CheckStamina(30)) return;
        isDash = true;
        isDashOver = false;
        dashCooldownEndTime = Time.time + dashTime + dashResumeTime;
        StartCoroutine(GenerateGhost());
        rb.AddForce(movement.normalized * dashForce,  ForceMode2D.Impulse);
        StartCoroutine(OnDashEnd());
    }

    bool CheckStamina(float expend)
    {
        // 精力值不足
        if(currentStamina < expend)
            return false;
        // 精力值够
        currentStamina -= expend;
        if (UIController.Instance != null)
            UIController.Instance.SetStaminaFillAmount(currentStamina / fullStamina);
        return true;
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
        if(!CheckStamina(20))  return;
        isAttackOver = false;
        attackCooldownEndTime = Time.time + attackCD;
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCD());
    }

    IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(attackCD);
        isAttackOver = true;
    }

    private bool isKnockedBack;
    public float forceRange = 5;
    float knockBackTime = 0.2f;
    
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

    public int fullHP = 100;
    int currentHp;
    public GameObject damageNum;
    
    public void TakeDamage(int damage, Transform attacker)
    {
        KnockBack(attacker);
        StartCoroutine(ChangeColor());
        currentHp -= damage;
        UIController.Instance.SetHpFillAmount((float)currentHp / fullHP);
        
        // 显示伤害数字
        GameObject numObj = Instantiate(damageNum, transform.position, Quaternion.identity);
        DamageNum num =  numObj.GetComponentInChildren<DamageNum>();
        num.SetNum(damage);
        
        // 更新血条
        // hpBar.ShowHPBar((float)currentHp / fullHP);
        
        // Debug.Log($"{gameObject.name} HP: {currentHp}");
        // CheckHp();
    }

    IEnumerator ChangeColor()
    {
        spriteRenderer.color  = Color.red;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color  = Color.white;
    }
    
}
