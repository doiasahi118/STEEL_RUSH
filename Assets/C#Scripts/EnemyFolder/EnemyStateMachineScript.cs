using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

//定数クラス
public static class CommonConst
{

}

//エネミーのステート
public enum EnemyState
{
    Patrol,
    Chase,
    Attack,
    Dead
}


[RequireComponent(typeof(EnemyBaseScript))]
public class EnemyStateMachineScript : MonoBehaviour
{
    [Header("ターゲット")]
    [SerializeField] Transform player;

    [Header("距離しきい値")]
    [SerializeField,Tooltip("この距離内で追跡開始")]
    public float chaseDistance = 15.0f;
    [SerializeField,Tooltip("この距離内で攻撃開始")]
    float attackDistance = 2.5f;
    [SerializeField, Tooltip("攻撃のクールダウン")]
    float attackInterval = 1.0f;

    [Header("移動")]
    [SerializeField,Tooltip("NavMashAgentを使って追跡ができる(任意)")]
    bool usNavMesh = true;
    [SerializeField] float stopDistance = 1.0f;

    [Header("参照(任意)")]
    [SerializeField] EnemyMove move;
    [SerializeField] EnemyAttacks attacks;
    [SerializeField] NavMeshAgent agent;

    EnemyBaseScript E_Base;
    public EnemyState currentState { get; private set; } =EnemyState.Patrol;
    float _attackTimer = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
       E_Base = GetComponent<EnemyBaseScript>();
       if(!move) move = GetComponent<EnemyMove>();
       if(!attacks) attacks = GetComponent<EnemyAttacks>();
       if(!agent) agent = GetComponent<NavMeshAgent>();

        //死亡したらDeadへ
        E_Base.OnDeath += () => ChangeState(EnemyState.Dead);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(EnemyState nextState)
    {
        if(currentState == nextState) 
        {
            return;
        }

        //Exit
        OnExit(currentState);

        //状態遷移
        currentState = nextState;

        //Enter
        OnEnter(currentState);
    }

    void OnEnter(EnemyState state)
    {
        switch(state)
        {
            case EnemyState.Patrol:
                if (usNavMesh && agent) { agent.isStopped = true;agent.ResetPath();}
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                _attackTimer = 0f;
                break;
        }
    }

    private void OnExit(EnemyState state)
    {
        
    }

    void TrickIdle(float distSq)
    {
        if (!HasPlayer()) return;
    }

    float DistanceSqToPlayer()
    {
        if (!HasPlayer()) return float.MaxValue;
        return (player.position - transform.position).sqrMagnitude;
    }

    bool HasPlayer() => player != null;

    public void SetPlayer(Transform t) =>player = t;
    public void SetDistances(float chase, float attack)
    {
        chaseDistance = Mathf.Max(0, chaseDistance);
        attackDistance = Mathf.Clamp(attack,0.1f, attackDistance);
    }
}
