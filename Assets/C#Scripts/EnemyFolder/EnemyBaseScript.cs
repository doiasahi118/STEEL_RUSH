using MaykerStudio.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseScript : MonoBehaviour,IDamageable
{
    [Header("HP")]
    [SerializeField,Tooltip("最大HP")] int maxHP = 100;
    [SerializeField,Tooltip("被弾後のHP")]float hitCoolDown=0.4f;
    [SerializeField, Tooltip("受けるダメージ倍率")] float apDamage=1.0f;

    [Header("死亡処理")]
    [SerializeField,Tooltip("死亡フラグから消滅までの時間")] float deathDelay=0.5f;
    [SerializeField,Tooltip("死亡エフェクト")] GameObject deathEffect;
    [SerializeField, Tooltip("死亡時の効果音")] AudioClip deathSE;

    [Header("被弾処理")]
    [SerializeField, Tooltip("被弾時のエフェクト")] GameObject hitEffect;
    [SerializeField,Tooltip("被弾時の効果音")] AudioClip hitSE;

    //ランタイム
    public int CurrentHP{
        get;
        private
        set;
    }

    public bool IsAlive =>CurrentHP > 0;
    bool canBeHit = true;
    //攻撃を受けるフラグ
    bool conHit = true;

    //UIやAI連動用イベント
    public event Action<int, int> OnHpChanged;
    public event Action<int> OnDamage;
    public event Action OnDeath;

    //参照
    Animator anim;
    NavMeshAgent agent;
    Collider[] colliders;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        colliders = GetComponentsInChildren<Collider>(true);
        Init();
    }

    void OnValidate()
    {
        if (maxHP < 1) { maxHP = 1;}
        if (hitCoolDown < 0.0f) {hitCoolDown = 0.0f;}
        if (apDamage<=0.0f) apDamage = 1.0f;
        if (deathDelay<0.0f) deathDelay = 0.0f;
    }

    //初期化処理
    public void Init()
    {
        CurrentHP = maxHP;
        canBeHit = true;
        OnHpChanged?.Invoke(CurrentHP, maxHP);
    }

    //ダメージ受け取り
    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitNormal, GameObject attcker = null)
    {
        if (!IsAlive || !canBeHit) { return; }
        //実ダメージ
        int dmg = Mathf.Max(0, Mathf.RoundToInt(amount * apDamage));
        if (dmg <= 0) {return;}
        //視覚/音
        if (hitEffect) Instantiate(hitEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        if (hitSE) AudioSource.PlayClipAtPoint(hitSE, hitPoint);

        CurrentHP = Mathf.Max(0, CurrentHP - dmg);
        OnHpChanged?.Invoke(CurrentHP, maxHP);
        OnDamage?.Invoke(dmg);

        if(CurrentHP<=0)
        {
            StartCoroutine(DieRoutine());
            return;
        }
        //無敵時間(多段ヒット抑制)
        StartCoroutine(HitCooldownRoutine());
    }

    IEnumerator HitCooldownRoutine()
    {
        canBeHit = false;
        yield return new WaitForSeconds(hitCoolDown);
        canBeHit= true;
    }

    IEnumerator DieRoutine()
    {
        canBeHit = false;
        if (agent){agent.isStopped = true;}
        if (anim){anim.SetTrigger("Die");}
        foreach(var col in colliders) col.enabled = false;

        //エフェクト/音
        if (deathEffect) Instantiate(deathEffect, transform.position, transform.rotation);
        if (deathSE) AudioSource.PlayClipAtPoint(deathSE, transform.position);
        //OnDead?.Invoke();
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        if (!IsAlive) { return; }
        CurrentHP = Mathf.Min(maxHP, CurrentHP + Mathf.Abs(amount));
        OnHpChanged?.Invoke(CurrentHP, maxHP);
    }

    public void ApplyDamage(HitData hit)
    {
        TakeDamage(hit.damage, hit.hitPoint, hit.hitNormal, hit.attacker);
    }
}
