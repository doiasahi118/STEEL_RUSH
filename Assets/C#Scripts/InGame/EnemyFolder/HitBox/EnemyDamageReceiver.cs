using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageReceiver : MonoBehaviour
{
    EnemyBaseScript enemy;

    public void Awake()
    {
        enemy = GetComponent<EnemyBaseScript>();
    }

    //HitData‚ğˆø”‚Æ‚µ‚Äó‚¯æ‚é
    public void ReceiveDamage()
    {

    }
}
