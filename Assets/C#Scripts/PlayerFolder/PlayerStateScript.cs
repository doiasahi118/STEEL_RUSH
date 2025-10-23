using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateScript : MonoBehaviour
{
    [Header("耐久値(AP)")]
    [SerializeField] float maxAP = 1000f;
    public float MaxAP => maxAP;
    public float AP { get; private set; }

    [Header("自己修復(REPEA)")]
    [SerializeField] int maxRepairCharge = 3;
    public int MaxRepairCharge => maxRepairCharge;
    public int RepairCharge { get; private set; }//残り回数
    [SerializeField] float repairAmount = 300f;//一回の回復量

    [Header("ブースト(スタミナ)")]
    [SerializeField] float maxBoost = 100f;
    public float MaxBoost => maxBoost;
    public float Boost { get; private set; }
    //[SerializeField] float boostConsumptionRate = 20f;//1秒あたりの消費量
    [SerializeField] float boostRegenDelay = 2.5f;//1秒あたりの回復量

    float boostRegenTimer;

    public float AttackPower = 30f;//近接
    public float MoveSpeed = 30f;
    public float jumpHeight = 15.0f;

    //UI/他システムに通知するイベント
    public event Action<float, float> OnAPChanged;//現在値、最大値
    public event Action<int, int> OnRepairChargeChanged;//現在値、最大値
    public event Action<float, float> OnBoostChanged;//現在値、最大値
    public event Action OnPlayerDead;

    private void Awake()
    {
        AP = maxAP;
        RepairCharge = maxRepairCharge;
        Boost = maxBoost;
    }

    // Update is called once per frame
    void Update()
    {
        //ブースト回復
       if(boostRegenTimer>0f)
        {
            boostRegenTimer -= Time.deltaTime;
        }

       else if(Boost<maxBoost)
        {
            Boost = Mathf.Min(maxBoost, Boost + boostRegenDelay * Time.deltaTime);
            OnBoostChanged?.Invoke(Boost,maxBoost);
        }

    }

    public void TakeDamage(float amount)
    {
        if (AP <= 0f) return;//すでに死亡している
        AP = Mathf.Max(0f, AP - Mathf.Abs(amount));
        OnAPChanged?.Invoke(AP, maxAP);
        if (AP <= 0f)
        {
            //死亡
            OnPlayerDead?.Invoke();
        }
    }

    public bool TryRepair()
    {
        if (RepairCharge <= 0 || AP <= 0f || AP >= maxAP) return false;//回復不可
        RepairCharge--;
        AP = Mathf.Min(maxAP, AP + repairAmount);
        OnAPChanged?.Invoke(AP, maxAP);
        OnRepairChargeChanged?.Invoke(RepairCharge, maxRepairCharge);
        return true;
    }

    public bool SpendBoost(float amountPerSec)
    {
        if (Boost <= 0f) return false;//ブースト不可
        Boost = Mathf.Max(0f, Boost - Mathf.Abs(amountPerSec) * Time.deltaTime);
        boostRegenTimer = boostRegenDelay;//1秒後から回復開始
        OnBoostChanged?.Invoke(Boost, maxBoost);
        return Boost > 0.0f;
    }

    public void AddBoost(float amount)
    {
        Boost = Mathf.Min(maxBoost, Boost + amount);
        OnBoostChanged?.Invoke(Boost, maxBoost);
    }
}
