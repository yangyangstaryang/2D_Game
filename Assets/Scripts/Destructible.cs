using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Destructible : MonoBehaviour
{
    public GameObject breakParticlePrefab;
    public GameObject CoinPrefab;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInChildren<AttackChecker>())
        {
            Instantiate(breakParticlePrefab, transform.position, Quaternion.identity);
            float probability = Random.Range(0, 1f);
            if (probability < 0.8f)
            {
                Instantiate(CoinPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
