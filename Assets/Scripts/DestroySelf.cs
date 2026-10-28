using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float delayTime = 0.1f;
    private float destroyTime;
    public SpriteRenderer sr;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelfCoroutine());
        destroyTime = Time.time + delayTime;
    }

    // Update is called once per frame
    void Update()
    {
        float a = (destroyTime - Time.time) / delayTime;
        sr.color = new Color(1, 1, 1, a);
    }

    IEnumerator DestroySelfCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject);
    }
}
