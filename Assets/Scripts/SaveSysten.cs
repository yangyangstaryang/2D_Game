using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 存盘 / 读盘工具。沿用原来的调用方式：SaveSysten.Save(data, "/SaveData.txt")
/// 存档位置：Application.persistentDataPath/SaveData/SaveData.txt
///
/// 改动说明：原来用 BinaryFormatter 把 JSON 字符串再包一层二进制，没必要且容易出兼容问题，
/// 现在直接写 JSON 文本，人眼可读、出问题也能直接打开看。
/// </summary>
public static class SaveSysten
{
    const string FolderName = "SaveData";

    /// <summary>
    /// 背包快照：跨场景保留。
    /// Scene2/Scene3 没有背包物体时，存档就用这份快照，物品不会丢。
    /// </summary>
    public static List<SlotSaveData> InventorySnapshot = new List<SlotSaveData>();

    /// <summary>拼出存档完整路径（fileName 传 "/SaveData.txt" 或 "SaveData.txt" 都行）</summary>
    public static string PathOf(string fileName)
    {
        string name = (fileName ?? "SaveData.txt").TrimStart('/', '\\');
        return Path.Combine(Application.persistentDataPath, FolderName, name);
    }

    public static void Save(SaveDataSO data, string fileName)
    {
        string path = PathOf(fileName);

        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        InventorySnapshot = data.slots;

        Debug.Log("[存档] 已保存：" + path + "（场景 " + data.sceneName + "，血量 " + data.playerHP + "，物品 " + (data.slots == null ? 0 : data.slots.Count) + " 格）");
    }

    public static SaveDataSO Load(string fileName)
    {
        string path = PathOf(fileName);
        SaveDataSO data = ScriptableObject.CreateInstance<SaveDataSO>();

        if (!File.Exists(path))
        {
            Debug.LogWarning("[读档] 找不到存档文件：" + path);
            return null;
        }

        string json = File.ReadAllText(path);
        try
        {
            JsonUtility.FromJsonOverwrite(json, data);
        }
        catch (System.Exception e)
        {
            // 旧版本用 BinaryFormatter 存过盘，那种文件读出来不是 JSON，会解析失败
            Debug.LogError("[读档] 存档格式不对（可能是旧版本存的），请在游戏里重新保存一次：" + e.Message);
            return null;
        }

        if (data.slots == null) data.slots = new List<SlotSaveData>();
        return data;
    }
}
