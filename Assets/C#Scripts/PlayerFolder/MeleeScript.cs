using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeScript : MonoBehaviour
{
    [Header("必須参照")]
    [SerializeField] Transform trs;
    [SerializeField] GameObject breadPrefab;
    [Header("攻撃設定")]
    [SerializeField,Tooltip("剣速")]float meleeSpeed = 30.0f;
    [Header("クールタイム")]
    [SerializeField, Tooltip("近接攻撃クールタイム")] 
    float meleeCooldown = 0.25f;

    [Header("エフェクト設定")]
    [SerializeField] GameObject meleeEffect; //近接攻撃エフェクト
    [SerializeField] AudioClip meleeSound; //近接攻撃音(予定)
    [SerializeField] float effectLifeTime = 0.5f; //エフェクトの生存時間
    AudioSource audioSource;
    //内部状態
    bool isSlash;
    float repeatTimer; //1秒あたりの攻撃数の感覚
    float interval; //次攻撃までのタイマー
    float coolDownTimer;  //クールダウン
    private void Awake()
    {
        if (!trs) trs = transform;//念のため
        interval = 1f / Mathf.Max(0.01f, meleeSpeed);
    }

    void Update()
    {
        //クールダウンの減算
        if(coolDownTimer>0)
        {
            coolDownTimer -= Time.deltaTime;
        }
        if (!isSlash)
        {
            return;
        }
        //押しっぱなしで連続攻撃
        repeatTimer -= Time.deltaTime;
        if (repeatTimer <= 0f)
        {
            TryMeleeOnce();
            coolDownTimer -= meleeCooldown;

        }
        else
        {
            //クールタイム中なら次フレームでもう一度チェック
            repeatTimer -=0.01f;
        }
    }

    bool TryMeleeOnce()
    {
        //近接攻撃処理
        //エフェクト生成
        if (meleeEffect)
        {
            GameObject effect = Instantiate(meleeEffect, trs.position, trs.rotation);
            Destroy(effect, effectLifeTime);
        }
        //音再生
        if (meleeSound)
        {
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(meleeSound);
        }
        
        if (breadPrefab)
        {
            //GameObject bread = Instantiate(breadPrefab, trs.position + trs.forward * 1f, trs.rotation);
            //Rigibody rb = bread.GetComponent<Rigibody>();
            //if (rb)
            //{
            //    rb.velocity = trs.forward * meleeSpeed;
            //}
            var bread = Instantiate(breadPrefab, trs.position + trs.forward * 1f, trs.rotation);
            if (bread.TryGetComponent<Rigidbody>(out var rb))
                rb.velocity = trs.forward * 30.0f;
        }
        return true;
    }

    public void StartAttack()
    {
        Debug.Log("Melee Attack Started");
        if (coolDownTimer > 0f)
        {
            return;
        }

        TryMeleeOnce();
        coolDownTimer = meleeCooldown;

        //連撃用のタイマーの初期化(すぐ次が出ないようIntervalをSet)
        repeatTimer = interval;
        isSlash = true;
    }

    public void EndAttack()
    {
        Debug.Log("Melee canceled");
        isSlash = false;
    }
}