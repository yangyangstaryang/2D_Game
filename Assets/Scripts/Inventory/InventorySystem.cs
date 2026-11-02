using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    static InventorySystem instance;
    public static InventorySystem Instance {  get { return instance; } }

    public List<Slot> slots;

    public List<BaseItem> items;

    [Header("物品数据表（存档还原用，把 Data/Item 下的 SO 拖进来）")]
    public List<BaseItemDataSO> itemDatabase = new List<BaseItemDataSO>();

    // 导入存档期间不要反向刷新快照，避免边读边写真把数据搞乱
    bool isImporting;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // 从别的场景回来时，把背包内容补回来
        if (SaveSysten.InventorySnapshot != null && SaveSysten.InventorySnapshot.Count > 0)
            ImportSlots(SaveSysten.InventorySnapshot);
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
        // 通关任务记账：拾取到金币就+1（即使背包满了也算拾取到）
        if (QuestManager.Instance != null) QuestManager.Instance.OnItemPicked(item);

        Slot emptySlot = FindEmptySlot(item);
        if (emptySlot == null)
        {
            Debug.LogWarning("背包已满，无法拾取：" + item.itemName);
            return;
        }
        if (item.type == BaseItem.ItemType.Consumable)
        {
            if (emptySlot.useAction == null)
            {
                emptySlot.useAction += ((ConsumableItem)item).Use;
            }
        }
        emptySlot.AddItem(item.ID, item.icon, item.type, item.prefab);
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

    // ---------------- 存档相关 ----------------

    /// <summary>把当前背包导出成存档数据（只记录非空格子）</summary>
    public List<SlotSaveData> ExportSlots()
    {
        List<SlotSaveData> result = new List<SlotSaveData>();
        if (slots == null) return result;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null || slots[i].IsEmpty) continue;
            SlotSaveData d = new SlotSaveData();
            d.index = i;
            d.itemID = slots[i].itemID;
            d.amount = slots[i].Amount;
            result.Add(d);
        }
        return result;
    }

    /// <summary>用存档数据覆盖当前背包（先全部清空再填，避免和旧物品叠加）</summary>
    public void ImportSlots(List<SlotSaveData> data)
    {
        if (slots == null || data == null) return;

        isImporting = true;

        foreach (var slot in slots)
            if (slot != null) slot.ClearItem();

        foreach (var d in data)
        {
            if (d == null || d.amount <= 0) continue;
            if (d.index < 0 || d.index >= slots.Count)
            {
                Debug.LogWarning("[读档] 存档里的格子下标越界：" + d.index);
                continue;
            }

            BaseItemDataSO so = FindData(d.itemID);
            if (so == null)
            {
                Debug.LogWarning("[读档] itemDatabase 里找不到 ID=" + d.itemID + " 的物品，请把对应的 Data SO 拖到 InventorySystem 的 Item Database 里");
                continue;
            }

            BaseItem.ItemType type = BaseItem.ItemType.Material;
            if (so is ConsumableItemDataSO) type = BaseItem.ItemType.Consumable;

            Slot slot = slots[d.index];
            slot.RestoreItem(so.ID, so.icon, type, so.prefab, d.amount);

            // 消耗品要把"使用"事件重新挂上，否则读档后按数字键吃药没反应
            if (type == BaseItem.ItemType.Consumable)
                slot.useAction += new ConsumableItem(so as ConsumableItemDataSO).Use;
        }

        isImporting = false;
        SyncSnapshot();
    }

    /// <summary>同步一份背包快照到存档系统，用于跨场景保留物品</summary>
    public void SyncSnapshot()
    {
        if (isImporting) return;
        SaveSysten.InventorySnapshot = ExportSlots();
    }

    BaseItemDataSO FindData(int id)
    {
        if (itemDatabase == null) return null;
        foreach (var so in itemDatabase)
            if (so != null && so.ID == id) return so;
        return null;
    }
}
