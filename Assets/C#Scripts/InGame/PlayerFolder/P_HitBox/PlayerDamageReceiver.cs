using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Corrected the spelling of "Reciver" to "Receiver" to fix the spelling error.
public class PlayerDamageReceiver : MonoBehaviour
{
    [Header("受け渡す先")]
    PlayerStateScript playerState;
    [SerializeField] MonoBehaviour target;

    IDamageable damageable;
    private void Awake()
    {
        damageable = target as IDamageable;

        if(damageable ==null)
        {
            Debug.LogError("PlayerDamageReceiverのTargetにIDamageableを実装したスクリプトをアタッチしてください");
        }
    }

    public void ReceiveDamage(HitData hit)
    {
        if (damageable != null)
        {
            damageable.ApplyDamage(hit);
        }
    }

}
