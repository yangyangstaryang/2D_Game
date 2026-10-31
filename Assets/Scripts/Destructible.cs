using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Destructible : MonoBehaviour
{
    public GameObject breakParticlePrefab;
    public GameObject CoinPrefab;
    public GameObject BloodPrefab;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInChildren<AttackChecker>())
        {
            Instantiate(breakParticlePrefab, transform.position, Quaternion.identity);
            float probability = Random.Range(0, 1f);
            if (probability < 0.3f)
            {
                Instantiate(CoinPrefab, transform.position, Quaternion.identity);
            }
            else if (probability < 0.6f)
            {
                Instantiate(BloodPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
