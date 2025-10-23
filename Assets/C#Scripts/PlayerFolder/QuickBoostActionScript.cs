using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class QuickBoostActionScript : MonoBehaviour
{
    [Header("QuickBoost_設定")]
    [SerializeField] float boostForce = 40f;
    [SerializeField] float boostDuration = 0.2f;
    [SerializeField] float cooldown = 2f;
    [SerializeField] float boostCost = 25f;

    Rigidbody rb;
    PlayerStateScript state;
    PlayerController controller;
    

    bool boosting = false;
    float cooldownTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        state = GetComponent<PlayerStateScript>();
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer = Math.Max(0f,cooldownTimer-Time.deltaTime);
        }
    }

    IEnumerable BoostRoutine(Vector3 dir)
    {
        boosting = true;
        cooldownTimer = cooldown;

        float timer = 0f;
        rb.velocity = Vector3.zero;
        var wait = new WaitForFixedUpdate();
       
        while(timer < boostDuration)
        {
            rb.AddForce(dir.normalized * boostForce, ForceMode.VelocityChange);
            yield return wait;
            timer += Time.fixedDeltaTime;
        }
        boosting = false;
    }

    public void DoQuickBoost(Vector3 direction)
    {
        if (boosting || cooldownTimer > 0f) return;
        if (direction.sqrMagnitude<0.0001f)
        {
            if (controller != null && controller.LastMoveDir.sqrMagnitude > 0.0001f)
                direction = controller.LastMoveDir;
            else
                direction = transform.forward;
        }
        //ブースト消費できないならキャンセル
        if (!state || !state.SpendBoost(boostCost)) return;
        StartCoroutine(BoostRoutine(direction));
    }

    private void StartCoroutine(IEnumerable enumerable)
    {
        throw new NotImplementedException();
    }
}
