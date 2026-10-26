using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChecker : MonoBehaviour
{
    private PlayerController player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Slime>())
        {
            Slime slime = collision.gameObject.GetComponent<Slime>();
            slime.TakeDamage(1, player.transform);
        }
    }
}
