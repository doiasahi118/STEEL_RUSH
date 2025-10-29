using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoostActionScript : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] PlayerController move;
    [SerializeField] PlayerStateScript stats;
    [SerializeField] PlayerInput input;
    [SerializeField]InputAction boostAction;    

    [Header("Boost_Settings")]
    [SerializeField] float boostMultiplier = 2f;
    [SerializeField] float drainPerSec = 25f;
    [SerializeField] float accelLerp = 10f;
    [SerializeField] float decelLerp = 8f;
    [SerializeField] public bool isBoosting = false;

    private void Awake()
    {
        if (!move) move = GetComponent<PlayerController>();
        if (!stats) stats = GetComponent<PlayerStateScript>();
        if (!input) input = GetComponent<PlayerInput>();

        if (input && input.currentActionMap != null)
        {
            input.currentActionMap.Enable();
            boostAction = input.currentActionMap.FindAction("Boost");
            boostAction?.Enable();
        }
    }

    private void OnEnable()
    {
        if (input && input.currentActionMap != null)
        {
            input.currentActionMap.Enable();
            boostAction = input.currentActionMap.FindAction("Boost");
            boostAction?.Enable();
        }
    }

    private void OnDisable()
    {
        boostAction?.Disable();
    }



    // Update is called once per frame
    void Update()
    {
        bool westBoost = boostAction != null && boostAction.IsPressed();

        if(westBoost&&stats.SpendBoost(drainPerSec))
        {
            isBoosting = true;
            move.SpeedMultiplier = Mathf.Lerp(move.SpeedMultiplier, boostMultiplier, accelLerp*Time.deltaTime);   
        }

        else
        {
            isBoosting = false;
            move.SpeedMultiplier = Mathf.Lerp(move.SpeedMultiplier, 1f, decelLerp*Time.deltaTime);
        }
    }
}

 
