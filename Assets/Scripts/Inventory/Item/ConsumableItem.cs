using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : BaseItem
{
    public float health;

    public ConsumableItem(ConsumableItemDataSO data)
    {
        type = ItemType.Consumable;
        ID = data.ID;
        itemName = data.itemName;
        icon = data.icon;
        prefab = data.prefab;
        canStack = data.canStack;
        health = data.health;
    }

    public void Use()
    {
        PlayerController.Instance.HealthHP((int)health);
    }
}