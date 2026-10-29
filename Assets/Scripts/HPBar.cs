using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public Transform hpBar;
    public float showTime = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        // hpBar.localScale = new Vector3(0.5f, 1, 1);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHPBar(float hpPercent)
    {
        StopCoroutine(SetShowTime());
        gameObject.SetActive(true);
        hpBar.localScale = new Vector3(hpPercent, 1, 1);
        StartCoroutine(SetShowTime());
    }

    IEnumerator SetShowTime()
    {
        yield return new WaitForSeconds(showTime);
        gameObject.SetActive(false);
    }
}
