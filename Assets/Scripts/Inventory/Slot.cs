using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject selected;
    public Image item;
    public TMP_Text amount_txt;

    public bool IsEmpty { get { return amount == 0; } }
    public int itemID;
    int amount = 0;
    public UnityAction useAction;
    BaseItem.ItemType itemType;

    //private void OnEnable()
    //{
    //    selected = transform.Find("Selected").gameObject;
    //    item = transform.Find("Item").GetComponent<Image>();
    //    amount_txt = transform.Find("Amount").GetComponent<TMP_Text>();
    //}

    public void IsSelected(bool isSelected)
    {
        selected.SetActive(isSelected);
    }

    public void AddItem(int ID, Sprite icon, BaseItem.ItemType type)
    {
        itemID = ID;
        // item = transform.Find("Item").GetComponent<Image>();
        item.sprite = icon;
        itemType = type;
        item.SetNativeSize();
        item.color = Color.white;
        amount++;
        amount_txt.text = amount > 1 ? amount.ToString() : "";
    }

    public void RemoveItem() 
    {
        if (IsEmpty) return;
        amount--;
        amount_txt.text = amount > 1 ? amount.ToString() : "";
        if (amount == 0)
        { 
            item.color = Color.clear;
            itemID = -1;
            useAction = null;
            itemType = BaseItem.ItemType.None;
        }
    }

    public void UseItem()
    {
        if (IsEmpty) return;
        if (itemType != BaseItem.ItemType.Consumable) return;
        useAction?.Invoke();
        RemoveItem();
    }
}