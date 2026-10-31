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
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectSlot(4);
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

    public void PickupItem(ItemSO data)
    {
        // 找到一个空的格子  或者 有相同物品的格子(可堆叠的情况下)
        Slot emptySlot = FindEmptySlot(data);
        // 将物品放入 堆叠的情况下就是数量+1
        emptySlot.AddItem(data);
    }

    Slot FindEmptySlot(ItemSO data)
    {
        foreach (Slot slot in slots)
        {
            if(slot.IsEmpty || (data.canStack && slot.itemID == data.ID))
                return slot;
        }
        return null;
    }
}
