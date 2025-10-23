using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] PlayerStateScript playerState;
    [Header("AP")]
    [SerializeField] Slider apSlider;
    [SerializeField] TextMeshProUGUI apText;
    [Header("BOOST")]
    [SerializeField] Slider boostSlider;
    [SerializeField] TextMeshProUGUI boostText;
    [Header("REPEA")]
    [SerializeField] TextMeshProUGUI repairChargeText;

    private void Awake()
    {
        if(!playerState)
        {
            playerState = FindObjectOfType<PlayerStateScript>();
        }
    }

    void OnEnable()
    {
        ApplyAll();
        playerState.OnAPChanged += OnAP;
        playerState.OnBoostChanged += OnBoost;
        playerState.OnRepairChargeChanged += OnRepair;
        playerState.OnPlayerDead += OnDead;
    }

    void OnDisable()
    {
        playerState.OnAPChanged -= OnAP;
        playerState.OnBoostChanged -= OnBoost;
        playerState.OnRepairChargeChanged -= OnRepair;
        playerState.OnPlayerDead -= OnDead;
    }

    void ApplyAll()
    {
        OnAP(playerState.AP, playerState.MaxAP);
        OnBoost(playerState.Boost, playerState.MaxBoost);
        OnRepair(playerState.RepairCharge, playerState.MaxRepairCharge);
    }

    void OnAP(float current, float max)
    {
        apSlider.maxValue = max;
        apSlider.value = current;
        if(apText)apText.text = current .ToString("0") ;
    }
    void OnBoost(float current, float max)
    {
       boostSlider.maxValue = max;
         boostSlider.value = current;
        if(boostText)boostText.text = current.ToString("0");
    }
    void OnRepair(int current, int max)
    {
       if(repairChargeText)repairChargeText.text = $"REPEA{current}";
    }
    void OnDead()
    {
        //You can add some UI effects here
        ApplyAll();
    }
}
