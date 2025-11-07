using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    [SerializeField] float health = 100.0f;
    private float currentHP;
    void Start()
    {
        float maxHP = health;
        currentHP = maxHP ;
    }
   
    public void TakeDamage(float dmg)
    {
               currentHP -= dmg;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
               Destroy(gameObject);
    }
    
}
