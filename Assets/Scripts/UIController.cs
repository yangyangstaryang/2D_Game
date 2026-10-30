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
