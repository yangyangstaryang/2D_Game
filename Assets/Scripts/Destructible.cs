using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject breakParticlePrefab;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponentInChildren<AttackChecker>())
        {
            Instantiate(breakParticlePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
