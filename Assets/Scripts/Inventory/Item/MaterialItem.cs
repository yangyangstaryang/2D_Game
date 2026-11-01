using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialItem : BaseItem
{
    public MaterialItem(MaterialItemDataSO data)
    {
        type = ItemType.Material;
        ID = data.ID;
        itemName = data.itemName;
        icon = data.icon;
        prefab = data.prefab;
        canStack = data.canStack;
    }
}