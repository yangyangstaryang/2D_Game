using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>单个背包格子的存档数据（只存 ID 和数量，图标/预制体靠 ID 反查）</summary>
[Serializable]
public class SlotSaveData
{
    public int index;     // 背包第几个格子
    public int itemID;    // 物品 ID
    public int amount;    // 数量
}

[PreferBinarySerialization]
public class SaveDataSO : ScriptableObject
{
    public Vector3 playerPos;
    public int playerHP;
    public string sceneName;                                    // 存档时所在场景
    public List<SlotSaveData> slots = new List<SlotSaveData>();  // 背包内容

    /// <summary>把当前游戏状态写进这份存档数据（调用后交给 SaveSysten 落盘）</summary>
    public void Save()
    {
        playerPos = PlayerController.Instance.transform.position;
        playerHP = PlayerController.Instance.currentHp;

        // 所在场景：读档时先切回这个场景，位置才有意义
        sceneName = SceneManager.GetActiveScene().name;

        // 背包：当前场景有背包就读背包，没有（如 Scene2/Scene3）就沿用跨场景快照
        if (InventorySystem.Instance != null)
            slots = InventorySystem.Instance.ExportSlots();
        else
            slots = SaveSysten.InventorySnapshot;
    }

    /// <summary>
    /// 把存档应用回游戏。必须保证已经身处 sceneName 这个场景，否则位置会错。
    /// </summary>
    public void Load()
    {
        PlayerController.Instance.targetGate = "";       // 清掉传送门标记，别让 Gate 把玩家吸走
        PlayerController.Instance.RestoreFromSave(playerHP, playerPos);

        if (slots == null) slots = new List<SlotSaveData>();
        SaveSysten.InventorySnapshot = slots;

        if (InventorySystem.Instance != null)
            InventorySystem.Instance.ImportSlots(slots);
        else
            Debug.LogWarning("[读档] 当前场景没有背包，物品先存在快照里，回到有背包的场景会自动补上");
    }
}
