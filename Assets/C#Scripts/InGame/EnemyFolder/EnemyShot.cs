using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyShot : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject bulletPrefab;

    [Header("射撃設定")]
    [SerializeField, Tooltip("一秒間に撃つ弾数")]
    float fireRate = 2.0f;
    [SerializeField,Tooltip("攻撃可能距離")]
    public float attackRange = 20.0f;
    [SerializeField,Tooltip("ターゲットがどの位置を狙うかを補正")]
    float aimOffset = 1.5f;
    [SerializeField,Tooltip("弾のバラつき角度")]
    float spreadAngle = 5.0f;

    Transform target;
    //内部状態
    bool isAttacking = false;
    float fireInterval;
    float fireTimer = 0.0f;

   

    public void SetTarget(Transform t)=>target =t;

    void Awake()
   {

         // 念のため Null チェック
        if (!muzzle)
            Debug.LogWarning("[EnemyShot] muzzle が設定されていません", this);
        if (!bulletPrefab)
            Debug.LogWarning("[EnemyShot] bulletPrefab が設定されていません", this);
      
   }

    public void StartAttack()
    {
        if (!bulletPrefab)
        {
            Debug.LogWarning("No Bullet Prefab Set", this);
            return;
        }

        isAttacking = true;
        fireTimer = 0.0f;
        Debug.Log("Enemy Start Attack");
    }
   public void StopAttack()
    {
        isAttacking = false;
        Debug.Log("Enemy Stop Attack");
    }
    void Update()
    {
        if(!isAttacking||target ==null)
        {
            return;
        }

        //ターゲット方向を向く
        Vector3 aimPos = target.position + Vector3.up * aimOffset;
        Vector3 dir = (aimPos - muzzle.position).normalized;
        muzzle.rotation = Quaternion.LookRotation(dir);

        //射撃問題
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = 1f / fireRate;
            Fire(dir);
        }

    }

   void Fire(Vector3 dir)
    {
        if(spreadAngle>0f)
        {
            dir = Quaternion.Euler(
                UnityEngine.Random.Range(-spreadAngle,spreadAngle),
                UnityEngine.Random.Range(-spreadAngle,spreadAngle),
                0) * dir; 
        }
        Debug.Log("[EnemyShot] Fire!", this);

        Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(dir));
    }
}
