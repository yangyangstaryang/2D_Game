using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemData", menuName = "CreateData/ItemData")]
public class ItemDataSO : ScriptableObject
{
    public int ID;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public bool canStack;
    //public UnityAction useAction;
}
