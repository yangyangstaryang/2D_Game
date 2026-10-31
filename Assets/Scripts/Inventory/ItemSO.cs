using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "CreateData/ItemData")]
public class ItemSO : ScriptableObject
{
    public int ID;
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public bool canStack;
}
