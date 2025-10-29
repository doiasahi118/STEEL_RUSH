//using System;
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;

//public class QuickBoostActionScript : MonoBehaviour
//{
//    [Header("QuickBoost 設定")]
//    [SerializeField] float dashSpeed = 1000f; // ダッシュ速度
//    [SerializeField] float dashDuration = 0.20f; // ダッシュ持続時間
//    [SerializeField] float cooldown = 0.6f; // クールダウン時間
//    [SerializeField] float boostCount = 25f; // ブースト消費量

//    PlayerController Controller;
//    PlayerStateScript State;

//    bool busy = false;
//    float cooldownTimer = 0f;

//     void Awake()
//    {
//        Controller = GetComponent<PlayerController>();
//        State = GetComponent<PlayerStateScript>();
//    }

//    void Update()
//    {
//        if(cooldownTimer >0f)
//        {
//            cooldownTimer -= Time.deltaTime;
//        }
//    }

//    //方向ベクトルはワールド空間。長狭は無視してよい
//    public void DoQuickBoost(Vector3 direction)
//    {
//        if(busy||cooldownTimer>0f)
//        {
//            return;
//        }
//        //スタミナ消費ができないなら中止
//        if(State ==null || !State.SpendBoost(boostCount))
//        {
//            return;
//        }
//        //水平成分だけ0なら前方
//        Vector3 dir = Vector3.ProjectOnPlane(direction, Vector3.up);
//        if(dir.sqrMagnitude < 0.0001f)
//        {
//            dir = transform.forward; 
//            dir.Normalize();
//            //PlayerControllerにインパルスを追加する
//            Controller?.AddImpulse(dir * dashSpeed, dashDuration);

//            //連打防止
//            cooldownTimer = cooldown;
//        }
//    }

//    System.Collections.IEnumerator FlagRoutine()
//    {
//        busy = true;
//        yield return new WaitForSeconds(dashDuration);
//        busy = false;
//    }   
//}
using System.Collections;
using UnityEngine;

public class QuickBoostActionScript : MonoBehaviour
{
    [Header("QuickBoost 設定")]
    [SerializeField] float dashSpeed = 1000f;   // 瞬間速度（m/s 相当）
    [SerializeField] float dashDuration = 0.20f;   // 持続時間（秒）
    [SerializeField] float cooldown = 0.60f;   // クールダウン（秒）
    [SerializeField] float boostCost = 25f;     // 消費スタミナ（瞬間消費）

    PlayerController controller;
    PlayerStateScript state;

    bool busy = false;
    float cooldownTimer = 0f;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        state = GetComponent<PlayerStateScript>();
    }

    void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    /// <summary>direction はワールド空間。長さは無視され正規化されます。</summary>
    public void DoQuickBoost(Vector3 direction)
    {
        // クールダウン・多重実行中は無視
        if (busy || cooldownTimer > 0f) return;

        // スタミナ瞬間消費（SpendBoost が毎秒消費方式なら専用メソッドを作る）
        if (state == null || !TrySpendBoostInstant(state, boostCost)) return;

        // 水平（XZ）成分に投影 → 方向がほぼゼロなら前方に
        Vector3 dir = Vector3.ProjectOnPlane(direction, Vector3.up);
        if (dir.sqrMagnitude < 0.0001f) dir = transform.forward;
        dir.Normalize();

        // ★ ここで必ず AddImpulse を呼ぶ（if の外！）
        controller?.AddImpulse(dir * dashSpeed, dashDuration);

        // フラグとクールダウン開始
        StartCoroutine(FlagRoutine());
        cooldownTimer = cooldown;
    }

    IEnumerator FlagRoutine()
    {
        busy = true;
        yield return new WaitForSeconds(dashDuration);
        busy = false;
    }

    // SpendBoost が「毎秒消費」実装の場合の代替（瞬間消費）
    bool TrySpendBoostInstant(PlayerStateScript st, float amount)
    {
        if (st.Boost < amount) return false;
        st.AddBoost(-amount); // AddBoost に負値を渡す or st.Boost -= amount; + OnBoostChanged 呼び出し
        return true;
    }
}