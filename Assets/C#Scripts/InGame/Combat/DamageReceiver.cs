using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] MonoBehaviour targetHandler;
    IDamageable damageable;

    void Awake()
    {
        damageable = targetHandler as IDamageable;
        if(damageable == null)
        {
            Debug.LogWarning($"{name}:targetHandler‚ªIDamageable‚ðŽÀ‘•‚µ‚Ä‚¢‚Ü‚¹‚ñ");
        }
    }

    public void ReceiveDamage(HitData hit)
    {
        if(damageable !=null)
        {
            damageable.ApplyDamage(hit);
        }
    }
}
