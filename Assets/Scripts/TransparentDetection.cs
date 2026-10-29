using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentDetection : MonoBehaviour
{
    [Range(0, 1)] public float transparency = 0.8f;
    public float fadeTime = 0.5f;
    
    SpriteRenderer sr;
    float elapsedTime;
    private Color color;
    
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            StopAllCoroutines();
            StartCoroutine(Fade(transparency));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            StopAllCoroutines();
            StartCoroutine(Fade(1f));
        }
    }

    IEnumerator Fade(float target)
    {
        elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime +=  Time.deltaTime;
            // 插值 Lerp (初始值，目标值，权重)
            float alpha = Mathf.Lerp(color.a, target, elapsedTime / fadeTime);
            color.a = alpha;
            sr.color = color;
            yield return null;
        }
    }
}
