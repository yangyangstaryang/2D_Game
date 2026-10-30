using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(2, transform);
        }
    }
}
