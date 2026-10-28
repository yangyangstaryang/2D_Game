using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageNum : MonoBehaviour
{
    public TMP_Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        // text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroySelf()
    {
        Destroy(transform.parent.gameObject);
    }

    public void SetNum(int num)
    {
        text.text = num.ToString();
    }
}
