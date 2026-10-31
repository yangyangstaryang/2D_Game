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

    public void IsSelected(bool isSelected)
    {
        selected.SetActive(isSelected);
    }

    public void Init(Sprite icon, int amount)
    {
        item.sprite = icon;
        item.color = Color.white;
        amount_txt.text = amount > 1 ? amount.ToString() : "";
    }
    
}
