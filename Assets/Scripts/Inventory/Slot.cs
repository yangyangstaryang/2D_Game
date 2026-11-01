using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject selected;
    public Image item_img;
    public TMP_Text amount_txt;

    public bool IsEmpty { get { return amount == 0; } }
    int amount = 0;
    public UnityAction useAction;

    public int itemID;
    BaseItem.ItemType type;
    GameObject prefab;

    public void IsSelected(bool isSelected)
    {
        selected.SetActive(isSelected);
    }

    public void AddItem(int ID, Sprite icon, BaseItem.ItemType type, GameObject prefab)
    {
        // 兜底：没绑定 UI 引用时给出明确提示，而不是抛空引用
        if (item_img == null || amount_txt == null)
        {
            Debug.LogError("Slot 的 item_img / amount_txt 没有绑定，请检查 Slot 预制体", this);
            return;
        }

        itemID = ID;
        this.type = type;
        this.prefab = prefab; 

        item_img.sprite = icon;
        item_img.SetNativeSize();
        item_img.color = Color.white;
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
            item_img.color = Color.clear;
            itemID = -1;
            type = BaseItem.ItemType.None;
            prefab = null;
            useAction = null;
        }
    }

    public void UseItem()
    {
        if (IsEmpty) return;
        if (type != BaseItem.ItemType.Consumable) return;
        useAction?.Invoke();
        RemoveItem();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (prefab == null) return; 
        item_img.transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (prefab == null) return; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (prefab == null) return; 
        item_img.rectTransform.localPosition = Vector2.zero;

        Instantiate(prefab, Camera.main.ScreenToWorldPoint(eventData.position), Quaternion.identity);
        RemoveItem();
    }
}
