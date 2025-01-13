using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public float attackDamage;

    public delegate void OnHitEnemy(GameObject enemy, float damage);
    public static event OnHitEnemy onHitEnemy;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Invoke event for damaging enemy
            onHitEnemy?.Invoke(other.gameObject, attackDamage);
        }
    }

}
