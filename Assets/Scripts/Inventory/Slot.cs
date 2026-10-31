using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject selected;
    public Image item;
    public TMP_Text amount_txt;

    public bool IsEmpty { get { return amount == 0; } }
    private int amount=0;
    public int itemID;
    

    public void IsSelected(bool isSelected)
    {
        selected.SetActive(isSelected);
    }

    public void AddItem(ItemSO data)
    {
        itemID = data.ID;
        item.sprite = data.icon;
        item.SetNativeSize();
        item.color = Color.white;
        amount++;
        amount_txt.text = amount > 1 ? amount.ToString() : "";
    }

    public void RemoveItem()
    {
        if(IsEmpty) return;
        amount--;
        amount_txt.text = amount > 1 ? amount.ToString() : "";
        if (amount == 0)
        {
            item.color = Color.clear;
            itemID = -1;
        }
    }
}
