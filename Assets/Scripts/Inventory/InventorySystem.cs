using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    static InventorySystem instance;
    public static InventorySystem Instance { get { return instance; } }
    public List<Slot> slots;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlot(0);
            slots[0].UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlot(1);
            slots[1].UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlot(2);
            slots[2].UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSlot(3);
            slots[3].UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectSlot(4);
            slots[4].UseItem();
        }
    }

    void SelectSlot(int index)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i == index)
                slots[i].IsSelected(true);
            else
                slots[i].IsSelected(false);
        }
    }

    public void PickupItem(ItemDataSO data)
    {
        // 找到一个空的格子  或者 有相同物品的格子(可堆叠的情况下)
        Slot emptySlot = FindEmptySlot(data);
        // 将物品放入 堆叠的情况下就是数量+1
        emptySlot.AddItem(data);
    }

    Slot FindEmptySlot(ItemDataSO data)
    {
        foreach (Slot slot in slots)
        {
            if(slot.IsEmpty || (data.canStack && slot.itemID == data.ID))
                return slot;
        }
        return null;
    }
}
