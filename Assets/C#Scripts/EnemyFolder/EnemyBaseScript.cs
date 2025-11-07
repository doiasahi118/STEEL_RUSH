using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseScript : MonoBehaviour
{
    [Header("HP")]
    [SerializeField,Tooltip("最大HP")] int maxHP = 100;
    [SerializeField,Tooltip("被弾後のHP")]float hitCoolDown;
    [SerializeField, Tooltip("受けるダメージ倍率")] float apDamage;

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

    void Start()
    {
        Init();
    }

    //初期化処理
    public void Init()
    {
        CurrentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //死亡処理
    void OnDead()
    {
        Debug.Log(gameObject.name + "を倒しました");
        Destroy(gameObject);
    }

    //攻撃ヒット後次の攻撃が当たるまでの待機処理時間
    IEnumerator HitWait()
    {
        //指定時間待機してフラグを戻す
        conHit = false;
        yield return new WaitForSeconds(0.5f);
        conHit = true;
    }
}
