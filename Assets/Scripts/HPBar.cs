using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    public Transform hpBar;
    public float showTime = 1f;

    private Coroutine coroutine;
    
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
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        // StopAllCoroutines();
        gameObject.SetActive(true);
        hpBar.localScale = new Vector3(hpPercent, 1, 1);
        coroutine =  StartCoroutine(SetShowTime());
    }

    IEnumerator SetShowTime()
    {
        yield return new WaitForSeconds(showTime);
        gameObject.SetActive(false);
    }
}
