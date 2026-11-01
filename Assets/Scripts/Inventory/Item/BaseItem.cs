using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem
{
    public enum ItemType
    {
        None,
        Consumable,
        Material,
        Equip
    }

    // Data
    public int ID;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public bool canStack;

    public ItemType type;
    public int slotIndex;

    public BaseItem()
    {
    }
}