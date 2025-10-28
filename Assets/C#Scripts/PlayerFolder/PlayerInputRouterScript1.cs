using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputRoutineScript : MonoBehaviour
{
    PlayerInput input;

    [Header("Receivers")]
    //プレイヤーのコントローラー
    [SerializeField] PlayerController controller;
    //ブーストアクション
    [SerializeField] BoostActionScript booster;
    //クイックブーストアクション
    [SerializeField] QuickBoostActionScript quickBoost;
    //ロックオン
    [SerializeField] LookOnActionScript lockOn;
    //射撃
    [SerializeField] ShotScript shooter;
    //状態管理
    [SerializeField] PlayerStateScript state;
    //近接攻撃
    [SerializeField] MeleeScript melee;

    //アクション名
    [Header("Action_Names")]
    [SerializeField] string quickBoostRightActionName = "QuickBoost";
    [SerializeField] string LookOnNextName = "LookOnNext";
    [SerializeField] string LookOnPrevName = "LookOn";
    [SerializeField] string repairActionName = "Repair";
    [SerializeField] string interactActionName = "Interact";
    [SerializeField] string meleeActionName = "MeleeAttack";
    [SerializeField] string reloadActionName = "Reload";
    [SerializeField] string fireActionName = "Fire";

    //取得したアクションの参照
    InputAction actFire, actReload, actQuickBoost, actRepair, actLookOnToggle, actLookOnNext, actInteract, actMeleeAttack;

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInput>();
        if (!controller) controller = GetComponent<PlayerController>();
        if (!booster) booster = GetComponent<BoostActionScript>();
        if (!quickBoost) quickBoost = GetComponent<QuickBoostActionScript>();
        if (!lockOn) lockOn = GetComponent<LookOnActionScript>();
        if (!shooter) shooter = GetComponent<ShotScript>();
        if (!state) state = GetComponent<PlayerStateScript>();
        if (!melee) melee = GetComponent<MeleeScript>();
    }

    void OnEnable()
    {
        if (input.currentActionMap == null)
        {
            Debug.LogWarning("[PlayerInputRouter] No ActionMap found!");
            return;
        }
        var map = input.currentActionMap;
        map.Enable();

        //アクションの取得
        actFire = FindAndEnable(map, fireActionName);
        actQuickBoost = FindAndEnable(map, quickBoostRightActionName);
        actReload = FindAndEnable(map, reloadActionName);
        actRepair = FindAndEnable(map, repairActionName);
        actLookOnToggle = FindAndEnable(map, LookOnPrevName);
        actLookOnNext = FindAndEnable(map, LookOnNextName);
        actInteract = FindAndEnable(map, interactActionName);
        actMeleeAttack = FindAndEnable(map, meleeActionName);

        //アクションの登録
        if (actQuickBoost != null)
        {
            actQuickBoost.performed += OnQuickBoost;
        }

        if (actRepair != null)
            actRepair.performed += _ =>
            {
                if (state != null && !state.TryRepair())
                    Debug.Log("[PlayerInputRouter] Repair 失敗");
            };


        if (actFire != null)
        {
            actFire.performed += _ => shooter?.StartFire();
            actFire.canceled += _ => shooter?.StopFire();
        }
    }

    private void OnDisable()
    {
        if (actQuickBoost != null)
        {
            actQuickBoost.performed -= OnQuickBoost;
        }

        if (actReload != null)
        {
            actReload.performed -= _ => shooter?.Reload();
        }

        if (actRepair != null)
        {
            actRepair.performed -= _ => state?.TryRepair();
        }

        if (actFire != null)
        {
            actFire.performed -= _ => shooter?.StartFire();
            actFire.canceled -= _ => shooter?.StopFire();
        }
    }

    void OnQuickBoost(InputAction.CallbackContext _)
    {
        if (quickBoost == null) return;

        //現在の移動方向(または前方)を取得
        Vector3 dir = controller != null && controller.LastMoveDir.sqrMagnitude > 0.0001f
        ? controller.LastMoveDir
        : transform.forward;

        //クイックブースト実行
        quickBoost.DoQuickBoost(dir);
    }

    static InputAction FindAndEnable(InputActionMap map, string actionName)
    {
        if (map == null || string.IsNullOrEmpty(actionName)) return null;
        var act = map.FindAction(actionName, throwIfNotFound: false);
        if (act == null)
        {
            Debug.LogWarning($"[InputRouter]Action'{actionName}'が見つかりません");
            return null;
        }
        act.Enable();
        return act;
    }
}

//using System;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerInputRoutineScript : MonoBehaviour
//{
//    PlayerInput input;
//    [Header("Receivers")]
//    [SerializeField] PlayerController controller;
//    [SerializeField] QuickBoostActionScript quickBoost;
//    [SerializeField] LookOnActionScript lockOn;
//    [SerializeField] PlayerStateScript state;
//    [SerializeField] BoostActionScript booster;
//    [SerializeField] ShotScript shooter;
//    [SerializeField] MeleeScript melee;

//    [Header("Action_Name")]
//    [SerializeField] string fireActionName = "Fire";
//    [SerializeField] string quickBoostActionName = "QuickBoost";
//    [SerializeField] string reloadActionName = "Reload";
//    [SerializeField] string repairActionName = "Repair";
//    [SerializeField] string lookOnNextName = "LookOnNext";
//    [SerializeField] string lookOnPrevName = "LookOn";
//    [SerializeField] string meleeActionName = "MeleeAttack";

//    InputAction actFire, actReload, actQuickBoost, actRepair, actMelee, actLookOn, actLookOnNext;

//    void Awake()
//    {
//        //安全に参照
//        input ??= GetComponent<PlayerInput>();
//        controller ??= GetComponent<PlayerController>();
//        booster ??= GetComponent<BoostActionScript>();
//        quickBoost ??= GetComponent<QuickBoostActionScript>();
//        lockOn ??= GetComponent<LookOnActionScript>();
//        shooter ??= GetComponent<ShotScript>();
//        state ??= GetComponent<PlayerStateScript>();
//        melee ??= GetComponent<MeleeScript>();
//    }

//    void OnEnable()
//    {
//        //有効なActionMapを取得
//        var map = input?.currentActionMap;
//        if(map==null)
//        {
//            Debug.LogWarning("[InputRouter]NO_ActionMap");
//            return;
//        }
//    //各アクションマップを検索＆購読
//    actFire =Subsct
//    }




//}