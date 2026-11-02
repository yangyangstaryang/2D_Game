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
    public int Amount { get { return amount; } }
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

        if (InventorySystem.Instance != null) InventorySystem.Instance.SyncSnapshot();
    }

    /// <summary>按存档一次性还原这一格（数量直接指定，不累加）</summary>
    public void RestoreItem(int ID, Sprite icon, BaseItem.ItemType type, GameObject prefab, int count)
    {
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

        amount = count;
        amount_txt.text = amount > 1 ? amount.ToString() : "";
    }

    /// <summary>清空这一格（读档前先清一遍，避免和旧物品叠加）</summary>
    public void ClearItem()
    {
        amount = 0;
        itemID = -1;
        type = BaseItem.ItemType.None;
        prefab = null;
        useAction = null;
        if (amount_txt != null) amount_txt.text = "";
        if (item_img != null) item_img.color = Color.clear;
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

        if (InventorySystem.Instance != null) InventorySystem.Instance.SyncSnapshot();
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
