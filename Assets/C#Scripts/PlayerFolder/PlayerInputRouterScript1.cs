using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputRoutineScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController controller; //Playerの移動・制御を担当するScript
    [SerializeField] QuickBoostActionScript quickBoostAction; //QuickBoostを担当するScript
    [SerializeField] PlayerStateScript state; //Playerの状態を管理するScript
    [SerializeField] ShotScript shooter; //射撃を担当するScript
    [SerializeField] MeleeScript melee; //近接攻撃を担当するScript

    private PlayerInput input;
    private InputAction actQuickBoost;
    private InputAction actFire;
    private InputAction actReload;
    private InputAction actRepair;
    private InputAction actMelee;

    [Header("Action_Name")] //InputActionの名前
    [SerializeField] string quickBoostActionName = "QuickBoost";
    [SerializeField] string fireActionName = "Fire";
    [SerializeField] string reloadActionName = "Reload";
    [SerializeField] string repairActionName = "Repair";
    [SerializeField] string meleeActionName = "Melee";

    // ---- ここでデリゲートを保持（解除が確実に効く）----
    System.Action<InputAction.CallbackContext> onFirePerformed, onFireCanceled;
    System.Action<InputAction.CallbackContext> onReloadPerformed;
    System.Action<InputAction.CallbackContext> onRepairPerformed;
    System.Action<InputAction.CallbackContext> onMeleePerformed, onMeleeCanceled;

    //初期化処理
    void Awake()
    {
        ////PlayerInputの取得
        //input = GetComponent<PlayerInput>();
        ////nullなら同じGameObjectから自動で参照を取得
        //controller??= GetComponent<PlayerController>();
        //quickBoostAction??= GetComponent<QuickBoostActionScript>();
        //state??= GetComponent<PlayerStateScript>();
        //shooter??= GetComponent<ShotScript>();
        //melee??= GetComponent<MeleeScript>();
        input = GetComponent<PlayerInput>();
        controller = controller ? controller : GetComponent<PlayerController>();
        quickBoostAction = quickBoostAction ? quickBoostAction : GetComponent<QuickBoostActionScript>();
        state = state ? state : GetComponent<PlayerStateScript>();
        shooter = shooter ? shooter : GetComponent<ShotScript>();
        melee = melee ? melee : GetComponent<MeleeScript>();
    }

    //有効化時:アクションマップを取得してイベントを登録

    void OnEnable()
    {
        if(input ==null||input.currentActionMap ==null)
        {
            Debug.LogWarning("[InputRouter]PlayerInputがActionMapがありません");
        }
        //現在有効なアクションマップ(Player)を取得
        var actionMap = input.currentActionMap;

        //念のためにアクションマップを有効化
        actionMap.Enable();

        //各アクションを名前で検索
        actQuickBoost = actionMap.FindAction(quickBoostActionName);
        actFire = actionMap.FindAction(fireActionName);
        actReload = actionMap.FindAction(reloadActionName);
        actRepair = actionMap.FindAction(repairActionName);
        actMelee = actionMap.FindAction(meleeActionName);
        //イベント登録
        //performed:アクションが実行されたときに呼ばれる
        //canceled:はボタンが話された瞬間によばれる

        //クイックブースト
        if (actQuickBoost != null)
        {
            actQuickBoost.performed += OnQuickBoost;
        }

        //射撃(押しっぱなしで連射、押すと停止)
        if(actFire != null)
        {
            actFire.performed += _ =>shooter?.StartFire();
            actFire.canceled += _ =>shooter?.StopFire();
        }

        if(actReload !=null)
        {
            actReload.performed -=_=>shooter?.Reload();
        }

        if(actReload !=null)
        {
            actRepair.performed -=_=>state?.TryRepair();
        }

        onMeleePerformed = ctx => { Debug.Log("[Router] Melee performed"); melee?.StartAttack(); };
        onMeleeCanceled = ctx => { Debug.Log("[Router] Melee canceled"); melee?.EndAttack(); };
        actMelee.performed += onMeleePerformed;
        actMelee.canceled += onMeleeCanceled;
        Debug.Log("[Router] Melee hooked");

    }

    void OnDisable()
    {
        //解除
        if (actQuickBoost != null)
        {
            actQuickBoost.performed -= OnQuickBoost;
        }

        if (actReload!=null)
        {
            actReload.performed -=_=>shooter?.Reload();
        }

        if (actFire != null)
        {
            actFire.performed -=_=>shooter.StartFire();
            actFire.canceled -= _=>shooter?.StopFire();
        }

        if (actRepair != null)
        {
            actRepair.performed -=_=>state.TryRepair();
        }

        if (actMelee != null)
        {
            if (onMeleePerformed != null) actMelee.performed -= onMeleePerformed;
            if (onMeleeCanceled != null) actMelee.canceled -= onMeleeCanceled;
        }
    }

    //クイックブースト処理
    //InputAction.CallbackContextは
    //Input System が渡す入力情報(押下状態、時間、デバイス)を持つ構造体
    void OnQuickBoost(InputAction.CallbackContext ctx)
    {
        //QuickBoost スクリプトが未アタッチならなら何もしない
        if(quickBoostAction == null)
        {
          return;
        }

        //現在の移動方向を取得(移動していない場合は前方)
        Vector3 dir=(controller !=null&&controller.LastMoveDir.sqrMagnitude>0.0001f)
            ?controller.LastMoveDir
            :transform.forward;
        //クイックブースト実行
        quickBoostAction.DoQuickBoost(dir);
    }

    static InputAction Find(InputActionMap map,string name)
    {
        if(string.IsNullOrEmpty(name))return null;
        var act = map.FindAction(name,throwIfNotFound:false);
        if (act == null)
        {
            Debug.LogWarning($"[InputRouter]Action'{name}'が見つかりません");
            return null;
        }
        act.Enable();
        return act;
    }
}

