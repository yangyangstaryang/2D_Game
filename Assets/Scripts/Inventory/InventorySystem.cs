using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    static InventorySystem instance;
    public static InventorySystem Instance {  get { return instance; } }

    public List<Slot> slots;

    public List<BaseItem> items;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
        for (int i = 0; i < slots.Count; i++) {
            if(i == index)
                slots[i].IsSelected(true);
            else 
                slots[i].IsSelected(false);
        }
    }

    public void PickupItem(BaseItem item)
    {
        Slot emptySlot = FindEmptySlot(item);
        if (item.type == BaseItem.ItemType.Consumable)
        {
            if (emptySlot.useAction == null)
            {
                emptySlot.useAction += ((ConsumableItem)item).Use;
            }
        }
        emptySlot.AddItem(item.ID, item.icon, item.type);
    }

    Slot FindEmptySlot(BaseItem item)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty || (item.canStack && slot.itemID == item.ID))
                return slot;
        }
        return null;
    }
}
