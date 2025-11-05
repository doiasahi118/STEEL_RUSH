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
    [SerializeField,Tooltip("剣速")]float meleeSpeed = 30f;
    [Header("クールタイム")]
    [SerializeField, Tooltip("近接攻撃クールタイム")] float meleeCooldown = 1f;

    [Header("エフェクト設定")]
    [SerializeField] GameObject meleeEffect; //近接攻撃エフェクト
    [SerializeField] AudioClip meleeSound; //近接攻撃音(予定)
    [SerializeField] float effectLifeTime = 0.5f; //エフェクトの生存時間
    AudioSource audioSource;
    //内部状態
    bool isSlash;
    float slashInterval; //1秒あたりの攻撃数の感覚
    float slashTimer; //次攻撃までのタイマー

    private void Awake()
    {
        if (!trs) trs = transform;//念のため
        slashInterval = 1f / Mathf.Max(0.01f, meleeSpeed);
    }

    void Update()
    {
        if (!isSlash) return;
        //押しっぱなしで連続攻撃
        slashTimer += Time.deltaTime;
        while (slashTimer >= slashInterval)
        {
            slashTimer -= slashInterval;
            if (!TryMeleeOnce())
            {
                //クールタイム終了
                break;
            }
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
            GameObject bread = Instantiate(breadPrefab, trs.position + trs.forward * 1f, trs.rotation);
            Rigidbody rb = bread.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = trs.forward * meleeSpeed;
            }
        }
        return true;
    }

    public void StartAttack()
    {
        Debug.Log("Melee Attack Started");
        isSlash = true;
    }

    public void EndAttack()
    {
        isSlash = false;
    }
}