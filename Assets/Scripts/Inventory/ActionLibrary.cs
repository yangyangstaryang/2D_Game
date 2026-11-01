using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionLibrary : MonoBehaviour
{
    private static ActionLibrary instance;
    
    public static ActionLibrary Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    public void InvokeAction(ConsumableSO data)
    {
        switch (data.ID)
        {
            case 1:
                BloodAction(data);
                break;
        }
    }

    public void BloodAction(ConsumableSO data)
    {
        PlayerController.Instance.HealthHP(data.health);
    }
}
