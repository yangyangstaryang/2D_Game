using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseItemDataSO : ScriptableObject
{
    public int ID;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public bool canStack;
}