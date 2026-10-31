using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    static UIController instance;
    
    public static UIController Instance { get { return instance; } }
    
    public Image img_health;
    public Image img_stamina;

    [Header("Skill Cooldown")]
    public Sprite dashIconSprite;
    public Sprite swordIconSprite;

    void Start()
    {
        // 在 UICanvas 上挂冷却UI组件（右下角：冲刺、攻击两个圆形按钮）
        SkillCooldownUI cdUI = GetComponent<SkillCooldownUI>();
        if (cdUI == null)
            cdUI = gameObject.AddComponent<SkillCooldownUI>();
        if (dashIconSprite != null) cdUI.dashIcon = dashIconSprite;
        if (swordIconSprite != null) cdUI.swordIcon = swordIconSprite;
        cdUI.ApplyIcons();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void SetHpFillAmount(float fillAmount)
    {
        img_health.fillAmount = fillAmount;
    }
    
    public void SetStaminaFillAmount(float fillAmount)
    {
        img_stamina.fillAmount = fillAmount;
    }
}
