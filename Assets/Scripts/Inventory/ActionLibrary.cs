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

    public void InvokeAction(ItemDataSO data)
    {
        switch (data.ID)
        {
            case 2:
                BloodAction(data);
                break;
        }
    }

    public void BloodAction(ItemDataSO data)
    {
        PlayerController.Instance.HealthHP(10);
    }
}
